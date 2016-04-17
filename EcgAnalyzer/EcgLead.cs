using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    public class EcgLead
    {
        private readonly IList<Reading> readings = new List<Reading>();

        public EcgLead(string name)
        {
            this.Name = name;
        }

        public void Add(TimeSpan elapsedTime, double millivolts)
        {
            this.readings.Add(new Reading(elapsedTime, millivolts));
        }

        public string Name { get; private set; }

        public double[] AsVector()
        {
            return this.readings.Select(a => a.Millivolts).ToArray();
        }

        private class Reading
        {
            public Reading(TimeSpan elapsedTime, double millivolts)
            {
                this.ElapsedTime = elapsedTime;
                this.Millivolts = millivolts;
            }

            public TimeSpan ElapsedTime { get; private set; }

            public double Millivolts { get; private set; }
        }
    }

}
