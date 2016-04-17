using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer
{
    public interface ILoadLabeledWaveformReadings
    {
        IEnumerable<IEnumerable<LabeledWaveformReadings>> Load();
    }
}
