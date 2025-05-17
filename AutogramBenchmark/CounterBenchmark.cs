using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class CounterBenchmark
    {
        const int AlphabetSize = 26;
        int? seed = 2001;

        private readonly Autogram.AutogramBytes autogram;
        private readonly string teststring;

        public CounterBenchmark()
        {
            var alphabet = Enumerable.Range(0, AlphabetSize).Select(p => (char)('a' + p)).ToList();
            autogram = new Autogram.AutogramBytes(alphabet, seed);
            for (int i = 0; i < AlphabetSize; i++)
            {
                autogram.Iterate();
            }
            teststring = autogram.ToString();
        }

        [Benchmark]
        public void GetActualCounts()
        {
            var x = autogram.GetActualCounts(teststring);
        }

        [Benchmark]
        public void GetActualCountsV2()
        {
            var x = autogram.GetActualCountsV2(teststring);
        }

    }
}
