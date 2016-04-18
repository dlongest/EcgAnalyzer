using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcgAnalyzer.Extensions;

namespace EcgAnalyzer
{
    public class CategorizedWaveforms
    {
        public CategorizedWaveforms(IEnumerable<WaveformReadings> normal, IEnumerable<WaveformReadings> arrhythmias)
        {
            this.NormalRhythms = normal;
            this.Arrhythmias = arrhythmias;
        }


    public IEnumerable<WaveformReadings> NormalRhythms { get; private set; }

    public IEnumerable<WaveformReadings> Arrhythmias { get; private set; }

        public IEnumerable<WaveformReadings> Combine()
        {
            return this.NormalRhythms.Concat(Arrhythmias);
        }


        /// <summary>
        /// Given the KMeans instance, ClusterAll will combine all of the contained WaveformReadings into a single sequence
        /// (i.e. concatenate Normal and Arrhythmias), then will use KMeans to cluster the readings (treating each
        /// individual WaveFormReadings as a vector).  All the readings are trimmed such that their length is equal to 
        /// the shortest reading in the combined set.  After the clustering, the EncodedPatientWaveforms instance
        /// that is returned contains the encoded version of each WaveformReadings.  The encoded version is simply 
        /// the quantization of each WaveformReadings into its cluster identifier.  
        /// </summary>
        /// <param name="kmeans"></param>
        /// <returns></returns>
        public QuantizedWaveforms ClusterAll(KMeans kmeans)
        {
            var combined = this.Combine().AsArray();
            var minimum = combined.MinimumWaveformLength();
            var trimmed = combined.Trim(minimum);

            var codebook = kmeans.Compute(trimmed);

            var normalEncoded = codebook.Take(this.NormalRhythms.Count()).ToArray();
            var arrhythmiasEncoded = codebook.Skip(this.NormalRhythms.Count()).Take(this.Arrhythmias.Count()).ToArray();

            return new QuantizedWaveforms
            {
                Normal = normalEncoded,
                Arrhythmias = arrhythmiasEncoded
            };
        }
    }   

    public class CategorizedWaveformCollection
    {
        private IList<CategorizedWaveforms> waveforms;

        public CategorizedWaveformCollection(IEnumerable<CategorizedWaveforms> waveforms)
        {
            if (waveforms == null)
                this.waveforms = new List<CategorizedWaveforms>();
            else
                this.waveforms = waveforms.ToList();
        }

        public void Add(CategorizedWaveforms waveform)
        {
            this.waveforms.Add(waveform);
        }

        public IEnumerable<CategorizedWaveforms> Waveforms
        {
            get
            {
                return this.waveforms;
            }
        }

        public IEnumerable<QuantizedWaveforms> ClusterAll(KMeans kmeans)
        {
            var combined = this.waveforms.Select(a => a.Combine().AsArray());
            var minimum = combined.Select(a => a.MinimumWaveformLength()).Min();
            var trimmed = combined.Select(a => a.Trim(minimum));
            
            var codebook = kmeans.Compute(trimmed.SelectMany(b => b).ToArray());

            var normalEncoded = this.waveforms.Select(a => a.NormalRhythms.AsArray()).ToArray()
                                              .Select(b => b.Trim(minimum))
                                              .Select(c => kmeans.Clusters.Nearest(c));

            var arrhythmiasEncoded = this.waveforms.Select(a => a.Arrhythmias.AsArray()).ToArray()
                                                   .Select(b => b.Trim(minimum))
                                                   .Select(c => kmeans.Clusters.Nearest(c));

            return normalEncoded.Zip(arrhythmiasEncoded, 
                                    (n, a) => new QuantizedWaveforms()
                                                                    {
                                                                        Normal = n,
                                                                        Arrhythmias = a
                                                                    });

            //var normalEncoded = codebook.Take(this.NormalRhythms.Count()).ToArray();
            //var arrhythmiasEncoded = codebook.Skip(this.NormalRhythms.Count()).Take(this.Arrhythmias.Count()).ToArray();

            //return new QuantizedWaveforms
            //{
            //    Normal = normalEncoded,
            //    Arrhythmias = arrhythmiasEncoded
            //};
        }

    }

    public class QuantizedWaveforms
    {
        public int[] Normal { get; set; }

        public int[] Arrhythmias { get; set; }
    }      
}
