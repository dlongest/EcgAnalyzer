using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    public class WaveformModelBuilder
    {
        private readonly IDictionary<int, IEnumerable<WaveformReadings>> rhythms;
        private int stateCount;
        private int symbolCount;

        public WaveformModelBuilder()
        {
            this.rhythms = new Dictionary<int, IEnumerable<WaveformReadings>>();
        }

        public WaveformModelBuilder AddRhythms(int label,
                                                     IEnumerable<WaveformReadings> waveformSequences)
        {
            if (!rhythms.ContainsKey(label))
            {
                this.rhythms.Add(label, new List<WaveformReadings>());
            }

            this.rhythms[label] = this.rhythms[label].Concat(waveformSequences);

            return this;
        }

        public WaveformModelBuilder WithModelParameters(int states, int symbols)
        {
            this.stateCount = states;
            this.symbolCount = symbols;

            return this;
        }


        public WaveformModels Build()
        {
            if (this.stateCount == 0)
            {
                throw new InvalidOperationException("Unable to create CategorizedWaveforms - please use WithStateCount to set the number of states for the underlying model");
            }

            if (this.symbolCount == 0)
            {
                throw new InvalidOperationException("Unable to create CategorizedWaveforms - please use WithSymbolCount to set the number of symbols for the underlying model");
            }

            return new WaveformModels(this.rhythms, this.stateCount, this.symbolCount);
        }
    }
}