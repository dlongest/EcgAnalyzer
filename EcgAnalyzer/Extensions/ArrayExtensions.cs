using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcgAnalyzer.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] FirstHalf<T>(this T[] values)
        {
            var half = (int)(values.Count() / 2);
            var v = new T[half];

            for (int i = 0; i < half; ++i)
            {
                v[i] = values[i];
            }

            return v;
        }

        public static T[] FirstQuarter<T>(this T[] values)
        {
            var half = (int)(values.Count() / 4);
            var v = new T[half];

            for (int i = 0; i < half; ++i)
            {
                v[i] = values[i];
            }

            return v;
        }

        public static T[] FirstHalf<T>(this IEnumerable<T> values)
        {
            return values.ToArray().FirstHalf();
        }

        public static T[] FirstQuarter<T>(this IEnumerable<T> values)
        {
            return values.ToArray().FirstQuarter();
        }
    }

}
