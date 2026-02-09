using Autogram;
using Autogram.Extensions;

namespace AutogramTest
{
    public class AutogramTest
    {
        private const string defaultTemplate = "This sentence is an autogram and it contains {0}."; // from https://en.wikipedia.org/wiki/Autogram
        private const int ExpectedIterations = 1710690;
        private const int RandomSeed = 2021428396; // cherry picked for fast resolve
        private const string Conjunction = " and lastly ";
        private const string SeparatorString = ", ";

        private static void RunAutogramTest<TAutogram>(
            Func<AutogramConfig, int, TAutogram> factory,
            Func<TAutogram, string> formatter,
            int expectedIterations)
            where TAutogram : IAutogramFinder
        {
            var autogramConfig = GetConfig();
            var sut = factory(autogramConfig, RandomSeed);
            int i = 0;
            while (true)
            {
                var status = sut.Iterate();
                i++;
                if (status.Success)
                {
                    var result = formatter(sut);
                    Assert.True(result.IsAutogram());
                    Assert.Equal(expectedIterations, i);
                    break;
                }
            }
        }

        private static AutogramConfig GetConfig()
        {
            return new AutogramConfigFactory().MakeAutogramConfig(
                "abcdefghijklmnopqrstuvwxyz",
                defaultTemplate,
                Conjunction,
                SeparatorString,
                "'s",
                "");
        }

        [Fact]
        public void Test()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV4(config, seed),
                sut => sut.ToString(defaultTemplate, Conjunction, SeparatorString),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5(config, seed),
                sut => sut.ToString(defaultTemplate, Conjunction, SeparatorString),
                ExpectedIterations);
        }

        [Fact]
        public void TestV5a()
        {
            const int Expected = 1042770;
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV5a(config, seed),
                sut => sut.ToString(defaultTemplate, Conjunction, SeparatorString),
                Expected);
        }


        [Fact]
        public void TestV6()
        {
            RunAutogramTest(
                (config, seed) => new AutogramBytesNoStringsV6(config, seed),
                sut => sut.ToString(defaultTemplate, Conjunction, SeparatorString),
                ExpectedIterations);
        }

        [Fact]
        public void TestV7()
        {
            RunAutogramTest(
                (config, seed) => new AutogramIntsNoStringsV7(config, seed),
                sut => sut.ToString(defaultTemplate, Conjunction, SeparatorString),
                ExpectedIterations);
        }
    }
}
