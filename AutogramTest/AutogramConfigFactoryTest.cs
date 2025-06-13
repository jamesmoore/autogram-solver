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
            Assert.Equal(pluralExtension, config.PluralExtension);
            Assert.Single(config.Letters);

            var letterConfig = config.Letters.First();
            Assert.Equal('a', letterConfig.Char);
            Assert.Equal(0, letterConfig.Index);
            Assert.False(letterConfig.IsVariable);
            Assert.Equal(2, letterConfig.BaselineCount);
            Assert.Equal(3, letterConfig.MinimumCount);
            Assert.Null(letterConfig.VariableBaselineCount);
            Assert.Null(letterConfig.VariableIndex);

            Assert.All(config.Letters, TestLetterConfig);
            Assert.All(config.Letters.Where(p => p.IsVariable), TestLetterConfig);
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
            Assert.Equal(pluralExtension, config.PluralExtension);
            Assert.Equal(2, config.Letters.Count);

            var letterConfigA = config.Letters.First();
            Assert.Equal('a', letterConfigA.Char);
            Assert.Equal(0, letterConfigA.Index);
            Assert.False(letterConfigA.IsVariable);
            Assert.Equal(2, letterConfigA.BaselineCount); // 1 'a' in template and one in conjunction
            Assert.Equal(3, letterConfigA.MinimumCount);
            Assert.Null(letterConfigA.VariableBaselineCount);
            Assert.Null(letterConfigA.VariableIndex);

            var letterConfigE = config.Letters.Last();
            Assert.Equal('e', letterConfigE.Char);
            Assert.Equal(1, letterConfigE.Index);
            Assert.True(letterConfigE.IsVariable);
            Assert.Equal(1, letterConfigE.BaselineCount); // 1 'e' in template
            Assert.Equal(4, letterConfigE.MinimumCount); // a test three a and X e => 4 x 'e'
            Assert.Equal(3, letterConfigE.VariableBaselineCount); // a test three a and => 3 x 'e'
            Assert.Equal(0, letterConfigE.VariableIndex);

            Assert.All(config.Letters, TestLetterConfig);
            Assert.All(config.Letters.Where(p => p.IsVariable), TestLetterConfig);
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
            Assert.Equal(pluralExtension, config.PluralExtension);
            Assert.Equal(4, config.Letters.Count);

            var letterConfigA = config.Letters.First();
            Assert.Equal('a', letterConfigA.Char);
            Assert.Equal(0, letterConfigA.Index);
            Assert.False(letterConfigA.IsVariable);
            Assert.Equal(2, letterConfigA.BaselineCount); // 1 'a' in template and one in conjunction
            Assert.Equal(3, letterConfigA.MinimumCount); // a test three a, X e and one z => 3 x 'a'
            Assert.Null(letterConfigA.VariableBaselineCount);
            Assert.Null(letterConfigA.VariableIndex);

            var letterConfigE = config.Letters.Skip(1).First();
            Assert.Equal('e', letterConfigE.Char);
            Assert.Equal(1, letterConfigE.Index);
            Assert.True(letterConfigE.IsVariable);
            Assert.Equal(1, letterConfigE.BaselineCount); // 1 'e' in template
            Assert.Equal(5, letterConfigE.MinimumCount); // a test three a, X e and one z => 4 x 'e'
            Assert.Equal(4, letterConfigE.VariableBaselineCount); // a test three a and one e => 4 x 'e'
            Assert.Equal(0, letterConfigE.VariableIndex);

            var letterConfigR = config.Letters.Skip(2).First();
            Assert.Equal('r', letterConfigR.Char);
            Assert.Equal(2, letterConfigR.Index);
            Assert.True(letterConfigR.IsVariable); // in cardinals
            Assert.Equal(0, letterConfigR.BaselineCount); // No 'r' in template
            Assert.Equal(1, letterConfigR.MinimumCount); // "thRee" a's
            Assert.Equal(1, letterConfigR.VariableBaselineCount); // "thRee" a's
            Assert.Equal(1, letterConfigR.VariableIndex);

            var letterConfigZ = config.Letters.Last();
            Assert.Equal('z', letterConfigZ.Char);
            Assert.Equal(3, letterConfigZ.Index);
            Assert.False(letterConfigZ.IsVariable);
            Assert.Equal(0, letterConfigZ.BaselineCount); // No 'z' in template
            Assert.Equal(1, letterConfigZ.MinimumCount); // a test three a, X e and one z => 1 x 'z'
            Assert.Null(letterConfigZ.VariableBaselineCount);
            Assert.Null(letterConfigZ.VariableIndex);

            Assert.All(config.Letters, TestLetterConfig);
            Assert.All(config.Letters.Where(p => p.IsVariable), TestLetterConfig);
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
