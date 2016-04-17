using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    /// <summary>
    /// A WaveformReading represents one impulse reading in millivolts at a particular time.  The time is elapsed
    /// within the construct of some larger session.  The absolute time is not necessary as the readings only have
    /// meaning in relation to each other.  For that reason, TimeSpan is used to standard the readings within
    /// sessions, which is typical of ECG signal analysis. 
    /// </summary>
    public class WaveformReading
    {
        public WaveformReading(TimeSpan elapsedTime, double millivolts)
        {
            this.ElapsedTime = elapsedTime;
            this.Millivolts = millivolts;
        }

        public TimeSpan ElapsedTime { get; private set; }

        public double Millivolts { get; private set; }
    }
}
