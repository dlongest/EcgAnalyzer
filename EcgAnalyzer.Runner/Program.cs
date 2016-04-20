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
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu11"
                                          };
            var arrhythmiaDirs = new string[] {
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu01",
                                            @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu11"
                                          };

            var normalFiles = normalDirs.Select(dir => System.IO.Directory.EnumerateFiles(dir));
            var arrFiles = arrhythmiaDirs.Select(dir => System.IO.Directory.EnumerateFiles(dir));

            var normalRhythms = normalFiles.Select(f => WaveformReadings.CreateWaveformReadingsFromPatientFiles(f, tokens => CreateWaveformReadingFromCsvTokens(tokens)));
            var arrRhythms = arrFiles.Select(f => WaveformReadings.CreateWaveformReadingsFromPatientFiles(f, tokens => CreateWaveformReadingFromCsvTokens(tokens)));


            var cw = new CategorizedWaveformBuilder().AddRhythms(1, normalRhythms.First().Take(10))
                                                     .AddRhythms(2, arrRhythms.First().Take(10))
                                                     .WithModelParameters(5, 5)
                                                     .Build();

            cw.Learn();
            var expectClass1 = cw.Predict(normalRhythms.First().Skip(10).Take(3));
            var expectClass2 = cw.Predict(arrRhythms.First().Skip(10).Take(3));


            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
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
