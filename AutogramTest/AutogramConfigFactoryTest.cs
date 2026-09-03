using Autogram;

namespace AutogramTest
{
    public class AutogramConfigFactoryTest
    {
        private const string SeparatorString = ", ";

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
        public void Test_AutogramConfigFactory_With_Extended_Chars()
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

            var letterConfigA = allChars.First();
            Assert.Equal('a', letterConfigA.Char);
            Assert.Equal(0, letterConfigA.Index);
            Assert.False(letterConfigA.IsVariable); // in cardinals
            Assert.Equal(5, letterConfigA.BaselineCount); // 1 (template) + 1 (conjunction) + 3 (Apostrophe, spAce, commA) 
            Assert.Equal(6, letterConfigA.MinimumCount); // baseline + 1 for self
            Assert.Null(letterConfigA.VariableBaselineCount);

            var letterConfigE = allChars.Skip(1).First();
            Assert.Equal('e', letterConfigE.Char);
            Assert.Equal(1, letterConfigE.Index);
            Assert.True(letterConfigE.IsVariable); // in cardinals
            Assert.Equal(4, letterConfigE.BaselineCount); // 1 (template) + 3 (apostrophE, spacE, hyphEn) 
            Assert.Equal(5, letterConfigE.MinimumCount); // baseline + 1 for self
            Assert.Equal(4, letterConfigE.VariableBaselineCount); 

            var letterConfigR = allChars.Skip(2).First();
            Assert.Equal('r', letterConfigR.Char);
            Assert.Equal(2, letterConfigR.Index);
            Assert.True(letterConfigR.IsVariable);
            Assert.Equal(1, letterConfigR.BaselineCount); // 1 (apostRophe)  
            Assert.Equal(2, letterConfigR.MinimumCount); // baseline + 1 for self 
            Assert.Equal(1, letterConfigR.VariableBaselineCount);

            var letterConfigSpace = allChars.Skip(3).First();
            Assert.Equal(' ', letterConfigSpace.Char);
            Assert.Equal(3, letterConfigSpace.Index);
            Assert.True(letterConfigSpace.IsVariable);
            Assert.Equal(2, letterConfigSpace.BaselineCount); // 4 in template+conjunction BUT 2 deducted (end of listify)
            Assert.Equal(4, letterConfigSpace.MinimumCount); // baseline + 2 for the "a" invariant
            Assert.Equal(4, letterConfigSpace.VariableBaselineCount);

            var letterConfigApostrophe = allChars.Skip(4).First();
            Assert.Equal('\'', letterConfigApostrophe.Char);
            Assert.Equal(4, letterConfigApostrophe.Index);
            Assert.True(letterConfigApostrophe.IsVariable);
            Assert.Equal(0, letterConfigApostrophe.BaselineCount); // There are 0 in the template
            Assert.Equal(1, letterConfigApostrophe.MinimumCount); // baseline + 1 for the "a's"
            Assert.Equal(1, letterConfigApostrophe.VariableBaselineCount);

            var letterConfigComma = allChars.Skip(5).First();
            Assert.Equal(',', letterConfigComma.Char);
            Assert.Equal(5, letterConfigComma.Index);
            Assert.True(letterConfigComma.IsVariable);
            Assert.Equal(-2, letterConfigComma.BaselineCount); // 0 in template+conjunction BUT 2 deducted (end of listify)
            Assert.Equal(-1, letterConfigComma.MinimumCount); // baseline + 1 for the "a's" 
            Assert.Equal(-1, letterConfigComma.VariableBaselineCount);

            var letterConfigHyphen = allChars.Skip(6).First();
            Assert.Equal('-', letterConfigHyphen.Char);
            Assert.Equal(6, letterConfigHyphen.Index);
            Assert.True(letterConfigHyphen.IsVariable);
            Assert.Equal(0, letterConfigHyphen.BaselineCount); // Zero in template and cardinals
            Assert.Equal(0, letterConfigHyphen.MinimumCount); // No reason to increment from baseline
            Assert.Equal(0, letterConfigHyphen.VariableBaselineCount);

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
            // The generic numeric table includes the plural suffix; the character-specific
            // table omits "commas", whose letters are already in the baseline.
            Assert.Equal(1, s.InvariantMinimumContribution);
            Assert.Equal(0, s.InvariantBaselineContribution);
            Assert.Equal(3, s.MinimumCount);
            Assert.Equal(1, s.VariableBaselineCount);
            Assert.All(config.AllChars, TestLetterConfig);
        }

        [Theory]
        [InlineData("a{0}e", 1, 3)]
        [InlineData("a{0}ee", 2, 4)]
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
            Assert.Equal(baselineCount, e.UnadjustedBaselineCount);
            Assert.Equal(baselineCount - 2, e.BaselineCount);
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
