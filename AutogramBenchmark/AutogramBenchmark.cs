using Autogram;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class AutogramBenchmark
    {
        [Params(100)]
        public int SeedCount;

        private AutogramConfig autogramConfig;

        [GlobalSetup]
        public void Setup()
        {
            var factory = new AutogramConfigFactory();
            autogramConfig = factory.MakeAutogramConfig(
                "abcdefghijklmnopqrstz",
                "This sentence employs {0}.",
                " and ",
                ", ",
                "'s",
                "z");
        }

        [Benchmark]
        public void AutogramBytesNoStringsV4_Solve_Average_Seeds_0_100()
        {
            for (int i = 0; i < 100; i++)
            {
                var x = new AutogramBytesNoStringsV4(autogramConfig, i);
                while (true)
                {
                    var result = x.Iterate();
                    if (result.Success) break;
                }
            }
        }

        [Benchmark]
        public void AutogramBytesNoStringsV5_Solve_Average_Seeds_0_100()
        {
            for (int i = 0; i < 100; i++)
            {
                var x = new AutogramBytesNoStringsV5(autogramConfig, i);
                while (true)
                {
                    var result = x.Iterate();
                    if (result.Success) break;
                }
            }
        }
    }
}
