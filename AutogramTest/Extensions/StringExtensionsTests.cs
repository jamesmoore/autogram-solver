using Autogram.Extensions;

namespace AutogramTest.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void GetCharFrequencyTests()
        {
            const string example = "This is a test sentence.";
            var result = example.GetCharFrequency();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(1, result['T']);
            Assert.Equal(3, result['t']);
            Assert.Equal(2, result['i']);
            Assert.Equal(1, result['.']);
            Assert.Equal(4, result[' ']);
        }

        [Fact]
        public void GetStatedFrequencyTests()
        {
            // https://en.wikipedia.org/wiki/Autogram
            const string example = @"This sentence employs 
                two a's, two c's, two d's, twenty-eight e's, five f's, three g's, eight h's, eleven i's, 
                three l's, two m's, thirteen n's, nine o's, two p's, five r's, twenty-five s's, 
                twenty-three t's, six v's, ten w's, two x's, five y's, and one z.";
            var result = example.GetStatedFrequency();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(1, result['z']);
            Assert.Equal(2, result['a']);
            Assert.Equal(2, result['d']);
            Assert.Equal(28, result['e']);
        }

        [Fact]
        public void GetStatedFrequency_Extended_Char_Tests()
        {
            // https://en.wikipedia.org/wiki/Autogram
            const string example = @"Ten spaces, two commas, three a's.";
            var result = example.GetStatedFrequency();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(3, result['a']);
            Assert.Equal(2, result[',']);
            Assert.Equal(10, result[' ']);
        }

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

        [Fact]
        public void IsAutogram_Extended_Char_Success_Test()
        {
            const string example = @"
                Only the fool would take trouble to verify that his sentence was composed of 
                ten a's, three b's, four c's, four d's, forty-six e's, sixteen f's, four g's, thirteen h's, 
                fifteen i's, two k's, nine l's, four m's, twenty-five n's, twenty-four o's, five p's, sixteen r's, 
                forty-one s's, thirty-seven t's, ten u's, eight v's, eight w's, four x's, eleven y's, twenty-seven commas, 
                twenty-three apostrophes, seven hyphens and, last but not least, a single !
                ";

            Assert.True(example.IsAutogram());
        }

        [Fact]
        public void IsAutogram_Extended_Char_Fail_Test()
        {
            const string example = @"
                Only the fool would take trouble to verify that his sentence was composed of 
                ten a's, three b's, four c's, four d's, forty-six e's, sixteen f's, four g's, thirteen h's, 
                fifteen i's, two k's, nine l's, four m's, twenty-five n's, twenty-four o's, five p's, sixteen r's, 
                forty-one s's, thirty-seven t's, ten u's, eight v's, eight w's, four x's, eleven y's, twenty-seven commas, 
                twenty-three apostrophes, six hyphens and, last but not least, a single !
                ";

            Assert.False(example.IsAutogram());
        }
    }
}
