namespace Autogram
{
    public class AutogramConfigFactory
    {
        /// <summary>
        /// Creates an autogram config, defining the inputs to the autogram search process.
        /// </summary>
        /// <param name="alphabet">The range of letters to count eg, "abcdefghijklmnopqrstuvwxyz"</param>
        /// <param name="template">The template for the body of the autogram eg, "This is an autogram and it contains {0}"</param>
        /// <param name="conjunction">The conjunction to use at the end of the letter count list eg, " and "</param>
        /// <param name="pluralExtension">The plural extension for letters with counts greater than one eg, "'s"</param>
        /// <param name="forced">Any characters absent from the template, conjunction and plural that should also be included in the count eg, "z"</param>
        /// <returns>A populated autogram config.</returns>
        public AutogramConfig MakeAutogramConfig(
            string alphabet,
            string template,
            string conjunction,
            string pluralExtension,
            string forced
            )
        {
            var baselineTemplate = string.Format(template, String.Empty);

            // CAVEAT#1: There is an assumption that the non-pluralised punctuation words will be present in the output
            var specialChars = alphabet.Where(p => p.HasExtendedName()).Select(p => p.GetPluralisedCharacterName()).ToList();

            var baselineString = (baselineTemplate + conjunction + (specialChars.Count != 0 ? specialChars.Aggregate((p, q) => p + q) : string.Empty)).ToLower();

            var numericStrings = GetNumericStrings();

            var separatorString = ExtensionsClass.Separator; // ", "

            var pluralisedNumericStrings = numericStrings.Select((p, i) => p + " " + (i == 1 ? String.Empty : pluralExtension) + separatorString);

            var relevantAlphabetArray = (
                baselineString +
                forced +
                pluralisedNumericStrings.Skip(1).Aggregate((p, q) => p + q)).ToLower().Distinct().Where(alphabet.Contains).OrderBy(p => GetOrder(p)).ToList();

            // an array of counts for the cardinal numbers plus possible plural
            var numericCounts = pluralisedNumericStrings.Select(p => p.GetFrequencies(relevantAlphabetArray).ToByteArray()).ToList();

            int variableIndex = 0;
            var letters = relevantAlphabetArray.Select((p, i) =>
                new
                {
                    Index = i,
                    Char = p,
                    BaselineCount = baselineString.Count(c => c == p),
                    IsVariable = numericCounts.Skip(1).Any(q => q[i] > 0), // skip(1) is to exclude "zero"
                    Forced = forced.ToLower().Contains(p),
                }
            ).Select(p => new CharacterConfig
            {
                Index = p.Index,
                Char = p.Char,
                BaselineCount = p.BaselineCount,
                IsVariable = p.IsVariable,
                VariableIndex = p.IsVariable ? variableIndex++ : null,
                MinimumCount = p.BaselineCount + (p.Char.HasExtendedName() == false && (p.BaselineCount > 0 || p.Forced) ? 1 : 0), // increment by 1 if guaranteed to be present unless it's got an extended name.
                VariableBaselineCount = p.IsVariable ? p.BaselineCount : null,
            }).ToList();

            AutogramConfig autogramConfig = new()
            {
                Template = template,
                Conjunction = conjunction,
                AllChars = letters,
            };

            var invariantLetters = letters.Where(c => c.IsVariable == false).ToList();

            var variableNumericCounts = autogramConfig.GetNumericCounts();

            // increment minimums with invariants
            foreach (var letter in invariantLetters)
            {
                var numericCount = numericCounts[letter.MinimumCount];
                for (int i = 0; i < numericCount.Length; i++)
                {
                    letters[i].MinimumCount += numericCount[i];
                }

                // add the cardinals of the invariants into the variant baseline.
                var numericCount3 = variableNumericCounts[letter.Index][letter.BaselineCount + 1];
                for (int i = 0; i < numericCount3.Length; i++)
                {
                    var letterConfig = letters.Single(p => p.VariableIndex == i);
                    letterConfig.VariableBaselineCount += numericCount3[i];
                }
            }

            // comma, space special case - when these get joined, the last two won't need a comma + space separator
            foreach (var letter in letters)
            {
                var countDelta = letter.PerDistinctCountModifier;
                letter.BaselineCount += countDelta;
                letter.MinimumCount += countDelta;
                letter.VariableBaselineCount += countDelta;
            }

            autogramConfig.Validate();
            return autogramConfig;
        }

        private static IEnumerable<string> GetNumericStrings()
        {
            return Enumerable.Range(0, 100).Select(p => ((byte)p).ToCardinalNumberStringPrecomputed().ToLower()).ToList();
        }

        private int GetOrder(char p)
        {
            switch (p)
            {
                case >= 'a' and <= 'z':
                    return p;
                default:
                    return p + 100;
            }
        }
    }
}
