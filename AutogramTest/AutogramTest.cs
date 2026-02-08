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
            var autogramConfig = GetConfig();

            var sut = new AutogramBytesNoStringsV4(autogramConfig, RandomSeed);
            int i = 0;
            while (true)
            {
                var status = sut.Iterate();
                i++;
                if (status.Success)
                {
                    var result = sut.ToString(defaultTemplate, Conjunction, SeparatorString);
                    Assert.True(result.IsAutogram());
                    Assert.Equal(ExpectedIterations, i);
                    break;
                }
            }
        }

        [Fact]
        public void TestV5()
        {
            var autogramConfig = GetConfig();

            var sut = new AutogramBytesNoStringsV5(autogramConfig, RandomSeed);
            int i = 0;
            while (true)
            {
                var status = sut.Iterate();
                i++;
                if (status.Success)
                {
                    var result = sut.ToString(defaultTemplate, Conjunction, SeparatorString);
                    Assert.True(result.IsAutogram());
                    Assert.Equal(ExpectedIterations, i);
                    break;
                }
            }
        }

        [Fact]
        public void TestV6()
        {
            var autogramConfig = GetConfig();

            var sut = new AutogramBytesNoStringsV6(autogramConfig, RandomSeed);
            int i = 0;
            while (true)
            {
                var status = sut.Iterate();
                i++;
                if (status.Success)
                {
                    var result = sut.ToString(defaultTemplate, Conjunction, SeparatorString);
                    Assert.True(result.IsAutogram());
                    Assert.Equal(ExpectedIterations, i);
                    break;
                }
            }
        }
    }
}
