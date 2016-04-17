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
    public class EcgDataSet
    {
        private readonly KMeans kmeans = new KMeans(3);

        public void Learn(string directory)
        {
            var normalFiles = Directory.EnumerateFiles(directory).Where(a => a.EndsWith("txt") && !a.Contains("_"));
            var vfFiles = Directory.EnumerateFiles(directory).Where(a => a.EndsWith("txt") && a.Contains("_"));

            var allData = ReadAll(normalFiles, vfFiles);

            var codebook = kmeans.Compute(allData);

            var hmmNormal = new HiddenMarkovModel(2, 3);
            new BaumWelchLearning(hmmNormal).Run(codebook.Take(10).ToArray());

            var p = hmmNormal.Evaluate(codebook.Skip(10).Take(3).ToArray());
            var p2 = hmmNormal.Evaluate(codebook.Skip(16).Take(3).ToArray());


            var hmmVf = new HiddenMarkovModel(2, 3);
            new BaumWelchLearning(hmmVf).Run(codebook.Skip(16).Take(10).ToArray());

            var p3 = hmmVf.Evaluate(codebook.Skip(3).Take(3).ToArray());
            var p4 = hmmVf.Evaluate(codebook.Skip(15).Take(3).ToArray());
        }

        private double[][] ReadAll(IEnumerable<string> firstSet, IEnumerable<string> secondSet)
        {
            var first = Read(firstSet);
            var second = Read(secondSet);

            var allData = Concatenate(first, second);

            var minimum = allData.Min(a => a.Count());

            return allData.Select(a => a.Take(minimum).ToArray()).ToArray();
        }

        private double[][] Read(IEnumerable<string> csvFiles)
        {
            return csvFiles.Select(a => Read(a)).ToArray();
        }

        private double[][] Concatenate(double[][] first, double[][] second)
        {
            return first.Concat(second).ToArray();
        }

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

        private double[][] FeatureVectors(string controlDirectory, string infarctionDirectory, int windowSize)
        {
            var controlSessions = Load(controlDirectory).ToArray();

            var infarctionSessions = Load(infarctionDirectory).ToArray();

            var combinedSessions = controlSessions.Concat(infarctionSessions);

            return combinedSessions.Select(a => a.AsAveragedValues("v1", windowSize).FirstHalf().FirstHalf().FirstHalf()).ToArray();
        }

        private IEnumerable<EcgMonitoringSession> Load(string directory)
        {
            var files = CsvFiles(directory);

            return files.Select(f => EcgMonitoringSession.Load12LeadSessionFromFile(f));
        }

        private IEnumerable<string> CsvFiles(string directory)
        {
            return System.IO.Directory.EnumerateFiles(directory).Where(a => a.EndsWith("csv"));
        }
    }
}
