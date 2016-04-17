using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    public class CsvFileBasesdLoadLabeledWaveformReadings : ILoadLabeledWaveformReadings
    {
        private readonly IEnumerable<string> csvFilenames;
        private readonly Func<string[], WaveformReading> parseReading;
        private readonly int label;
        private readonly bool hasHeaderLine;

        public CsvFileBasesdLoadLabeledWaveformReadings(string directory,
                                                        Func<string[], WaveformReading> parseReading,
                                                        int label,
                                                        bool hasHeaderLine = false)
            : this(System.IO.Directory.EnumerateFiles(directory), parseReading, label, hasHeaderLine)
        {
        }

        public CsvFileBasesdLoadLabeledWaveformReadings(IEnumerable<string> csvFilenames,
                                                        Func<string[], WaveformReading> parseReading,
                                                        int label,
                                                        bool hasHeaderLine = false)
        {
            this.csvFilenames = csvFilenames;
            this.parseReading = parseReading;
            this.label = label;
            this.hasHeaderLine = hasHeaderLine;
        }

        /// <summary>
        /// Loads a collection of WaveformSeries.  Each WaveformSeries represents a single file's worth
        /// of signals and all are labeled with the label within this instance.  
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEnumerable<LabeledWaveformReadings>> Load()
        {
            foreach (var filename in this.csvFilenames)
            {
                var series = new WaveformSeries(filename);
                series.BeginNewWaveform(this.label);

                using (var reader = new System.IO.StreamReader(filename))
                {
                    if (this.hasHeaderLine) reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        var tokens = reader.ReadLine().Split(new string[] { "," }, StringSplitOptions.None);

                        var reading = this.parseReading(tokens);

                        series.Add(reading);
                    }
                }

                yield return series;
            }
        }

        public int Label { get { return this.label;  } }
    }
}
