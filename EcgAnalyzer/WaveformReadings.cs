using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    /// <summary>
    /// LabeledWaveformReadings represents a collection of WaveformReadings with a label attached.  The readings
    /// are presumed to be contiguous and the entire collection of readings is indicative of some larger construct
    /// that is categorized under label.  There are lots of ways to categorize signals as both waves (waveform parts
    /// like P, R, and Q orcomplex waveforms like PR, QRS, and ST) as well as in more diagnostic terms such as
    /// normal, ventricular tachycardia, atrial fibrillation, etc.  To cover the wide uses that readings can be 
    /// categorized under, LabeledWaveformReadings uses an integer label and it's up to the client to decide
    /// what the meaning of the label means within an application context. If the readings being loaded are
    /// unknown with respect to their label or classification (i.e. no annotations are available), then simply use
    /// the same label for every waveform reading and it will treat them like one block of readings. 
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

        public double[] AsArray()
        {
            return this.readings.OrderBy(a => a.ElapsedTime).Select(a => a.Millivolts).ToArray();
        }
    }
}
