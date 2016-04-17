using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{    
    /// <summary>
    /// A WaveformSeries represents some number of waveform readings.  Since LabeledWaveformReadings 
    /// is itself a collection of WaveformReadings, the relationship is: 
    /// WaveformSeries --> LabeledWaveformReadings[] --> WaveformReading[].
    /// A given series may contain waveforms with different labels and that's supported. 
    /// </summary>
    public class WaveformSeries : IEnumerable<LabeledWaveformReadings>
    {
        private readonly IList<LabeledWaveformReadings> readings;
        private LabeledWaveformReadings current;

        public WaveformSeries():this(Guid.NewGuid().ToString())
        {
        }

        public WaveformSeries(string name)
        {
            this.Name = name;
            this.readings = new List<LabeledWaveformReadings>();
        }
              
        public void BeginNewWaveform(int label)
        {
            this.current = new LabeledWaveformReadings(label);
            this.readings.Add(this.current);
        }

        public void Add(WaveformReading reading)
        {
            if (current == null)
                throw new InvalidOperationException("Unable to add waveform reading - you need to call BeginNewWaveform to start a new waveform before calling Add.");

            this.current.Add(reading);
        }

        public void Add(TimeSpan elapsedTime, double millivolts)
        {
            Add(new WaveformReading(elapsedTime, millivolts));
        }      

        public string Name { get; private set; }

        IEnumerator<LabeledWaveformReadings> IEnumerable<LabeledWaveformReadings>.GetEnumerator()
        {
            return this.readings.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerable<LabeledWaveformReadings> this[int label]
        {
            get
            {
                return this.readings.Where(a => a.Label == label);
            }
        }

        public IDictionary<int, IEnumerable<LabeledWaveformReadings>> ByLabel
        {
            get
            {
                return this.readings.GroupBy(a => a.Label, a => a)
                           .ToDictionary(a => a.Key, a => a.AsEnumerable());
            }
        }
    }  
}
