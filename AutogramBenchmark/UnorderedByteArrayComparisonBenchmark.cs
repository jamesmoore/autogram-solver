using Autogram;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class UnorderedByteArrayComparisonBenchmark
    {
        private const int TestDataSize = 1000;
        private const int V = 26;
        private byte[][] testdata = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            testdata = new byte[TestDataSize][];
            var random = new Random(1);

            for (int i = 0; i < TestDataSize; i++)
            {
                var arr = new byte[V];
                for(int j = 0; j < V; j++)
                {
                    arr[j] = (byte)random.Next(256);
                }
                testdata[i] = arr;
            }
        }

        [Benchmark(OperationsPerInvoke = TestDataSize * TestDataSize)]
        public int UnorderedByteArrayComparer()
        {
            var matches = 0;

            foreach(var arr in testdata)
            {
                foreach (var arr2 in testdata)
                {
                    if (((ReadOnlySpan<byte>)arr.AsSpan()).UnorderedByteSpanEquals(arr2))
                    {
                        matches++;
                    }
                }
            }

            return matches;
        }

        [Benchmark(OperationsPerInvoke = TestDataSize * TestDataSize)]
        public int UnorderedByteArrayComparerWithSum()
        {
            var matches = 0;

            foreach (var arr in testdata)
            {
                foreach (var arr2 in testdata)
                {
                    if (((ReadOnlySpan<byte>)arr.AsSpan()).UnorderedByteSpanEqualsWithSum(arr2))
                    {
                        matches++;
                    }
                }
            }

            return matches;
        }

        [Benchmark(OperationsPerInvoke = TestDataSize * TestDataSize)]
        public int UnorderedByteArrayComparer2()
        {
            var matches = 0;

            foreach (var arr in testdata)
            {
                foreach (var arr2 in testdata)
                {
                    if (((ReadOnlySpan<byte>)arr.AsSpan()).UnorderedByteSpanEquals2(arr2))
                    {
                        matches++;
                    }
                }
            }

            return matches;
        }

        [Benchmark(OperationsPerInvoke = TestDataSize * TestDataSize)]
        public int ByteArraysHaveSameContents()
        {
            var matches = 0;

            foreach (var arr in testdata)
            {
                foreach (var arr2 in testdata)
                {
                    if (((ReadOnlySpan<byte>)arr.AsSpan()).ByteArraysHaveSameContents(arr2))
                    {
                        matches++;
                    }
                }
            }

            return matches;
        }

    }
}
