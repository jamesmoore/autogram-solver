using Autogram;

namespace AutogramTest
{
    public class CharExtensionsTests
    {
        [Theory]
        [InlineData(1,',', "comma")]
        [InlineData(1, '-', "hyphen")]
        [InlineData(1, '\'', "apostrophe")]
        [InlineData(1, ' ', "space")]
        [InlineData(1, 'a', "a")]
        [InlineData(2, ',', "commas")]
        [InlineData(2, '-', "hyphens")]
        [InlineData(2, '\'', "apostrophes")]
        [InlineData(2, ' ', "spaces")]
        [InlineData(2, 'a', "a's")]
        public void GetCharacterName_ReturnsExpectedName(int count, char character, string expected)
        {
            var result = character.GetCharacterName(count);
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
