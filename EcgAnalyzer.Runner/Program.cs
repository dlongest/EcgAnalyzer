using System;
using System.Collections.Generic;
using System.Linq;

namespace EcgAnalyzer.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var normalDirs = new string[] {
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu01",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu11",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu12",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu17",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu24"
                                          };
            var arrhythmiaDirs = new string[] {
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu01",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu11",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu12",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu17",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu24"
                                          };

            RunSinglePatientClassification(normalDirs, arrhythmiaDirs);
            RunMultiplePatientNormalClassification(normalDirs);


            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void RunSinglePatientClassification(IEnumerable<string> normalDirs, IEnumerable<string> arrhythmiaDirs)
        {
            Console.WriteLine("Running classifier for a single patient's normal rhythms vs. arrhythmias...");

            var normalFiles = normalDirs.Select(dir => System.IO.Directory.EnumerateFiles(dir));
            var arrFiles = arrhythmiaDirs.Select(dir => System.IO.Directory.EnumerateFiles(dir));

            var normalRhythms = normalFiles.Select(f => WaveformReadings.CreateWaveformReadingsFromPatientFiles(f, tokens => CreateWaveformReadingFromCsvTokens(tokens)));
            var arrRhythms = arrFiles.Select(f => WaveformReadings.CreateWaveformReadingsFromPatientFiles(f, tokens => CreateWaveformReadingFromCsvTokens(tokens)));

            // In the below code, we're generating a model with 5 states and 5 symbols and using a training sequence
            // of 10 rhythms.  We need to vary all of those for results generation.  
            var classifier = new WaveformModelBuilder().AddRhythms(1, normalRhythms.First().Take(10))
                                                       .AddRhythms(2, arrRhythms.First().Take(10))
                                                       .WithModelParameters(5, 5)
                                                       .Build();

            classifier.Learn();

            WriteExpectedVsPredicted(1, classifier.Predict(normalRhythms.First().Skip(10).Take(3)));
            WriteExpectedVsPredicted(2, classifier.Predict(arrRhythms.First().Skip(10).Take(3)));
            Console.WriteLine("-----------------------------------\n");
        }

        private static void RunMultiplePatientNormalClassification(IEnumerable<string> normalCsvDirs)
        {
            Console.WriteLine("Running multiple patient classifer that will attempt to predict which patient sourced each normal rhythm sequence...");

            var normalFiles = normalCsvDirs.Select(dir => System.IO.Directory.EnumerateFiles(dir));          

            var normalRhythms = normalFiles.Select(f => WaveformReadings.CreateWaveformReadingsFromPatientFiles(f, tokens => CreateWaveformReadingFromCsvTokens(tokens)));


            var builder = new WaveformModelBuilder();
            
            // For each label, we're adding a training sequence of 10 rhythms to the model builder.  This 10 is 
            // arbitrary and we should vary that number as part of our results generation to see what impact it has.
            foreach (var label in Enumerable.Range(0, normalCsvDirs.Count()))
            {
                builder.AddRhythms(label, normalRhythms.ElementAt(label).Take(10));
            }
            
            // This classifier is using 5 states and 5 symbols.  We need to vary these as part of our results analysis.
            var classifier = builder.WithModelParameters(5, 5).Build();

            classifier.Learn();

            foreach (var label in Enumerable.Range(0, normalCsvDirs.Count()))
            {
                var predicted = classifier.Predict(normalRhythms.ElementAt(label).Skip(10).Take(3));

                WriteExpectedVsPredicted(label, predicted);
            }
            Console.WriteLine("-----------------------------------\n");
        }

        private static void WriteExpectedVsPredicted(int expected, int predicted)
        {
            Console.WriteLine("Expected = {0} || Predicted = {1}", expected, predicted);
        }

        private static string ExtractPatientNameFromDirectory(string directory)
        {
            var lastSlashIndex = directory.LastIndexOf('\\');

            return directory.Substring(lastSlashIndex + 1);
        }

        private static WaveformReading CreateWaveformReadingFromCsvTokens(string[] tokens)
        {
            var elapsedTime = TimeSpan.Parse(string.Format("0:{0}", tokens[0].Replace("'", "")));
            var millivolts = Double.Parse(tokens[1]);

            return new WaveformReading(elapsedTime, millivolts);
        }
    }
}
