namespace Autogram
{
    public class AutogramConfigFactory
    {
        /// <summary>
        /// Creates an autogram config, defining the inputs to the autogram search process.
        /// </summary>
        /// <param name="autogramInput">The alphabet, sentence template, list formatting, pluralisation, and forced characters that define the autogram.</param>
        /// <returns>A populated autogram config.</returns>
        public AutogramConfig MakeAutogramConfig(AutogramInput autogramInput)
        {
            var baselineTemplate = string.Format(autogramInput.Template, String.Empty);

            // CAVEAT#1: There is an assumption that the pluralised punctuation words will be present in the output
            // This may affect unusual cases eg, very short alphabets where there is only a single comma, but comma was included in the alphabet.
            var specialChars = autogramInput.Alphabet.Where(p => p.HasExtendedName()).Select(p => p.GetCharacterName(2, autogramInput.PluralSuffix)).ToList();

            var baselineString = (baselineTemplate + autogramInput.Conjunction + (specialChars.Count != 0 ? specialChars.Aggregate((p, q) => p + q) : string.Empty)).ToLower();

            var pluralisedNumericStrings = 
                Enumerable.Range(0, 100).
                Select(p => ((byte)p).ToCardinalNumberStringPrecomputed().ToLower()).
                Select((p, i) => p + " " + (i == 1 ? String.Empty : autogramInput.PluralSuffix) + autogramInput.SeparatorString).ToList();

            var relevantAlphabetArray = (
                baselineString +
                autogramInput.Forced +
                pluralisedNumericStrings.Skip(1).Aggregate((p, q) => p + q)).ToLower().Distinct().Where(autogramInput.Alphabet.Contains).OrderBy(p => GetOrder(p)).ToList();

            // an array of counts for the cardinal numbers plus possible plural and separator
            var pluralisedNumericStringsCounts = pluralisedNumericStrings.Select(p => p.GetFrequencies(relevantAlphabetArray).ToByteArray()).ToList();

            var isVariable = relevantAlphabetArray.Select((p, i) => pluralisedNumericStringsCounts.Skip(1).Any(q => q[i] > 0)).ToList(); // skip(1) is to exclude "zero"

            var letters = relevantAlphabetArray.Select((p, i) => new CharacterConfig(autogramInput.SeparatorString, autogramInput.PluralSuffix)
            {
                Index = i,
                Char = p,
                Forced = autogramInput.Forced.ToLower().Contains(p),
                UnadjustedBaselineCount = baselineString.Count(c => c == p),
                IsVariable = isVariable[i],
            }).ToList();

            AutogramConfig autogramConfig = new()
            {
                AllChars = letters,
                Input = autogramInput,
            };

            var invariantLetters = letters.Where(c => c.IsVariable == false).ToList();

            var variableNumericCounts = autogramConfig.GetNumericCounts();

            var variableLetters = autogramConfig.VariableChars.ToList();

            // An invariant character cannot occur in generated count wording, so its
            // MinimumCount is already its exact final count. Account for each invariant's
            // rendered list entry in the remaining character counts.
            foreach (var letter in invariantLetters)
            {
                // The minimum count of the invariant character is already its exact final count.
                var minimumCount = letter.MinimumCount;
                
                // Add the invariant's number word, plural suffix, and separator to the
                // minimum counts. The character itself was accounted for when its
                // MinimumCount was initialized.
                // Example: "seven a's" contributes two e's, two s's, one v, one n, etc.
                var numericCount = pluralisedNumericStringsCounts[minimumCount];
                for (int i = 0; i < numericCount.Length; i++)
                {
                    letters[i].InvariantMinimumContribution += numericCount[i];
                }

                // Add the full rendered invariant entry to the baselines used by the
                // variable-character search. This table contains only variable-character
                // frequencies, but includes the invariant's self-reference when applicable.
                var variableNumericCount = variableNumericCounts[letter.Index][minimumCount];
                for (int i = 0; i < variableNumericCount.Length; i++)
                {
                    variableLetters[i].InvariantBaselineContribution += variableNumericCount[i];
                }
            }

            autogramConfig.Validate();
            return autogramConfig;
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
