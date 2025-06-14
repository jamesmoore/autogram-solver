using System.Text.RegularExpressions;
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

        [Fact]
        public void Test()
        {
            var autogramConfig = new AutogramConfigFactory().MakeAutogramConfig(
                "abcdefghijklmnopqrstuvwxyz",
                defaultTemplate,
                " and lastly ",
                "'s",
                "");

            const int RandomSeed = 2021428396; // cherry picked for fast resolve
            var sut = new AutogramBytesNoStringsV4(autogramConfig, RandomSeed);

            while (true)
            {
                var status = sut.Iterate();

                if (status.Success)
                {
                    var result = sut.ToString();
                    Assert.True(result.IsAutogram());
                    break;
                }
            }
        }
    }
}
