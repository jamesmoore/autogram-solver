using Autogram;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class ExtensionsBenchmark
    {
        private byte[] testData;

        public ExtensionsBenchmark()
        {
            var random = new Random(1);

            testData = Enumerable.Range(0, 26).Select(p => (byte)(random.Next() % 30)).ToArray();
        }

        [Benchmark]
        public void ToCardinalNumberStringPrecomputedBenchmark()
        {
            foreach (var item in testData)
            {
                item.ToCardinalNumberStringPrecomputed();
            }
        }
    }
}
