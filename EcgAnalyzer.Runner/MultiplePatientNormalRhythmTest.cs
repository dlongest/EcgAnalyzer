using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer.Runner
{
    public class MultiplePatientNormalRhythmTest : ConsoleClassificationTestBase
    {
        private readonly IEnumerable<string> patientCsvRhythmDirectories;

        public MultiplePatientNormalRhythmTest(IEnumerable<string> patientCsvRhythmDirectories)
        {
            this.patientCsvRhythmDirectories = patientCsvRhythmDirectories;
        }

        public override void Run()
        {
            Console.WriteLine("Running multiple patient classifer that will attempt to predict which patient sourced each normal rhythm sequence...");

            var normalFiles = this.patientCsvRhythmDirectories.Select(dir => System.IO.Directory.EnumerateFiles(dir));

            var normalRhythms = normalFiles.Select(f => WaveformReadings.CreateWaveformReadingsFromPatientFiles(f, tokens => CreateWaveformReadingFromCsvTokens(tokens)));
            
            var builder = new WaveformModelBuilder();

            // For each label, we're adding a training sequence of 10 rhythms to the model builder.  This 10 is 
            // arbitrary and we should vary that number as part of our results generation to see what impact it has.
            foreach (var label in Enumerable.Range(0, this.patientCsvRhythmDirectories.Count()))
            {
                builder.AddRhythms(label, normalRhythms.ElementAt(label).Take(10));
            }

            // This classifier is using 5 states and 5 symbols.  We need to vary these as part of our results analysis.
            var classifier = builder.WithModelParameters(5, 5).Build();

            classifier.Learn();

            foreach (var label in Enumerable.Range(0, this.patientCsvRhythmDirectories.Count()))
            {
                var predicted = classifier.Predict(normalRhythms.ElementAt(label).Skip(10).Take(3));

                WriteExpectedVsPredicted(label, predicted);
            }
            WriteSpacer();
        }
    }
}
