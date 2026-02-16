using Autogram;

namespace AutogramTest
{
    public class CharExtensionsTests
    {
        [Theory]
        [InlineData(',', "comma")]
        [InlineData('-', "hyphen")]
        [InlineData('\'', "apostrophe")]
        [InlineData(' ', "space")]
        [InlineData('a', "a")]
        public void GetCharacterName_ReturnsExpectedName(char character, string expected)
        {
            var result = character.GetCharacterName(1);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(',', "commas")]
        [InlineData('-', "hyphens")]
        [InlineData('\'', "apostrophes")]
        [InlineData(' ', "spaces")]
        [InlineData('a', "a's")]
        public void GetPluralisedCharacterName_ReturnsExpectedName(char character, string expected)
        {
            var result = character.GetCharacterName(2);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData('a', 1, "a")]
        [InlineData('a', 2, "a's")]
        [InlineData(' ', 1, "space")]
        [InlineData(' ', 3, "spaces")]
        public void GetCharacterName_WithCount_ReturnsExpected(char character, int count, string expected)
        {
            var result = character.GetCharacterName(count);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData('a', false)]
        [InlineData(',', true)]
        public void HasExtendedName_ReturnsExpected(char character, bool expected)
        {
            var result = character.HasExtendedName();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPluralisedCharacterName_ReturnsApostropheSWithoutBackslash()
        {
            var result = 'a'.GetCharacterName(2);
            Assert.Equal("a's", result);
        }
    }
}
