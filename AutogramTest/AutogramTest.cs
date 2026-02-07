using Autogram;
using Autogram.Extensions;

namespace AutogramTest
{
    public class AutogramTest
    {
        const string defaultAlphabetRegex = "[a-z]";
        const string defaultTemplate = "This sentence is an autogram and it contains {0}."; // from https://en.wikipedia.org/wiki/Autogram
        const string defaultConjunction = " and ";
        const string defaultForced = "";
        private const int ExpectedIterations = 1710690;
        const int RandomSeed = 2021428396; // cherry picked for fast resolve

        private static AutogramConfig GetConfig()
        {
            return new AutogramConfigFactory().MakeAutogramConfig(
                "abcdefghijklmnopqrstuvwxyz",
                defaultTemplate,
                " and lastly ",
                ", ",
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
                    var result = sut.ToString();
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
                    var result = sut.ToString();
                    Assert.True(result.IsAutogram());
                    Assert.Equal(ExpectedIterations, i);
                    break;
                }
            }
        }
    }
}
