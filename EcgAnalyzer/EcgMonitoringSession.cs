using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    public class EcgMonitoringSession
    {
        private readonly IDictionary<string, EcgLead> leads = new Dictionary<string, EcgLead>();

        private EcgMonitoringSession(IDictionary<string, EcgLead> session)
        {
            this.leads = session;
        }

        public static EcgMonitoringSession Load12LeadSessionFromFile(string csvFile)
        {
            return new EcgMonitoringSession(
                    new EcgLeadFactory().From12Lead(csvFile).ToDictionary(a => a.Name, a => a));
        }

        public IEnumerable<string> LeadNames
        {
            get
            {
                return this.leads.Keys;
            }
        }

        public double[] this[string leadName]
        {
            get
            {
                return this.leads[leadName].AsVector();
            }
        }

        public double[] AsAveragedValues(string leadName, int windowSize)
        {
            var averaged = new List<double>();
            var values = this.leads[leadName].AsVector();

            if (values.Count() % windowSize == 0)
            {
                for (int window = 0; window < values.Count() / windowSize; ++window)
                {
                    var sum = 0.0;
                    for (int i = 0; i < windowSize; ++i)
                    {
                        sum += values[window * windowSize + i];
                    }

                    averaged.Add(sum / windowSize);
                }
            }
            else
            {
                int fullWindows = (int)(values.Count() / windowSize);

                for (int window = 0; window < fullWindows; ++window)
                {
                    var sum = 0.0;
                    for (int i = 0; i < windowSize; ++i)
                    {
                        sum += values[window * windowSize + i];
                    }

                    averaged.Add(sum / windowSize);
                }

                double remainingSum = 0.0;
                for (int i = fullWindows * windowSize + 1; i < values.Count(); ++i)
                {
                    remainingSum += values[i];
                }

                averaged.Add(remainingSum / (values.Count() - (fullWindows * windowSize)));
            }

            return averaged.ToArray();
        }
    }
}
