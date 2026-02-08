using Autogram;
using BenchmarkDotNet.Attributes;

namespace AutogramBenchmark
{
    public class AutogramBenchmark
    {
        [Params(10, 50, 100)]
        public int SeedCount;

        private AutogramConfig autogramConfig = null!;
        private List<AutogramBytesNoStringsV4> solver4List = null!;
        private List<AutogramBytesNoStringsV5> solver5List = null!;
        private List<AutogramBytesNoStringsV6> solver6List = null!;

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

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV4_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV4()
        {
            solver4List = Enumerable.Range(0, SeedCount).Select(p => new AutogramBytesNoStringsV4(autogramConfig, p)).ToList();
        }

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV5()
        {
            solver5List = Enumerable.Range(0, SeedCount).Select(p => new AutogramBytesNoStringsV5(autogramConfig, p)).ToList();
        }

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV6_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV6()
        {
            solver6List = Enumerable.Range(0, SeedCount).Select(p => new AutogramBytesNoStringsV6(autogramConfig, p)).ToList();
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV4_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV4()
        {
            solver4List = null!;
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV5()
        {
            solver5List = null!;
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV6_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV6()
        {
            solver6List = null!;
        }

        [Benchmark]
        public void AutogramBytesNoStringsV4_Solve_Average_Batched_Seeds()
        {
            foreach (var solver in solver4List)
            {
                while (true)
                {
                    var result = solver.Iterate();
                    if (result.Success) break;
                }
            }
        }

        [Benchmark]
        public void AutogramBytesNoStringsV5_Solve_Average_Batched_Seeds()
        {
            foreach (var solver in solver5List)
            {
                while (true)
                {
                    var result = solver.Iterate();
                    if (result.Success) break;
                }
            }
        }

        [Benchmark]
        public void AutogramBytesNoStringsV6_Solve_Average_Batched_Seeds()
        {
            foreach (var solver in solver6List)
            {
                while (true)
                {
                    var result = solver.Iterate();
                    if (result.Success) break;
                }
            }
        }
    }
}
