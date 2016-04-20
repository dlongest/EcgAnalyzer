using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcgAnalyzer.Extensions;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;

namespace EcgAnalyzer
{
    public class WaveformModels
    {
        private int maximumWaveformLength;
        private readonly KMeans kmeans;
        private readonly int numberStates;
        private readonly int numberSymbols;
        private readonly IDictionary<int, IEnumerable<WaveformReadings>> trainingRhythms;
        private readonly IDictionary<int, HiddenMarkovModel> models;

        public WaveformModels(IDictionary<int, IEnumerable<WaveformReadings>> trainingRhythms,
                                    int numberStates,
                                    int numberSymbols)
        {
            if (trainingRhythms == null)
            {
                throw new ArgumentNullException("trainingRhythms");
            }

            if (numberStates == 0)
            {
                throw new ArgumentException("numberStates must be > 0", "numberStates");
            }

            if (numberSymbols == 0)
            {
                throw new ArgumentException("numberSymbols must be > 0", "numberSymbols");
            }

            this.trainingRhythms = trainingRhythms;
            this.kmeans = new KMeans(numberSymbols);
            this.numberStates = numberStates;
            this.numberSymbols = numberSymbols;

            this.models = this.trainingRhythms.ToDictionary(a => a.Key, a => new HiddenMarkovModel(numberStates, numberSymbols));
        }


        public IEnumerable<WaveformReadings> TrainingRhythms(int label)
        {
            return this.trainingRhythms[label];
        }

        public int[] Labels { get { return this.trainingRhythms.Keys.OrderBy(a => a).ToArray(); } }

        /// <summary>
        /// Combines all of the training rhythms into one single sequence by concatenating each 
        /// together.  The reason for this is to build a single sequence that can be used for clustering
        /// purposes.  
        /// </summary>
        /// <returns></returns>
        private IEnumerable<WaveformReadings> Combine()
        {
            return this.trainingRhythms.Aggregate(new List<WaveformReadings>(),
                                           (acc, seq) =>
                                           {
                                               acc.AddRange(seq.Value);
                                               return acc;
                                           },
                                           acc => acc);
        }

        public void Learn()
        {
            var encodedTrainingRhythms = ClusterAll(this.kmeans);

            foreach (var encodedRhythm in encodedTrainingRhythms)
            {
                new BaumWelchLearning(this.models[encodedRhythm.Key]).Run(encodedRhythm.Value);
            }
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
        public IDictionary<int, int[]> ClusterAll(KMeans kmeans)
        {
            var combined = this.Combine().AsArray();
            this.maximumWaveformLength = combined.MinimumWaveformLength();
            var trimmed = combined.Trim(this.maximumWaveformLength);

            var codebook = kmeans.Compute(trimmed);

            return this.trainingRhythms.Select(a => new { Label = a.Key, EncodedSequence = Encode(a.Value) })
                                       .ToDictionary(a => a.Label, a => a.EncodedSequence);

        }


        /// <summary>
        /// Given a sequence of rhythms, predicts the class that the sequence best fits into. Returns
        /// 1 if the sequence are normal rhythms, 2 if they are arrhythmias.  The waveform sequence
        /// is a collection of WaveformReadings instances where each instance in the collection is itself
        /// a collection of WaveformReading instances.  In other words, we have a sequence of sequences.  
        /// Prediction requires two steps:
        /// - Each instance in the collection has to be quantized to a discrete representation
        /// - Now the sequence of quantizations can be used for prediction
        /// </summary>
        /// <param name="waveformSequence"></param>
        /// <returns></returns>
        public int Predict(IEnumerable<WaveformReadings> waveformSequences)
        {
            var logLikelihoods = ComputeLogLikelihoods(waveformSequences);

            return logLikelihoods.OrderByDescending(a => a.Value).First().Key;
        }

        public double Evaluate(IEnumerable<WaveformReadings> waveformSequences)
        {
            var logLikelihoods = ComputeLogLikelihoods(waveformSequences);

            return logLikelihoods.OrderByDescending(a => a.Value).First().Value;
        }

        private IDictionary<int, double> ComputeLogLikelihoods(IEnumerable<WaveformReadings> waveformSequences)
        {
            var encodedSequence = Encode(waveformSequences);

            var logLikelihoods = this.models.Select(a => new { Label = a.Key, LogLikelihood = a.Value.Evaluate(encodedSequence) });

            return logLikelihoods.ToDictionary(a => a.Label, a => a.LogLikelihood);
        }


        /// <summary>
        /// Given a collection of N WaveformReadings instances, returns an int[] of size N where each position is
        /// the code for the corresponding WaveformReadings.  
        /// </summary>
        /// <param name="readings"></param>
        /// <returns></returns>
        private int[] Encode(IEnumerable<WaveformReadings> waveformSequences)
        {
            var trimmed = AsTrimmedArray(waveformSequences);

            return trimmed.Select(s => Encode(s)).ToArray();
        }


        /// <summary>
        /// Given a collection of WaveformReadings, trims each instance down to this.minimumWaveformLength, then
        /// converts the entire collection of readings to a double[][] representation necessary. 
        /// </summary>
        /// <param name="waveformSequence"></param>
        /// <returns></returns>
        private double[][] AsTrimmedArray(IEnumerable<WaveformReadings> waveformSequence)
        {
            return waveformSequence.Select(a => a.AsArray()).ToArray().Trim(this.maximumWaveformLength);
        }


        /// <summary>
        /// Given an array that represents ECG signal data, maps it to its code representation using
        /// this.kmeans to find its centroid.  Assumes that Learn has already been called so that this.kmeans
        /// contains the clusters.  
        /// </summary>
        /// <param name="readings"></param>
        /// <returns></returns>
        private int Encode(double[] readings)
        {
            return this.kmeans.Clusters.Nearest(readings);
        }
    }
}
