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

        public static double[][] AsArray(this IEnumerable<WaveformReadings> readings)
        {
            return readings.Select(a => a.Select(b => b.Millivolts).ToArray()).ToArray();
        }

        public static double[][] Trim(this double[][] waveforms)
        {
            var minimum = waveforms.MinimumWaveformLength();

            return waveforms.Trim(minimum);
        }

        public static double[][] Trim(this double[][] waveforms, int length)
        {
            var r = new double[waveforms.Length][];

            foreach (var index in Enumerable.Range(0, waveforms.Length))
            {
                r[index] = new double[length];

                Array.Copy(waveforms[index], r[index], length);
            }

            return r;
        }

        /// <summary>
        /// Returns the length of the shortest waveform.  
        /// </summary>
        /// <param name="waveforms"></param>
        /// <returns></returns>
        public static int MinimumWaveformLength(this double[][] waveforms)
        {
            return waveforms.Select(a => a.Length).Min();
        }

        /// <summary>
        /// Given a source sequence of length N, breaks it up into at minimum M collections where M = N / bundleSize.  The M+1 bundle
        /// can be created as an incomplete (i.e. size less than bundleSize) if the parameter is set to true, otherwise the remaining
        /// items in source are not included in any bundle.  All bundles are distinct (i.e. if the bundleSize is 2, then the first
        /// returned collection is items 1 and 2 from source, second returned collection is items 3 and 4, etc. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="partitionSize"></param>
        /// <param name="lastPartitionCanBeIncomplete"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source,
                                                            int partitionSize,
                                                            bool lastPartitionCanBeIncomplete = true)
        {

            var numberCompletePartitions = source.Count() / partitionSize;

            foreach (var partition in Enumerable.Range(0, numberCompletePartitions))
            {
                yield return source.Skip(partition * partitionSize).Take(partitionSize);
            }

            if (lastPartitionCanBeIncomplete && numberCompletePartitions * partitionSize < source.Count())
            {
                yield return source.Skip(numberCompletePartitions * partitionSize);
            }
        }

        public static IEnumerable<IEnumerable<T>> OverlappedPartition<T>(this IEnumerable<T> source,
                                                                        int partitionSize,
                                                                        int overlap)
        {
            if (partitionSize <= overlap)
                throw new ArgumentException("partitionSize must be greater than overlap");

           // int numberPartitions = (source.Count() / overlap) + 1;

            int startIndex = 0;
            int step = partitionSize - overlap;
            
            while (startIndex + partitionSize <= source.Count())
            {
                var s = source.Subset(startIndex, partitionSize);
                yield return s;
                startIndex += step;
            }
        }

        public static IEnumerable<T> Subset<T>(this IEnumerable<T> source, int startIndex, int count)
        {
            var ar = source.ToArray();

            return Enumerable.Range(startIndex, count).Select(a => ar[a]);
        }

        public static IEnumerable<IEnumerable<T>> TakeNext<T>(this IEnumerable<T> source, int skip, int perGroup)
        {
            var remainder = source.Skip(skip);

            var nextGroup = remainder.Take(perGroup);

            while (nextGroup.Count() == perGroup)
            {
                yield return nextGroup;
                remainder = remainder.Skip(1);
                nextGroup = remainder.Take(perGroup);
            }
        }
    }
}
