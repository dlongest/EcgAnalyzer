﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcgAnalyzer.Extensions;

namespace EcgAnalyzer.Runner
{
    public class SinglePatientNormalVsArrhythmiaTest : ConsoleClassificationTestBase
    {
        private readonly IEnumerable<string> normalRhythmDirectories;
        private readonly IEnumerable<string> arrhythmiaDirectories;

        public SinglePatientNormalVsArrhythmiaTest(IEnumerable<string> normalRhythmDirectories, IEnumerable<string> arrhythmiaDirectories)
        {
            this.normalRhythmDirectories = normalRhythmDirectories;
            this.arrhythmiaDirectories = arrhythmiaDirectories;
        }

        public override void Run(WaveformModelParameters parameters)
        {

            Console.WriteLine("Single Patient Classifier: Normal Rhythms vs. Arrhythmias");
            Console.WriteLine("States = {0} | Symbols = {1}", parameters.States, parameters.Symbols);

            // For each directory, we expand it into the files contained within the directory
            var normalFiles = this.normalRhythmDirectories.Select(dir => System.IO.Directory.EnumerateFiles(dir));
            var arrFiles = this.arrhythmiaDirectories.Select(dir => System.IO.Directory.EnumerateFiles(dir));

            // Here we're taking all of the files we found in the directories and converting them into 
            // a collection of a collectio of WaveformReadings.  Each file contains the signal data for one set of patient rhythms.
            // In other words, a file == an instance of WaveformReadings.  The directory can have multiple files hence multiple WaveformReadings.
            // Since we have multiple directories, we have multiple collections of WaveformReadings.  
            var normalRhythms = normalFiles.Select(f => WaveformReadings.CreateWaveformReadingsFromPatientFiles(f, tokens => CreateWaveformReadingFromCsvTokens(tokens)));
            var arrRhythms = arrFiles.Select(f => WaveformReadings.CreateWaveformReadingsFromPatientFiles(f, tokens => CreateWaveformReadingFromCsvTokens(tokens)));

            // In the below code, we're generating a model with 5 states and 5 symbols and using a training sequence
            // of 10 rhythms.  We need to vary all of those for results generation.  
            // normalRhythms.First() and arrRhythms.First() select the very first collection of WaveformReadings.  These are presumed to be all
            // the WaveformReadings for a single patient of that rhythm type.  We then select 10 of those to use for training purposes.  The remainder
            // of the readings are then available for validation purposes. 

            foreach (var trainingSequenceLength in parameters.TrainingSequenceLength)
            {
                Console.WriteLine("Training Sequence Length = {0}", trainingSequenceLength);

                var classifier = new WaveformModelBuilder().AddRhythms(1, normalRhythms.First().Take(trainingSequenceLength))
                                                           .AddRhythms(2, arrRhythms.First().Take(trainingSequenceLength))
                                                           .WithModelParameters(parameters.States, parameters.Symbols)
                                                           .Build();

                classifier.Learn();
                foreach (var group in normalRhythms.First().TakeNext(trainingSequenceLength, 3))
                {
                    WriteExpectedVsPredicted(1, classifier.Predict(normalRhythms.First().Skip(trainingSequenceLength).Take(3)));
                    WriteExpectedVsPredicted(2, classifier.Predict(arrRhythms.First().Skip(trainingSequenceLength).Take(3)));
                    WriteSpacer();
                }

                WriteSpacer();
            }
        }
    }

    public class WaveformModelParameters
    {
        private readonly IEnumerable<int> trainingSequenceLengths;

        public WaveformModelParameters()
        {
            this.trainingSequenceLengths = Enumerable.Range(5, 7);
        }

        public WaveformModelParameters(params int[] trainingSequenceLengths)
        {
            this.trainingSequenceLengths = trainingSequenceLengths;
        }
        public int States { get; set; }

        public int Symbols { get; set; }
        
        public IEnumerable<int> TrainingSequenceLength
        {
            get
            {
                return this.trainingSequenceLengths;
            }
        }       
    }
}
