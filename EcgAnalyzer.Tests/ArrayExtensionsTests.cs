using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using EcgAnalyzer.Extensions;
using System.Linq.Expressions;

namespace EcgAnalyzer.Tests
{
    public class ArrayExtensionsTests
    {
        [Fact]
        public void Bundle_ProducesDistinctGroups()
        {
            var ar = new int[] { 1, 2, 3, 4, 5, 6 };

            var bundles = ar.Partition(2);

            Assert.Equal(3, bundles.Count());
            Assert.Collection(bundles.First(), a => Assert.Equal(1, a), a => Assert.Equal(2, a));
            Assert.Collection(bundles.ElementAt(1), a => Assert.Equal(3, a), a => Assert.Equal(4, a));
            Assert.Collection(bundles.Last(), a => Assert.Equal(5, a), a => Assert.Equal(6, a));
        }

        [Fact]
        public void Bundle_ReturnsIncompleteFinalCollection_BasedOnParameter()
        {
            var ar = new int[] { 1, 2, 3, 4, 5 };

            var bundles = ar.Partition(2);

            Assert.Equal(3, bundles.Count());
            Assert.Equal(1, bundles.Last().Count());
            Assert.Collection(bundles.First(), a => Assert.Equal(1, a), a => Assert.Equal(2, a));
            Assert.Collection(bundles.ElementAt(1), a => Assert.Equal(3, a), a => Assert.Equal(4, a));
            Assert.Collection(bundles.Last(), a => Assert.Equal(5, a));
        }

        [Fact]
        public void Bundle_DoesNotReturnIncompleteFinalCollection_BasedOnParameter()
        {
            var ar = new int[] { 1, 2, 3, 4, 5 };

            var bundles = ar.Partition(2, lastPartitionCanBeIncomplete: false);

            Assert.Equal(2, bundles.Count());          
            Assert.Collection(bundles.First(), a => Assert.Equal(1, a), a => Assert.Equal(2, a));
            Assert.Collection(bundles.ElementAt(1), a => Assert.Equal(3, a), a => Assert.Equal(4, a));
        }

        [Fact]
        public void Bundle_OverlapsCollections()
        {
            var ar = new int[] { 1, 2, 3, 4, 5 };

            var bundles = ar.OverlappedPartition(3, 2).ToArray();

            Assert.Equal(3, bundles.Count());
            Assert.Collection(bundles.First(), a => Assert.Equal(1, a), a => Assert.Equal(2, a), a => Assert.Equal(3, a));
            Assert.Collection(bundles.ElementAt(1), a => Assert.Equal(2, a), a => Assert.Equal(3, a), a => Assert.Equal(4, a));
            Assert.Collection(bundles.Last(), a => Assert.Equal(3, a), a => Assert.Equal(4, a), a => Assert.Equal(5, a));
        }

        [Fact]
        public void TakeNext_ReturnsCorrectNumberOfGroups()
        {
            var ar = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var groups = ar.TakeNext(4, 3);

            Assert.Equal(3, groups.Count());          
        }

        [Fact]
        public void TakeNext_ReturnsCorrectGroupsOfRecords()
        {
            var ar = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var groups = ar.TakeNext(4, 3);

            Assert.Collection(groups.First(), a => Assert.Equal(5, a), a => Assert.Equal(6, a), a => Assert.Equal(7, a));
            Assert.Collection(groups.ElementAt(1), a => Assert.Equal(6, a), a => Assert.Equal(7, a), a => Assert.Equal(8, a));
            Assert.Collection(groups.Last(), a => Assert.Equal(7, a), a => Assert.Equal(8, a), a => Assert.Equal(9, a));
        }
    }
}
