using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
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
