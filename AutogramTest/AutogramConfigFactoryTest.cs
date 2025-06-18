using Autogram;

namespace AutogramTest
{
    public class AutogramConfigFactoryTest
    {
        [Fact]
        public void Test_AutogramConfigFactory_Single_Invariant()
        {
            const string alphabet = "a";
            const string template = "A test {0}";
            const string conjunction = " and ";
            const string pluralExtension = "'s";

            var sut = new AutogramConfigFactory();
            var config = sut.MakeAutogramConfig(alphabet, template, conjunction, pluralExtension, "");

            Assert.NotNull(config);
            Assert.Equal(template, config.Template);
            Assert.Equal(conjunction, config.Conjunction);
            var allChars = config.AllChars;
            Assert.Single(allChars);

            var letterConfig = allChars.First();
            Assert.Equal('a', letterConfig.Char);
            Assert.Equal(0, letterConfig.Index);
            Assert.False(letterConfig.IsVariable);
            Assert.Equal(2, letterConfig.BaselineCount);
            Assert.Equal(3, letterConfig.MinimumCount);
            Assert.Null(letterConfig.VariableBaselineCount);
            Assert.Null(letterConfig.VariableIndex);

            Assert.All(allChars, TestLetterConfig);
            Assert.All(allChars.Where(p => p.IsVariable), TestLetterConfig);
        }

        [Fact]
        public void Test_AutogramConfigFactory()
        {
            const string alphabet = "ae";
            const string template = "A test {0}";
            const string conjunction = " and ";
            const string pluralExtension = "'s";

            var sut = new AutogramConfigFactory();
            var config = sut.MakeAutogramConfig(alphabet, template, conjunction, pluralExtension, "");

            Assert.NotNull(config);
            Assert.Equal(template, config.Template);
            Assert.Equal(conjunction, config.Conjunction);
            var allChars = config.AllChars;
            Assert.Equal(2, allChars.Count);

            var letterConfigA = allChars.First();
            Assert.Equal('a', letterConfigA.Char);
            Assert.Equal(0, letterConfigA.Index);
            Assert.False(letterConfigA.IsVariable);
            Assert.Equal(2, letterConfigA.BaselineCount); // 1 'a' in template and one in conjunction
            Assert.Equal(3, letterConfigA.MinimumCount);
            Assert.Null(letterConfigA.VariableBaselineCount);
            Assert.Null(letterConfigA.VariableIndex);

            var letterConfigE = allChars.Last();
            Assert.Equal('e', letterConfigE.Char);
            Assert.Equal(1, letterConfigE.Index);
            Assert.True(letterConfigE.IsVariable);
            Assert.Equal(1, letterConfigE.BaselineCount); // 1 'e' in template
            Assert.Equal(4, letterConfigE.MinimumCount); // a test three a and X e => 4 x 'e'
            Assert.Equal(3, letterConfigE.VariableBaselineCount); // a test three a and => 3 x 'e'
            Assert.Equal(0, letterConfigE.VariableIndex);

            Assert.All(allChars, TestLetterConfig);
            Assert.All(allChars.Where(p => p.IsVariable), TestLetterConfig);
        }

        [Fact]
        public void Test_AutogramConfigFactory_With_Forced()
        {
            const string alphabet = "aerz";
            const string template = "A test {0}";
            const string conjunction = " and ";
            const string pluralExtension = "'s";
            const string forced = "z";

            var sut = new AutogramConfigFactory();
            var config = sut.MakeAutogramConfig(alphabet, template, conjunction, pluralExtension, forced);

            Assert.NotNull(config);
            Assert.Equal(template, config.Template);
            Assert.Equal(conjunction, config.Conjunction);
            var allChars = config.AllChars;
            Assert.Equal(4, allChars.Count);

            var letterConfigA = allChars.First();
            Assert.Equal('a', letterConfigA.Char);
            Assert.Equal(0, letterConfigA.Index);
            Assert.False(letterConfigA.IsVariable);
            Assert.Equal(2, letterConfigA.BaselineCount); // 1 'a' in template and one in conjunction
            Assert.Equal(3, letterConfigA.MinimumCount); // a test three a, X e and one z => 3 x 'a'
            Assert.Null(letterConfigA.VariableBaselineCount);
            Assert.Null(letterConfigA.VariableIndex);

            var letterConfigE = allChars.Skip(1).First();
            Assert.Equal('e', letterConfigE.Char);
            Assert.Equal(1, letterConfigE.Index);
            Assert.True(letterConfigE.IsVariable);
            Assert.Equal(1, letterConfigE.BaselineCount); // 1 'e' in template
            Assert.Equal(5, letterConfigE.MinimumCount); // a test three a, X e and one z => 4 x 'e'
            Assert.Equal(4, letterConfigE.VariableBaselineCount); // a test three a and one e => 4 x 'e'
            Assert.Equal(0, letterConfigE.VariableIndex);

            var letterConfigR = allChars.Skip(2).First();
            Assert.Equal('r', letterConfigR.Char);
            Assert.Equal(2, letterConfigR.Index);
            Assert.True(letterConfigR.IsVariable); // in cardinals
            Assert.Equal(0, letterConfigR.BaselineCount); // No 'r' in template
            Assert.Equal(1, letterConfigR.MinimumCount); // "thRee" a's
            Assert.Equal(1, letterConfigR.VariableBaselineCount); // "thRee" a's
            Assert.Equal(1, letterConfigR.VariableIndex);

            var letterConfigZ = allChars.Last();
            Assert.Equal('z', letterConfigZ.Char);
            Assert.Equal(3, letterConfigZ.Index);
            Assert.False(letterConfigZ.IsVariable);
            Assert.Equal(0, letterConfigZ.BaselineCount); // No 'z' in template
            Assert.Equal(1, letterConfigZ.MinimumCount); // a test three a, X e and one z => 1 x 'z'
            Assert.Null(letterConfigZ.VariableBaselineCount);
            Assert.Null(letterConfigZ.VariableIndex);

            Assert.All(config.AllChars, TestLetterConfig);
            Assert.All(allChars.Where(p => p.IsVariable), TestLetterConfig);
        }

        [Fact]
        public void Test_AutogramConfigFactory_With_Extended_Chars()
        {
            const string alphabet = "aerz ,-'";
            const string template = "A test {0}";
            const string conjunction = " and ";
            const string pluralExtension = "'s";

            var sut = new AutogramConfigFactory();
            var config = sut.MakeAutogramConfig(alphabet, template, conjunction, pluralExtension, "");

            Assert.NotNull(config);
            Assert.Equal(template, config.Template);
            Assert.Equal(conjunction, config.Conjunction);
            var allChars = config.AllChars;
            Assert.Equal(7, allChars.Count);

            var letterConfigA = allChars.First();
            Assert.Equal('a', letterConfigA.Char);
            Assert.Equal(0, letterConfigA.Index);
            Assert.False(letterConfigA.IsVariable); // in cardinals
            Assert.Equal(5, letterConfigA.BaselineCount); // 1 (template) + 1 (conjunction) + 3 (Apostrophe, spAce, commA) 
            Assert.Equal(6, letterConfigA.MinimumCount); // baseline + 1 for self
            Assert.Null(letterConfigA.VariableBaselineCount);
            Assert.Null(letterConfigA.VariableIndex);

            var letterConfigE = allChars.Skip(1).First();
            Assert.Equal('e', letterConfigE.Char);
            Assert.Equal(1, letterConfigE.Index);
            Assert.True(letterConfigE.IsVariable); // in cardinals
            Assert.Equal(4, letterConfigE.BaselineCount); // 1 (template) + 3 (apostrophE, spacE, hyphEn) 
            Assert.Equal(5, letterConfigE.MinimumCount); // baseline + 1 for self
            Assert.Equal(4, letterConfigE.VariableBaselineCount); 
            Assert.Equal(0, letterConfigE.VariableIndex);

            var letterConfigR = allChars.Skip(2).First();
            Assert.Equal('r', letterConfigR.Char);
            Assert.Equal(2, letterConfigR.Index);
            Assert.True(letterConfigR.IsVariable);
            Assert.Equal(1, letterConfigR.BaselineCount); // 1 (apostRophe)  
            Assert.Equal(2, letterConfigR.MinimumCount); // baseline + 1 for self 
            Assert.Equal(1, letterConfigR.VariableBaselineCount);
            Assert.Equal(1, letterConfigR.VariableIndex);

            var letterConfigSpace = allChars.Skip(3).First();
            Assert.Equal(' ', letterConfigSpace.Char);
            Assert.Equal(3, letterConfigSpace.Index);
            Assert.True(letterConfigSpace.IsVariable);
            Assert.Equal(2, letterConfigSpace.BaselineCount); // 4 in template+conjunction BUT 2 deducted (end of listify)
            Assert.Equal(4, letterConfigSpace.MinimumCount); // baseline + 2 for the "a" invariant
            Assert.Equal(4, letterConfigSpace.VariableBaselineCount);
            Assert.Equal(2, letterConfigSpace.VariableIndex);

            var letterConfigApostrophe = allChars.Skip(4).First();
            Assert.Equal('\'', letterConfigApostrophe.Char);
            Assert.Equal(4, letterConfigApostrophe.Index);
            Assert.True(letterConfigApostrophe.IsVariable);
            Assert.Equal(0, letterConfigApostrophe.BaselineCount); // There are 0 in the template
            Assert.Equal(1, letterConfigApostrophe.MinimumCount); // baseline + 1 for the "a's"
            Assert.Equal(1, letterConfigApostrophe.VariableBaselineCount);
            Assert.Equal(3, letterConfigApostrophe.VariableIndex);

            var letterConfigComma = allChars.Skip(5).First();
            Assert.Equal(',', letterConfigComma.Char);
            Assert.Equal(5, letterConfigComma.Index);
            Assert.True(letterConfigComma.IsVariable);
            Assert.Equal(-2, letterConfigComma.BaselineCount); // 0 in template+conjunction BUT 2 deducted (end of listify)
            Assert.Equal(-1, letterConfigComma.MinimumCount); // baseline + 1 for the "a's" 
            Assert.Equal(-1, letterConfigComma.VariableBaselineCount);
            Assert.Equal(4, letterConfigComma.VariableIndex);

            var letterConfigHyphen = allChars.Skip(6).First();
            Assert.Equal('-', letterConfigHyphen.Char);
            Assert.Equal(6, letterConfigHyphen.Index);
            Assert.True(letterConfigHyphen.IsVariable);
            Assert.Equal(0, letterConfigHyphen.BaselineCount); // Zero in template and cardinals
            Assert.Equal(0, letterConfigHyphen.MinimumCount); // No reason to increment from baseline
            Assert.Equal(0, letterConfigHyphen.VariableBaselineCount);
            Assert.Equal(5, letterConfigHyphen.VariableIndex);

            Assert.All(allChars, TestLetterConfig);
            Assert.All(allChars.Where(p => p.IsVariable), TestLetterConfig);
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
