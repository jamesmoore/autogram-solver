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

        [Theory]
        [InlineData("one apostrophe,", '\'', 1)]
        [InlineData("one apostrophe and", '\'', 1)]
        [InlineData("one apostrophe", '\'', 1)]
        [InlineData("two apostrophes,", '\'', 2)]
        [InlineData("one comma,", ',', 1)]
        [InlineData("one hyphen.", '-', 1)]
        [InlineData("one space.", ' ', 1)]
        public void GetStatedFrequency_Punctuation_Tests(string sentence, char character, int count)
        {
            var result = sentence.GetStatedFrequency();

            Assert.Equal(count, result[character]);
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
        public void IsAutogram_Invalid_Test()
        {
            const string example = @"Not an autogram.";

            Assert.False(example.IsAutogram());
        }

        [Fact]
        public void IsAutogram_Invalid_Extended_Test()
        {
            const string InvalidAutogram = "This sentence is an autogram and it contains ten a's, four c's, three d's, thirty-three e's, ten f's, four g's, fifteen h's, fourteen i's, three l's, four m's, twenty n's, sixteen o's, four p's, seventeen r's, thirty-two s's, thirty-three t's, eight u's, two v's, six w's, three x's, eight y's, one apostrophe, twenty-two commas and finally four hyphens.";
            Assert.False(InvalidAutogram.IsAutogram());
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
