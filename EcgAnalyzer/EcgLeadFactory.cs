using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    public class EcgLeadFactory
    {
        public static string[] Lead12Names = new[] { "i", "ii", "iii", "avr", "avl", "avf", "v1", "v2", "v3", "v4", "v5", "v6", "vx", "vy", "vz" };

        public IEnumerable<EcgLead> From12Lead(string csvFilename, bool hasHeaderLine = false)
        {
            var leads = new Dictionary<string, EcgLead>();

            Lead12Names.ToList().ForEach(a => leads.Add(a, new EcgLead(a)));

            using (var reader = new StreamReader(csvFilename))
            {
                if (hasHeaderLine) reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var tokens = line.Split(new string[] { "," }, StringSplitOptions.None);

                    // Have to prepend 0: to the time span since it doesn't include hours, but TimeSpan 
                    // requires it to appear. 
                    var time = TimeSpan.Parse(string.Format("0:{0}", tokens[0].Replace("'", string.Empty)));

                    tokens.Skip(1).Zip(Lead12Names, (mv, name) => new { LeadName = name, Millivolts = Double.Parse(mv) })
                                  .ToList()
                                  .ForEach(a => leads[a.LeadName].Add(time, a.Millivolts));
                }
            }

            return leads.Select(a => a.Value);
        }
    }

}
