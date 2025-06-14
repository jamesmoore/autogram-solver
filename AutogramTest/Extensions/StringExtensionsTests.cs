using Autogram.Extensions;

namespace AutogramTest.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void IsAutogram_Successful_Test()
        {
            // https://en.wikipedia.org/wiki/Autogram
            const string example = @"This sentence employs 
                two a's, two c's, two d's, twenty-eight e's, five f's, three g's, eight h's, eleven i's, 
                three l's, two m's, thirteen n's, nine o's, two p's, five r's, twenty-five s's, 
                twenty-three t's, six v's, ten w's, two x's, five y's, and one z.";

            Assert.True(example.IsAutogram());
        }

        [Fact]
        public void IsAutogram_Failure_Test()
        {
            const string example = @"This sentence employs
                three a's, two c's, two d's, twenty-eight e's, five f's, three g's, eight h's, eleven i's, 
                three l's, two m's, thirteen n's, nine o's, two p's, five r's, twenty-five s's, 
                twenty-three t's, six v's, ten w's, two x's, five y's, and one z.";

            Assert.False(example.IsAutogram());
        }
    }
}
