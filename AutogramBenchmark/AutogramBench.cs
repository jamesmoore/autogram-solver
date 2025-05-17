using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class AutogramBench
    {
        const int AlphabetSize = 12;
        int? seed = 2001;

        private readonly Autogram.AutogramBytes autogramBytes;
        private readonly Autogram.AutogramBytes autogramBytesNoReset;

        public AutogramBench()
        {
            var alphabet = Enumerable.Range(0, AlphabetSize).Select(p => (char)('a' + p)).ToList();
            autogramBytes = new Autogram.AutogramBytes(alphabet, seed);
            autogramBytesNoReset = new Autogram.AutogramBytes(alphabet, seed);
        }

        [Benchmark]
        public void SolveAutogramBytes()
        {
            autogramBytes.Reset();
            while (true)
            {
                var status = autogramBytes.Iterate();
                if (status.Success) break;
            }
        }

        [Benchmark]
        public void SolveAutogramBytesNoReset()
        {
            autogramBytesNoReset.Reset(false);
            while (true)
            {
                var status = autogramBytesNoReset.Iterate();
                if (status.Success) break;
            }
        }

    }
}
