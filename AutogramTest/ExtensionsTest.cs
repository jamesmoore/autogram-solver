using Xunit.Abstractions;
using Autogram;

namespace AutogramTest
{
    public class ExtensionsTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ExtensionsTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ToNumberStringTest()
        {
            for (int i = 0; i < 100; i++)
            {
                string result = i.ToCardinalNumberString();
                _testOutputHelper.WriteLine(result);
                Assert.False(string.IsNullOrWhiteSpace(result));
            }
        }
    }
}