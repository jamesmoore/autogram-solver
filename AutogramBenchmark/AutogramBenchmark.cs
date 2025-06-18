using Autogram;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class AutogramBenchmark
    {
        private AutogramConfig autogramConfig;

        public AutogramBenchmark()
        {
            var factory = new AutogramConfigFactory();

            autogramConfig = factory.MakeAutogramConfig("abcdefghijklmnopqrstz", "This sentence employs {0}.", " and ", "'s", "z");
        }


        [Benchmark]
        public void RunAutogramBytesNoStringsV4()
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
    }
}
