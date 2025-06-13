using System.Diagnostics;

namespace Autogram
{
    public class AutogramConfigFactory
    {
        public AutogramConfig MakeAutogramConfig(
            string alphabet,
            string template,
            string conjunction,
            string pluralExtension,
            string forced
            )
        {
            var baselineTemplate = string.Format(template, String.Empty);

            var numericStrings = Enumerable.Range(0, 100).Select(p => ((byte)p).ToCardinalNumberStringPrecomputed().ToLower()).ToList();

            var relevantAlphabetArray = (baselineTemplate + conjunction + pluralExtension + forced + numericStrings.Skip(1).Aggregate((p, q) => p + q)).ToLower().Distinct().Where(alphabet.Contains).OrderBy(p => p).ToList();

            var pluralisedNumericStrings = numericStrings.Select((p, i) => p + (i == 1 ? String.Empty : pluralExtension));

            // an array of counts for the cardinal numbers plus possible plural
            var numericCounts = pluralisedNumericStrings.Select(p => p.GetFrequencies(relevantAlphabetArray).ToByteArray()).ToList();

            int variableIndex = 0;
            var letters = relevantAlphabetArray.Select((p, i) =>
                new
                {
                    Index = i,
                    Char = p,
                    BaselineCount = (baselineTemplate + conjunction).ToLower().Count(c => c == p),
                    IsVariable = numericCounts.Skip(1).Any(q => q[i] > 0), // skip(1) is to exclude "zero"
                }
            ).Select(p => new CharacterConfig
            {
                Index = p.Index,
                Char = p.Char,
                BaselineCount = p.BaselineCount,
                IsVariable = p.IsVariable,
                VariableIndex = p.IsVariable ? variableIndex++ : null,
                MinimumCount = p.BaselineCount > 0 || forced.ToLower().Contains(p.Char) ? p.BaselineCount + 1 : 0,
                VariableBaselineCount = p.IsVariable ? p.BaselineCount : null,
            }).ToList();

            var variableNumericCounts = pluralisedNumericStrings.Select(p => p.GetFrequencies(letters.Where(c => c.IsVariable).Select(p => p.Char)).ToByteArray()).ToList();

            // increment minimums with invariants
            foreach (var letter in letters.Where(p => p.IsVariable == false))
            {
                var numericCount = numericCounts[letter.MinimumCount];
                for (int i = 0; i < numericCount.Length; i++)
                {
                    letters[i].MinimumCount += numericCount[i];
                }

                // add the cardinals of the invariants into the variant baseline.
                var numericCount2 = variableNumericCounts[letter.BaselineCount + 1]; // baseline incrememted by 1 to include the letter itself
                for (int i = 0; i < numericCount2.Length; i++)
                {
                    var letterConfig = letters.Single(p => p.VariableIndex == i);
                    letterConfig.VariableBaselineCount += numericCount2[i];
                }
            }

            Debug.Assert(letters.All(p => p.MinimumCount >= p.BaselineCount));
            Debug.Assert(letters.Where(p => p.IsVariable).All(p => p.MinimumCount >= p.VariableBaselineCount));

            return new AutogramConfig()
            {
                Template = template,
                Conjunction = conjunction,
                PluralExtension = pluralExtension,
                Letters = letters,
                NumericCounts = numericCounts,
                VariableNumericCounts = variableNumericCounts,
            };
        }
    }
}
