using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class AutogramBench
    {
        const int AlphabetSize = 12;
        int? seed = 2001;

        private readonly Autogram.Autogram autogram;
        private readonly Autogram.Autogram autogramNoReset;
        private readonly Autogram.AutogramBytes autogramBytes;
        private readonly Autogram.AutogramBytes autogramBytesNoReset;

        public AutogramBench()
        {
            var alphabet = Enumerable.Range(0, AlphabetSize).Select(p => (char)('a' + p)).ToList();
            autogram = new Autogram.Autogram(alphabet, seed);
            autogramBytes = new Autogram.AutogramBytes(alphabet, seed);
            autogramNoReset = new Autogram.Autogram(alphabet, seed);
            autogramBytesNoReset = new Autogram.AutogramBytes(alphabet, seed);
        }

        [Benchmark]
        public void SolveAutogram()
        {
            autogram.Reset();
            while (true)
            {
                var status = autogram.Iterate();
                if (status.Success) break;
            }
        }

        [Benchmark]
        public void SolveAutogramNoReset()
        {
            autogramNoReset.Reset(false);
            while (true)
            {
                var status = autogramNoReset.Iterate();
                if (status.Success) break;
            }
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
