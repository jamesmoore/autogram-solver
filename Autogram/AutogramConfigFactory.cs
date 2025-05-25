using System.Diagnostics;

namespace Autogram
{
    public class AutogramConfigFactory
    {
        public AutogramConfig MakeAutogramConfig(
            string alphabet,
            string template,
            string conjunction,
            string pluralExtension
            )
        {
            var baselineTemplate = string.Format(template, "");

            var numericStrings = Enumerable.Range(0, 100).Select(p => ((byte)p).ToCardinalNumberStringPrecomputed().ToLower()).ToList();

            var relevantAlphabetArray = (baselineTemplate + conjunction + pluralExtension + numericStrings.Skip(1).Aggregate((p, q) => p + q)).ToLower().Distinct().Where(alphabet.Contains).OrderBy(p => p).ToList();

            var pluralisedNumericStrings = numericStrings.Select((p, i) => p + (i == 1 ? "" : pluralExtension));

            // an array of counts for the cardinal numbers plus possible plural
            var numericCounts = pluralisedNumericStrings.Select(p => p.GetFrequencies(relevantAlphabetArray).ToByteArray()).ToList();

            var letters = relevantAlphabetArray.Select((p, i) =>
                new LetterConfig()
                {
                    Index = i,
                    Char = p,
                    BaselineCount = (baselineTemplate + conjunction).ToLower().Count(c => c == p),
                    IsVariable = numericCounts.Skip(1).Any(q => q[i] > 0), // skip(1) is to exclude "zero"
                }
            ).ToList();

            letters.Where(p => p.IsVariable).ToList().ForEach(p => p.VariableIndex = letters.Where(p => p.IsVariable).ToList().IndexOf(p));

            var variableNumericCounts = pluralisedNumericStrings.Select(p => p.GetFrequencies(letters.Where(c => c.IsVariable).Select(p => p.Char)).ToByteArray()).ToList();

            // calculate minumums first pass
            foreach (var letter in letters)
            {
                letter.MinimumCount = letter.BaselineCount > 0 ? letter.BaselineCount + 1 : 0;
                if (letter.IsVariable)
                {
                    letter.VariableBaselineCount = letter.BaselineCount;
                }
            }

            // increment minimums with invariants
            foreach (var letter in letters.Where(p => p.IsVariable == false))
            {
                var numericCount = numericCounts[letter.MinimumCount];
                for (int i = 0; i < numericCount.Length; i++)
                {
                    letters[i].MinimumCount += numericCount[i];
                }

                var numericCount2 = variableNumericCounts[letter.BaselineCount + 1];
                for (int i = 0; i < numericCount2.Length; i++)
                {
                    var letterConfig = letters.Single(p => p.VariableIndex == i);
                    letterConfig.VariableBaselineCount = (letterConfig.VariableBaselineCount ?? 0) + numericCount2[i];
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
