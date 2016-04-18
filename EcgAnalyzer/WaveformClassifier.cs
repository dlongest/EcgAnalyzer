using Accord.MachineLearning;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcgAnalyzer.Extensions;

namespace EcgAnalyzer
{
    public class WaveformClassifier
    {
        private readonly KMeans kmeans = new KMeans(5);

        /// <summary>
        /// Learn trains multiple Hidden Markov Models using the provided csvFiles as input.  Each top-level collection is intended
        /// to represent a different label and each subsequent collection advances the label identifier by 1.  Once Learn completes
        /// there will be N hidden Markov models, one per label.  
        /// </summary>
        /// <param name="csvFiles"></param>
        public void Learn(PatientWaveforms patientWaveforms)
        {
            var encoded = patientWaveforms.ClusterAll(this.kmeans);

            var normalSequence = encoded.Normal.Take(10);
            var arrhythmiaSequence = encoded.Arrhythmias.Take(10);
            
            var hmmNormal = new HiddenMarkovModel(5, 5);
            new BaumWelchLearning(hmmNormal).Run(normalSequence.ToArray());

            var p = hmmNormal.Evaluate(encoded.Normal.Skip(10).Take(3).ToArray());
            var p2 = hmmNormal.Evaluate(encoded.Arrhythmias.Skip(10).Take(3).ToArray());

            Console.WriteLine("P(normal sequence) = " + p);
            Console.WriteLine("P(arrhythmia) = " + p2);

            var hmmArrhythmia = new HiddenMarkovModel(5, 5);
            new BaumWelchLearning(hmmArrhythmia).Run(arrhythmiaSequence.ToArray());

            var p3 = hmmArrhythmia.Evaluate(encoded.Normal.Skip(10).Take(3).ToArray());
            var p4 = hmmArrhythmia.Evaluate(encoded.Arrhythmias.Skip(10).Take(3).ToArray());

            Console.WriteLine("\nP(normal sequence) = " + p3);
            Console.WriteLine("P(arrhythmia) = " + p4);
        }


        private int[] Encode(IEnumerable<WaveformReadings> readings)
        {
            return readings.Select(r => this.kmeans.Clusters.Nearest(r.AsArray())).ToArray();
        }


        /// <summary>
        /// IGNORE THIS METHOD - IT WAS THE INITIAL SAMPLE METHOD.  IT WILL BE LEFT TEMPORARILY AS A GUIDE
        /// BUT ALL ACTIVE WORK IS BASED ON THE OVERLOAD OF LEARN WITH PATIENTWAVEFORMS AS PARAMETERS
        /// </summary>
        /// <param name="directory"></param>
        public void Learn(string directory)
        {
            // Find the filenames representing normal files and arrhythmia files. 
            var normalFiles = Directory.EnumerateFiles(directory).Where(a => a.EndsWith("txt") && !a.Contains("_"));
            var vfFiles = Directory.EnumerateFiles(directory).Where(a => a.EndsWith("txt") && a.Contains("_"));

            // Get all the data in a combined double[][]
            var allData = ReadAll(normalFiles, vfFiles);

            // Compute the K-means codebook from all of the signal data. If there are N different signals represented
            // by allData (i.e. N arrays of some length M), then the codebook will be length N and contains a symbol
            // that places the signal (treated as a vector) into the appropriate centroid (coded by the codebook). 
            var codebook = kmeans.Compute(allData);

            // Now train an HMM to represent normal rhythms.  The magic number 10 corresponds to knowing that there 
            // are at least 10 files in the "normal" set and thus the first 10 symbols in the codebook will 
            // correspond to normal signals.  
            var hmmNormal = new HiddenMarkovModel(2, 3);
            new BaumWelchLearning(hmmNormal).Run(codebook.Take(10).ToArray());

            // Evaluate probability of the 11th through 13th signals occurring.  Again, it is implicit that we have
            // at least 13 normal files so these 3 will all be normal so we expect this probability to be relatively strong.
            var p = hmmNormal.Evaluate(codebook.Skip(10).Take(3).ToArray());

            // Evaluate the probability of 3 non-normal signals occuring.  This probability should be much lower than p. 
            var p2 = hmmNormal.Evaluate(codebook.Skip(16).Take(3).ToArray());

            // Repeat the training steps for arrhythmias and evaluate the probabilities for both types of rhythms. 
            var hmmVf = new HiddenMarkovModel(2, 3);
            new BaumWelchLearning(hmmVf).Run(codebook.Skip(16).Take(10).ToArray());

            var p3 = hmmVf.Evaluate(codebook.Skip(3).Take(3).ToArray());
            var p4 = hmmVf.Evaluate(codebook.Skip(15).Take(3).ToArray());
        }

        /// <summary>
        /// firstSet and secondSet are assumed to each contain an arbitrary (and possibly different) number of
        /// CSV filenames containing ECG data.  Both are read into memory, then concatenated together, trimmed
        /// such that all entries have the same number of signals.  
        /// </summary>
        /// <param name="firstSet"></param>
        /// <param name="secondSet"></param>
        /// <returns></returns>
        private double[][] ReadAll(IEnumerable<string> firstSet, IEnumerable<string> secondSet)
        {
            var first = Read(firstSet);
            var second = Read(secondSet);

            var allData = Concatenate(first, second);

            var minimum = allData.Min(a => a.Count());

            return allData.Select(a => a.Take(minimum).ToArray()).ToArray();
        }

        /// <summary>
        /// For all of the absolute CSV filenames, reads the signals from each file and returns them, one file
        /// per record in the result (i.e. double[0] contains all the signals from the first file, etc). 
        /// </summary>
        /// <param name="csvFiles"></param>
        /// <returns></returns>
        private double[][] Read(IEnumerable<string> csvFiles)
        {
            return csvFiles.Select(a => Read(a)).ToArray();
        }

        /// <summary>
        /// Returns a double[][] that is the result of concatenating second onto first. 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private double[][] Concatenate(double[][] first, double[][] second)
        {
            return first.Concat(second).ToArray();
        }

        /// <summary>
        /// From csvFilename, loads all the waveform signals in order and returns them.  The CSV file is assumed
        /// to have the waveform signal in the second column of the file.  The first column is ignored.
        /// </summary>
        /// <param name="csvFilename"></param>
        /// <param name="hasHeaderLine"></param>
        /// <returns></returns>
        private double[] Read(string csvFilename, bool hasHeaderLine = false)
        {
            var data = new List<double>();

            using (var reader = new StreamReader(csvFilename))
            {
                if (hasHeaderLine) reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var tokens = line.Split(new string[] { "," }, StringSplitOptions.None);

                    data.Add(Double.Parse(tokens[1]));
                }
            }

            return data.ToArray();
        }

        private IEnumerable<string> CsvFiles(string directory)
        {
            return System.IO.Directory.EnumerateFiles(directory).Where(a => a.EndsWith("csv"));
        }
    }
}
