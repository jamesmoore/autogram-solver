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
        private List<AutogramBytesNoStringsV5c> solver5cList = null!;
        private List<AutogramBytesNoStringsV5d> solver5dList = null!;
        private List<AutogramBytesNoStringsV5e> solver5eList = null!;
        private List<AutogramBytesNoStringsV5g> solver5gList = null!;
        private List<AutogramBytesNoStringsV5i> solver5iList = null!;
        private List<AutogramVector256> solverVector256List = null!;
        private List<AutogramVector256V2> solverVector256V2List = null!;
        private List<AutogramVector512> solverVector512List = null!;
        private List<AutogramBytesNoStringsV5h16> solver5h16List = null!;
        private List<AutogramBytesNoStringsV5h24> solver5h24List = null!;
        private List<AutogramBytesNoStringsV5h32> solver5h32List = null!;
        private List<AutogramBytesNoStringsV5h64> solver5h64List = null!;
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

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV4_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV4()
        //{
        //    solver4List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV4(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5()
        //{
        //    solver5List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5c_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5c()
        //{
        //    solver5cList = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5c(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5d_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5d()
        //{
        //    solver5dList = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5d(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5e_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5e()
        //{
        //    solver5eList = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5e(autogramConfig, p));
        //}

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5g_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV5g()
        {
            solver5gList = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5g(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5i_Solve_Average_Batched_Seeds) })]
        public void IterationSetupV5i()
        {
            solver5iList = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5i(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramVector256_Solve_Average_Batched_Seeds) })]
        public void IterationSetupVector256()
        {
            solverVector256List = CreateSolvers(SeedCount, p => new AutogramVector256(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramVector256V2_Solve_Average_Batched_Seeds) })]
        public void IterationSetupVector256V2()
        {
            solverVector256V2List = CreateSolvers(SeedCount, p => new AutogramVector256V2(autogramConfig, p));
        }

        [IterationSetup(Targets = new[] { nameof(AutogramVector512_Solve_Average_Batched_Seeds) })]
        public void IterationSetupVector512()
        {
            solverVector512List = CreateSolvers(SeedCount, p => new AutogramVector512(autogramConfig, p));
        }

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5h16_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5h16()
        //{
        //    solver5h16List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5h16(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5h24_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5h24()
        //{
        //    solver5h24List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5h24(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5h32_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5h32()
        //{
        //    solver5h32List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5h32(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5h64_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5h64()
        //{
        //    solver5h64List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5h64(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5a_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5a()
        //{
        //    solver5aList = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5a(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV5b_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV5b()
        //{
        //    solver5bList = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV5b(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV6_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV6()
        //{
        //    solver6List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV6(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramIntsNoStringsV7_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV7()
        //{
        //    solver7List = CreateSolvers(SeedCount, p => new AutogramIntsNoStringsV7(autogramConfig, p));
        //}

        //[IterationSetup(Targets = new[] { nameof(AutogramBytesNoStringsV8_Solve_Average_Batched_Seeds) })]
        //public void IterationSetupV8()
        //{
        //    solver8List = CreateSolvers(SeedCount, p => new AutogramBytesNoStringsV8(autogramConfig, p));
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV4_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV4()
        //{
        //    ClearSolvers(ref solver4List);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5()
        //{
        //    ClearSolvers(ref solver5List);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5c_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5c()
        //{
        //    ClearSolvers(ref solver5cList);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5d_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5d()
        //{
        //    ClearSolvers(ref solver5dList);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5e_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5e()
        //{
        //    ClearSolvers(ref solver5eList);
        //}

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5g_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV5g()
        {
            ClearSolvers(ref solver5gList);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5i_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationV5i()
        {
            ClearSolvers(ref solver5iList);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramVector256_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationVector256()
        {
            ClearSolvers(ref solverVector256List);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramVector256V2_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationVector256V2()
        {
            ClearSolvers(ref solverVector256V2List);
        }

        [IterationCleanup(Targets = new[] { nameof(AutogramVector512_Solve_Average_Batched_Seeds) })]
        public void CleanupIterationVector512()
        {
            ClearSolvers(ref solverVector512List);
        }

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5h16_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5h16()
        //{
        //    ClearSolvers(ref solver5h16List);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5h24_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5h24()
        //{
        //    ClearSolvers(ref solver5h24List);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5h32_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5h32()
        //{
        //    ClearSolvers(ref solver5h32List);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5h64_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5h64()
        //{
        //    ClearSolvers(ref solver5h64List);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5a_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5a()
        //{
        //    ClearSolvers(ref solver5aList);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV5b_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV5b()
        //{
        //    ClearSolvers(ref solver5bList);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV6_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV6()
        //{
        //    ClearSolvers(ref solver6List);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramIntsNoStringsV7_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV7()
        //{
        //    ClearSolvers(ref solver7List);
        //}

        //[IterationCleanup(Targets = new[] { nameof(AutogramBytesNoStringsV8_Solve_Average_Batched_Seeds) })]
        //public void CleanupIterationV8()
        //{
        //    ClearSolvers(ref solver8List);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV4_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver4List);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV5_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5List);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV5c_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5cList);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV5d_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5dList);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV5e_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5eList);
        //}

        [Benchmark(Baseline = true)]
        public void AutogramBytesNoStringsV5g_Solve_Average_Batched_Seeds()
        {
            SolveAll(solver5gList);
        }

        [Benchmark]
        public void AutogramBytesNoStringsV5i_Solve_Average_Batched_Seeds()
        {
            SolveAll(solver5iList);
        }

        [Benchmark]
        public void AutogramVector256_Solve_Average_Batched_Seeds()
        {
            SolveAll(solverVector256List);
        }

        [Benchmark]
        public void AutogramVector256V2_Solve_Average_Batched_Seeds()
        {
            SolveAll(solverVector256V2List);
        }

        [Benchmark]
        public void AutogramVector512_Solve_Average_Batched_Seeds()
        {
            SolveAll(solverVector512List);
        }

        //[Benchmark]
        //public void AutogramBytesNoStringsV5h16_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5h16List);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV5h24_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5h24List);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV5h32_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5h32List);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV5h64_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5h64List);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV5a_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5aList);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV5b_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver5bList);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV6_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver6List);
        //}

        //[Benchmark]
        //public void AutogramIntsNoStringsV7_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver7List);
        //}

        //[Benchmark]
        //public void AutogramBytesNoStringsV8_Solve_Average_Batched_Seeds()
        //{
        //    SolveAll(solver8List);
        //}
    }
}
