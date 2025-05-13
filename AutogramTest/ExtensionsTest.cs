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

        [Theory]
        [InlineData(0, "")]
        [InlineData(1, "a")]
        [InlineData(2, "a and b")]
        [InlineData(3, "a, b and c")]
        [InlineData(4, "a, b, c and d")]
        public void ListifyWithConjunctionTest(int itemCount,  string expected)
        {
            var items = Enumerable.Range(0, itemCount).Select(p => ((char)('a' + p)).ToString());
            var joined = items.ListifyWithConjunction();
            Assert.Equal(expected, joined);
        }
    }
}