using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    public static class PatientWaveformFactory
    {

       public static CategorizedWaveforms LoadSeriesForPatient(string patientNormalRhythmDirectory, 
                                                           string patientArrhythmiaDirectory,                                                         
                                                           Func<string[], WaveformReading> createReadingFromCsvTokens,
                                                           bool hasHeaderLine = false)
        {
            var normal = Directory.EnumerateFiles(patientNormalRhythmDirectory)
                                  .Select(file => CreateReadingsFromSingleFile(file, createReadingFromCsvTokens, hasHeaderLine));

            var arrhythmias = Directory.EnumerateFiles(patientArrhythmiaDirectory)
                                  .Select(file => CreateReadingsFromSingleFile(file, createReadingFromCsvTokens, hasHeaderLine));

            return new CategorizedWaveforms(normal, arrhythmias);
        }

        public static CategorizedWaveformCollection LoadSeriesForMultiplePatients(IEnumerable<string> normalRhythmDirectories, 
                                                                                      IEnumerable<string> arrhythmiaDirectories,
                                                                                      Func<string[], WaveformReading> createReadingFromCsvTokens,
                                                                                      bool hasHeaderLine = false)
        {
            if (normalRhythmDirectories.Count() != arrhythmiaDirectories.Count())
                throw new ArgumentException("Number of normal directories must match the number of arrhythmia directories");

            return new CategorizedWaveformCollection(normalRhythmDirectories.Zip(arrhythmiaDirectories, (n, a) => LoadSeriesForPatient(n, a, createReadingFromCsvTokens, hasHeaderLine)));
        }

        private static IEnumerable<WaveformReadings> CreateWaveformReadingsFromPatientFiles(IEnumerable<string> csvFiles,
                                                                                            Func<string[], WaveformReading> createReadingFromCsvTokens,
                                                                                            bool hasHeaderLine)
        {
            return csvFiles.Select(file => CreateReadingsFromSingleFile(file, createReadingFromCsvTokens, hasHeaderLine));
        }

        private static WaveformReadings CreateReadingsFromSingleFile(string csvFile,
                                                                     Func<string[], WaveformReading> createReadingFromCsvTokens,
                                                                     bool hasHeaderLine)
        {
            var readings = new WaveformReadings();

            using (var reader = new System.IO.StreamReader(csvFile))
            {
                if (hasHeaderLine) reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var reading = createReadingFromCsvTokens(reader.ReadLine().Split(new string[] { "," }, StringSplitOptions.None));

                    readings.Add(reading);
                }
            }

            return readings;
        }
    }   
}
