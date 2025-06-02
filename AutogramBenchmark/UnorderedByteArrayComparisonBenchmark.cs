using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autogram;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class UnorderedByteArrayComparisonBenchmark
    {
        private const int TestDataSize = 1000;
        private const int V = 26;
        private byte[][] testdata = new byte[TestDataSize][];

        public UnorderedByteArrayComparisonBenchmark()
        {
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

        [Benchmark]
        public void UnorderedByteArrayComparer()
        {
            foreach(var arr in testdata)
            {
                foreach (var arr2 in testdata)
                {
                    var matches = ((ReadOnlySpan<byte>)arr.AsSpan()).UnorderedByteSpanEquals(arr2);
                }
            }
        }

        [Benchmark]
        public void UnorderedByteArrayComparer2()
        {
            foreach (var arr in testdata)
            {
                foreach (var arr2 in testdata)
                {
                    var matches = ((ReadOnlySpan<byte>)arr.AsSpan()).UnorderedByteSpanEquals2(arr2);
                }
            }
        }

        [Benchmark]
        public void ByteArraysHaveSameContents()
        {
            foreach (var arr in testdata)
            {
                foreach (var arr2 in testdata)
                {
                    var matches = ((ReadOnlySpan<byte>)arr.AsSpan()).ByteArraysHaveSameContents(arr2);
                }
            }
        }

    }
}
