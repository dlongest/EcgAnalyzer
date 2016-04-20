using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    /// <summary>
    /// WaveformReadings are presumed to be contiguous and the entire collection of readings is indicative of some larger 
    /// construct or rhythm pattern. 
    /// </summary>
    public class WaveformReadings : IEnumerable<WaveformReading>
    {
        private readonly IList<WaveformReading> readings;

        public WaveformReadings()
        {
            this.readings = new List<WaveformReading>();
        }

        public void Add(WaveformReading reading)
        {
            this.readings.Add(reading);
        }

        public IEnumerator<WaveformReading> GetEnumerator()
        {
            return this.readings.OrderBy(a => a.ElapsedTime).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns the millivolt signals for the contained readings in order of their occurrence
        /// in real life (i.e. ordered by ElapsedTime of the readings). 
        /// </summary>
        /// <returns></returns>
        public double[] AsArray()
        {
            return this.readings.OrderBy(a => a.ElapsedTime).Select(a => a.Millivolts).ToArray();
        }

        public static IEnumerable<WaveformReadings> CreateWaveformReadingsFromPatientFiles(IEnumerable<string> csvFiles,
                                                                                           Func<string[], WaveformReading> createReadingFromCsvTokens)
        {
            return csvFiles.Select(file => CreateReadingsFromSingleFile(file, createReadingFromCsvTokens));
        }

        public static WaveformReadings CreateReadingsFromSingleFile(string csvFile,
                                                                     Func<string[], WaveformReading> createReadingFromCsvTokens)
        {
            var readings = new WaveformReadings();

            using (var reader = new System.IO.StreamReader(csvFile))
            {
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