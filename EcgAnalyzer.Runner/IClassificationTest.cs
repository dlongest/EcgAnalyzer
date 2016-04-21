using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer.Runner
{
    public interface IClassificationTest
    {
        void Run(WaveformModelParameters parameters);
    }
}
