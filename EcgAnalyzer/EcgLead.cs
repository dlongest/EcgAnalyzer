using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    /// <summary>
    /// An EcgLead corresponds to one specific lead (or input) within the ECG system.  An ECG can use as few as
    /// 1 lead or as many as 15 depending on the circumstance.  Each lead takes a reading that should tell the
    /// same story, but they tell it in different ways due to the physical placement of the lead.  A lead is 
    /// really just a sequence of WaveformReadings.  
    /// </summary>
    public class EcgLead : IEnumerable<IEnumerable<WaveformReading>>
    {
        private readonly IList<List<WaveformReading>> readings;
        private List<WaveformReading> current;

        public EcgLead(string name)
        {
            this.Name = name;
            this.readings = new List<List<WaveformReading>>();
        }
              
        public void BeginNewWaveform()
        {
            this.current = new List<WaveformReading>();
            this.readings.Add(this.current);
        }

        public void Add(TimeSpan elapsedTime, double millivolts)
        {
            if (current == null)
                throw new InvalidOperationException("Unable to add waveform reading - you need to call BeginNewWaveform to start a new waveform before calling Add.");

            this.current.Add(new WaveformReading(elapsedTime, millivolts));
        }      

        public string Name { get; private set; }

        public IEnumerator<IEnumerable<WaveformReading>> GetEnumerator()
        {
            return this.readings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerable<double[]> Waveforms
        {
            get
            {
                return this.readings.Select(a => a.OrderBy(b => b.ElapsedTime).Select(b => b.Millivolts).ToArray());
            }
        }
    }    
}
