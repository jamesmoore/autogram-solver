using Autogram;

namespace AutogramTest
{
    public class AutogramConfigFactoryTest
    {
        private const string SeparatorString = ", ";

        [Theory]
        [InlineData(",", ",,,{0}", 2)]
        [InlineData(",", "{0}", 0)]
        public void VariablePunctuationNameMinimumPreservesRepeatedCharacters(string separator, string template, int expected)
        {
            var config = new AutogramConfigFactory().MakeAutogramConfig(new AutogramInput
            {
                Alphabet = "abcdefghijklmnopqrstuvwxyz,",
                Template = template,
                Conjunction = "",
                SeparatorString = separator,
                PluralSuffix = "",
                Forced = "e"
            });

            Assert.Equal(expected, config.AllChars.Single(p => p.Char == 'm').GuaranteedNameContribution);
            Assert.Equal(0, config.AllChars.Single(p => p.Char == 's').GuaranteedNameContribution);
        }

        [Theory]
        [InlineData(",{0}", 1)]
        [InlineData(",,{0}", 2)]
        public void InvariantPunctuationAddsExactNameToMinimum(string template, int commaCount)
        {
            var config = new AutogramConfigFactory().MakeAutogramConfig(new AutogramInput
            {
                Alphabet = "abcdefghijklmnopqrstuvwxyz,",
                Template = template,
                Conjunction = "",
                SeparatorString = "",
                PluralSuffix = "",
                Forced = ""
            });

            Assert.Equal(commaCount, config.AllChars.Single(p => p.Char == ',').MinimumCount);
            Assert.Equal(2, config.AllChars.Single(p => p.Char == 'm').InvariantMinimumContribution);
            Assert.Equal(commaCount == 1 ? 0 : 1, config.AllChars.Single(p => p.Char == 's').InvariantMinimumContribution);
            Assert.All(config.VariableChars, p => Assert.True(p.MinimumCount >= p.VariableBaselineCount));
        }

        [Fact]
        public void Test_AutogramConfigFactory_Single_Invariant()
        {
            var input = new AutogramInput
            {
                Alphabet = "a",
                Template = "A test {0}",
                Conjunction = " and ",
                SeparatorString = SeparatorString,
                PluralSuffix = "'s",
                Forced = ""
            };

            var sut = new AutogramConfigFactory();
            var config = sut.MakeAutogramConfig(input);

            Assert.NotNull(config);
            var allChars = config.AllChars;
            Assert.Single(allChars);

            var letterConfig = allChars.First();
            Assert.Equal('a', letterConfig.Char);
            Assert.Equal(0, letterConfig.Index);
            Assert.False(letterConfig.IsVariable);
            Assert.Equal(2, letterConfig.BaselineCount);
            Assert.Equal(3, letterConfig.MinimumCount);
            Assert.Null(letterConfig.VariableBaselineCount);

            Assert.All(allChars, TestLetterConfig);
            Assert.All(allChars.Where(p => p.IsVariable), TestLetterConfig);
        }

        [Fact]
        public void Test_AutogramConfigFactory()
        {
            var input = new AutogramInput
            {
                Alphabet = "ae",
                Template = "A test {0}",
                Conjunction = " and ",
                SeparatorString = SeparatorString,
                PluralSuffix = "'s",
                Forced = ""
            };

            var sut = new AutogramConfigFactory();
            var config = sut.MakeAutogramConfig(input);

            Assert.NotNull(config);
            var allChars = config.AllChars;
            Assert.Equal(2, allChars.Count);

            var letterConfigA = allChars.First();
            Assert.Equal('a', letterConfigA.Char);
            Assert.Equal(0, letterConfigA.Index);
            Assert.False(letterConfigA.IsVariable);
            Assert.Equal(2, letterConfigA.BaselineCount); // 1 'a' in template and one in conjunction
            Assert.Equal(3, letterConfigA.MinimumCount);
            Assert.Null(letterConfigA.VariableBaselineCount);

            var letterConfigE = allChars.Last();
            Assert.Equal('e', letterConfigE.Char);
            Assert.Equal(1, letterConfigE.Index);
            Assert.True(letterConfigE.IsVariable);
            Assert.Equal(1, letterConfigE.BaselineCount); // 1 'e' in template
            Assert.Equal(4, letterConfigE.MinimumCount); // a test three a and X e => 4 x 'e'
            Assert.Equal(3, letterConfigE.VariableBaselineCount); // a test three a and => 3 x 'e'

            Assert.All(allChars, TestLetterConfig);
            Assert.All(allChars.Where(p => p.IsVariable), TestLetterConfig);
        }

        [Fact]
        public void Test_AutogramConfigFactory_With_Forced()
        {
            var input = new AutogramInput
            {
                Alphabet = "aerz",
                Template = "A test {0}",
                Conjunction = " and ",
                SeparatorString = SeparatorString,
                PluralSuffix = "'s",
                Forced = "z"
            };

            var sut = new AutogramConfigFactory();
            var config = sut.MakeAutogramConfig(input);

            Assert.NotNull(config);
            var allChars = config.AllChars;
            Assert.Equal(4, allChars.Count);

            var letterConfigA = allChars.First();
            Assert.Equal('a', letterConfigA.Char);
            Assert.Equal(0, letterConfigA.Index);
            Assert.False(letterConfigA.IsVariable);
            Assert.Equal(2, letterConfigA.BaselineCount); // 1 'a' in template and one in conjunction
            Assert.Equal(3, letterConfigA.MinimumCount); // a test three a, X e and one z => 3 x 'a'
            Assert.Null(letterConfigA.VariableBaselineCount);

            var letterConfigE = allChars.Skip(1).First();
            Assert.Equal('e', letterConfigE.Char);
            Assert.Equal(1, letterConfigE.Index);
            Assert.True(letterConfigE.IsVariable);
            Assert.Equal(1, letterConfigE.BaselineCount); // 1 'e' in template
            Assert.Equal(5, letterConfigE.MinimumCount); // a test three a, X e and one z => 4 x 'e'
            Assert.Equal(4, letterConfigE.VariableBaselineCount); // a test three a and one e => 4 x 'e'

            var letterConfigR = allChars.Skip(2).First();
            Assert.Equal('r', letterConfigR.Char);
            Assert.Equal(2, letterConfigR.Index);
            Assert.True(letterConfigR.IsVariable); // in cardinals
            Assert.Equal(0, letterConfigR.BaselineCount); // No 'r' in template
            Assert.Equal(1, letterConfigR.MinimumCount); // "thRee" a's
            Assert.Equal(1, letterConfigR.VariableBaselineCount); // "thRee" a's

            var letterConfigZ = allChars.Last();
            Assert.Equal('z', letterConfigZ.Char);
            Assert.Equal(3, letterConfigZ.Index);
            Assert.False(letterConfigZ.IsVariable);
            Assert.Equal(0, letterConfigZ.BaselineCount); // No 'z' in template
            Assert.Equal(1, letterConfigZ.MinimumCount); // a test three a, X e and one z => 1 x 'z'
            Assert.Null(letterConfigZ.VariableBaselineCount);

            Assert.All(config.AllChars, TestLetterConfig);
            Assert.All(allChars.Where(p => p.IsVariable), TestLetterConfig);
        }

        [Fact]
        public void Test_AutogramConfigFactory_With_WordNamed_Chars()
        {
            var input = new AutogramInput
            {
                Alphabet = "aerz ,-'",
                Template = "A test {0}",
                Conjunction = " and ",
                SeparatorString = SeparatorString,
                PluralSuffix = "'s",
                Forced = ""
            };

            var sut = new AutogramConfigFactory();
            var config = sut.MakeAutogramConfig(input);

            Assert.NotNull(config);
            var allChars = config.AllChars;
            Assert.Equal(7, allChars.Count);

            var a = allChars.Single(p => p.Char == 'a');
            Assert.True(a.IsVariable);
            Assert.Equal(2, a.BaselineCount);
            Assert.Equal(4, a.MinimumCount);
            Assert.Equal(2, a.VariableBaselineCount);
            var e = allChars.Single(p => p.Char == 'e');
            Assert.Equal(1, e.BaselineCount);
            Assert.Equal(3, e.MinimumCount);
            Assert.Equal(1, e.VariableBaselineCount);
            Assert.Equal(0, allChars.Single(p => p.Char == 'r').MinimumCount);
            Assert.Equal(4, allChars.Single(p => p.Char == ' ').MinimumCount);
            Assert.Equal(0, allChars.Single(p => p.Char == '\'').MinimumCount);
            Assert.Equal(0, allChars.Single(p => p.Char == ',').MinimumCount);
            Assert.True(allChars.Single(p => p.Char == '-').IsVariable);
            Assert.All(allChars, TestLetterConfig);
            Assert.All(allChars.Where(p => p.IsVariable), TestLetterConfig);
        }

        [Fact]
        public void InvariantComma_CanHaveDifferentMinimumAndBaselineContributions()
        {
            var input = new AutogramInput
            {
                Alphabet = "s,",
                Template = ",,{0}",
                Conjunction = " and ",
                SeparatorString = "; ",
                PluralSuffix = "'s",
                Forced = ""
            };

            var config = new AutogramConfigFactory().MakeAutogramConfig(input);

            var comma = config.AllChars.Single(p => p.Char == ',');
            Assert.False(comma.IsVariable);
            Assert.Equal(2, comma.MinimumCount);

            var s = config.AllChars.Single(p => p.Char == 's');
            // Both minimum and baseline contributions include the exact plural name.
            // The punctuation name is no longer included in the template baseline.
            Assert.Equal(1, s.InvariantMinimumContribution);
            Assert.Equal(1, s.InvariantBaselineContribution);
            Assert.Equal(1, s.MinimumCount);
            Assert.Equal(1, s.VariableBaselineCount);
            Assert.All(config.AllChars, TestLetterConfig);
        }

        [Theory]
        [InlineData("a{0}e", 1, 4)]
        [InlineData("a{0}ee", 2, 5)]
        public void LetterInSeparator_RetainsGuaranteedSelfCount(
            string template, int baselineCount, int minimumCount)
        {
            var input = new AutogramInput
            {
                Alphabet = "ae",
                Template = template,
                Conjunction = " and ",
                SeparatorString = "e",
                PluralSuffix = "'s",
                Forced = ""
            };

            var config = new AutogramConfigFactory().MakeAutogramConfig(input);

            var e = config.AllChars.Single(p => p.Char == 'e');
            Assert.Equal(baselineCount, e.BaselineCount);
            Assert.Equal(1, e.GuaranteedSelfCount);
            Assert.Equal(minimumCount, e.MinimumCount);
            Assert.Equal(minimumCount - 1, e.VariableBaselineCount);
            Assert.All(config.AllChars, TestLetterConfig);
        }

        private static void TestLetterConfig(CharacterConfig p)
        {
            Assert.True(p.MinimumCount >= p.BaselineCount);
            if (p.IsVariable)
            {
                Assert.True(p.MinimumCount >= p.VariableBaselineCount);
            }
        }
    }
}


