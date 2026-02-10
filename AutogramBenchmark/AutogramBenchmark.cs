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
        private List<AutogramBytesNoStringsV5a> solver5aList = null!;
        private List<AutogramBytesNoStringsV5b> solver5bList = null!;
        private List<AutogramBytesNoStringsV6> solver6List = null!;
        private List<AutogramIntsNoStringsV7> solver7List = null!;
        private List<AutogramBytesNoStringsV8> solver8List = null!;

        private static List<TSolver> CreateSolvers<TSolver>(int seedCount, Func<int, TSolver> factory)
        {
            var solvers = new List<TSolver>(seedCount);
            for (var i = 0; i < seedCount; i++)
            {
                solvers.Add(factory(i));
            }

            return solvers;
        }

        private static void ClearSolvers<TSolver>(ref List<TSolver> solvers)
        {
            solvers = null!;
        }

        private static void SolveAll<TSolver>(List<TSolver> solvers) where TSolver : IAutogramFinder
        {
            for (var i = 0; i < solvers.Count; i++)
            {
                var solver = solvers[i];
                while (true)
                {
                    var result = solver.Iterate();
                    if (result.Success) break;
                }
            }
        }
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
            solver4List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV4(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV5()
        {
            solver5List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5a_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV5a()
        {
            solver5aList = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5a(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5b_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV5b()
        {
            solver5bList = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5b(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV6_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV6()
        {
            solver6List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV6(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramIntsNoStringsV7_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV7()
        {
            solver7List = CreateSolvers(SeedCount, p => new AutogramIntsNoStringsV7(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV8_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV8()
        {
            solver8List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV8(autogramConfig, p));
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV4_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV4()
        {
            ClearSolvers(ref solver4List);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV5()
        {
            ClearSolvers(ref solver5List);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5a_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV5a()
        {
            ClearSolvers(ref solver5aList);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5b_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV5b()
        {
            ClearSolvers(ref solver5bList);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV6_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV6()
        {
            ClearSolvers(ref solver6List);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramIntsNoStringsV7_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV7()
        {
            ClearSolvers(ref solver7List);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV8_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV8()
        {
            ClearSolvers(ref solver8List);
        }

        [Benchmark]
        public void AutogramBytesNoStringsV4_Solve_Average_Batched_Seeds()
        {
            SolveAll(solver4List);
        }

        [Benchmark(Baseline = true)]
        public void AutogramBytesNoStringsV5_Solve_Average_Batched_Seeds()
        {
            SolveAll(solver5List);
        }

        [Benchmark]
        public void AutogramBytesNoStringsV5a_Solve_Average_Batched_Seeds()
        {
            SolveAll(solver5aList);
        }

        [Benchmark]
        public void AutogramBytesNoStringsV5b_Solve_Average_Batched_Seeds()
        {
            SolveAll(solver5bList);
        }

        [Benchmark]
        public void AutogramBytesNoStringsV6_Solve_Average_Batched_Seeds()
        {
            SolveAll(solver6List);
        }

        [Benchmark]
        public void AutogramIntsNoStringsV7_Solve_Average_Batched_Seeds()
        {
            SolveAll(solver7List);
        }

        [Benchmark]
        public void AutogramBytesNoStringsV8_Solve_Average_Batched_Seeds()
        {
            SolveAll(solver8List);
        }
    }
}
