using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var allSeries = new CsvFileBasesdLoadLabeledWaveformReadings(@"C:\Users\dcl\Documents\GitHub\EcgAnalyzer\Data\Normal\cu01",
                                                                     tokens => new WaveformReading(TimeSpan.Parse(string.Format("0:{0}", tokens[0].Replace("'", ""))),
                                                                                                   Double.Parse(tokens[1])),
                                                                     1).Load().ToArray();
           
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}
