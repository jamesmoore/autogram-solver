using Autogram;

namespace AutogramTest;

public class SeparatorAccountingTests
{
    [Theory]
    [InlineData("abcdefghijklmnopqrstuvwxyz'- ,", ", ", " and finally ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz'-,", ", ", " and finally ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz'- ,", ",, ", " and finally ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz'- ,", ",, ", "")]
    [InlineData("abcdefghijklmnopqrstuvwxyz'- ,", ",, ", "   ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz'- ,", "e", " and finally ")]
    // Includes an invariant entry carrying a separator correction. Its folded
    // contributions must agree with the variable lookup tables.
    [InlineData("ade, ", ",, ", " and ")]
    public void LookupTotalsMatchRenderedSentence(string alphabet, string separator, string conjunction)
    {
        var config = MakeConfig(alphabet, separator, conjunction);
        var variables = config.VariableChars.ToList();
        var tables = config.GetVariableNumericCounts();

        Assert.All(config.AllChars, p => Assert.True(p.BaselineCount >= 0 && p.MinimumCount >= 0));
        Assert.All(variables, p => Assert.True(p.VariableBaselineCount >= 0));
        Assert.Equal(string.IsNullOrWhiteSpace(conjunction) ? 1 : 2,
            config.AllChars.Count(p => p.OmitSeparator));
        Assert.All(config.AllChars.Where(p => p.OmitSeparator), p => Assert.True(p.MinimumCount > 0));

        foreach (var quantity in new[] { 0, 1, 2, 21 })
        {
            var proposed = variables.Select(p => Math.Max(p.MinimumCount, quantity)).ToArray();
            var sentence = config.GetAutogramSnapshot(proposed).ToString().ToLowerInvariant();
            for (var target = 0; target < variables.Count; target++)
            {
                var computed = variables[target].VariableBaselineCount!.Value;
                for (var entry = 0; entry < variables.Count; entry++)
                {
                    if (proposed[entry] > 0)
                        computed += tables[entry][proposed[entry]][target];
                }
                Assert.Equal(sentence.Count(c => c == variables[target].Char), computed);
                Assert.True(computed >= variables[target].MinimumCount);
            }
        }
    }

    [Fact]
    public void VectorByteSolversAcceptCommaWithNoTemplateCommas()
    {
        var config = MakeConfig("abcdefghijklmnopqrstuvwxyz'-,", ", ", " and finally ");
        IAutogramFinder[] solvers =
        [
            new AutogramVector256(config, 123),
            new AutogramVector256V2(config, 123),
            new AutogramVector256V3(config, 123),
            new AutogramVector512(config, 123),
        ];
        foreach (var solver in solvers)
        {
            for (var i = 0; i < 25; i++)
                solver.Iterate();
        }
    }

    [Fact]
    public void ForcedLettersCanCarryTheSeparatorCorrection()
    {
        var config = new AutogramConfigFactory().MakeAutogramConfig(new AutogramInput
        {
            Alphabet = "ae,",
            Template = "{0}",
            Conjunction = " & ",
            SeparatorString = ",",
            PluralSuffix = "'s",
            Forced = "ae"
        });

        Assert.Equal(new[] { 'a', 'e' }, config.AllChars.Where(p => p.OmitSeparator).Select(p => p.Char));
        Assert.All(config.AllChars.Where(p => p.OmitSeparator), p => Assert.True(p.MinimumCount >= 1));
    }

    [Fact]
    public void TrackedSeparatorsRequireEnoughGuaranteedEntries()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            MakeConfig(",", ",", " and "));
        Assert.Contains("guaranteed list entries", exception.Message);
    }

    private static AutogramConfig MakeConfig(string alphabet, string separator, string conjunction) =>
        new AutogramConfigFactory().MakeAutogramConfig(new AutogramInput
        {
            Alphabet = alphabet,
            Template = "This sentence is an autogram and it contains {0}.",
            Conjunction = conjunction,
            SeparatorString = separator,
            PluralSuffix = "'s",
            Forced = ""
        });
}
