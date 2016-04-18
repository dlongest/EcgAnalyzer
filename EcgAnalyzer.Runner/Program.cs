using System;
using System.Linq;

namespace EcgAnalyzer.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var cu01Waveforms = PatientWaveformFactory.LoadSeriesForPatient(@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu01",
                                                                        @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu01",
                                                                        "cu01",
                                                                        tokens => CreateWaveformReadingFromCsvTokens(tokens));

            var cu11Waveforms = PatientWaveformFactory.LoadSeriesForPatient(@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu11",
                                                                        @"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Arrhythmia\cu11",
                                                                        "cu11",
                                                                        tokens => CreateWaveformReadingFromCsvTokens(tokens));


            new WaveformClassifier().Learn(cu01Waveforms);

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
