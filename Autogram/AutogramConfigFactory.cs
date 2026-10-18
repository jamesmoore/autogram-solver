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

            // A nonempty conjunction assumes at least two rendered entries.
            // Match the renderer, which uses a plain separator join for whitespace conjunctions.
            var conjunction = string.IsNullOrWhiteSpace(autogramInput.Conjunction) ? string.Empty : autogramInput.Conjunction;
            var baselineString = (baselineTemplate + conjunction).ToLower();
            var specialChars = autogramInput.Alphabet.Where(p => p.HasExtendedName()).Select(p =>
                p.GetCharacterName(1, autogramInput.PluralSuffix) +
                p.GetCharacterName(2, autogramInput.PluralSuffix)
                ).ToList();

            // Ordinary characters' self-references do not make their counts variable.
            // Only number words, formatting, suffixes, and extended names can do so.
            var variableCharacters = Enumerable.Range(1, 99)
                .SelectMany(p => ((byte)p).ToCardinalNumberStringPrecomputed().ToLower())
                .ToHashSet();
            variableCharacters.Add(' ');
            variableCharacters.UnionWith(autogramInput.PluralSuffix);
            variableCharacters.UnionWith(autogramInput.SeparatorString);

            var relevantAlphabetArray = (
                baselineString +
                autogramInput.Forced +
                string.Concat(specialChars) +
                string.Concat(variableCharacters)).ToLower().Distinct().Where(autogramInput.Alphabet.Contains).OrderBy(p => GetOrder(p)).ToList();

            variableCharacters.UnionWith(relevantAlphabetArray.Where(p => p.HasExtendedName())
                .SelectMany(p => p.GetCharacterName(1, autogramInput.PluralSuffix) +
                    p.GetCharacterName(2, autogramInput.PluralSuffix)));

            var letters = relevantAlphabetArray.Select((p, i) => new CharacterConfig(autogramInput.SeparatorString, autogramInput.PluralSuffix)
            {
                Index = i,
                Char = p,
                Forced = autogramInput.Forced.ToLower().Contains(p),
                BaselineCount = baselineString.Count(c => c == p),
                IsVariable = variableCharacters.Contains(p),
            }).ToList();

            // Move the missing trailing separators into guaranteed entries, keeping
            // all baselines and lookup contributions nonnegative for byte solvers.
            // Only tracked separator characters require this correction.
            if (autogramInput.SeparatorString.Any(relevantAlphabetArray.Contains))
            {
                var omittedSeparators = string.IsNullOrWhiteSpace(autogramInput.Conjunction) ? 1 : 2;
                var guaranteedEntries = letters.Where(p => p.MinimumCount > 0)
                    .Take(omittedSeparators).ToList();
                if (guaranteedEntries.Count < omittedSeparators)
                {
                    throw new ArgumentException(
                        $"Counting separator characters requires at least {omittedSeparators} guaranteed list entries in the template or forced alphabet.",
                        nameof(autogramInput));
                }
                foreach (var letter in guaranteedEntries)
                {
                    letter.OmitSeparator = true;
                }
            }

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
                if (minimumCount == 0) continue;

                // Add the invariant's number word, plural suffix, and separator to the
                // minimum counts. The character itself was accounted for when its
                // MinimumCount was initialized.
                // Example: "seven a's" contributes two e's, two s's, one v, one n, etc.
                var numericCount = letter.StringRepresentationFor((byte)minimumCount, includeSelf: false)
                    .GetFrequencies(relevantAlphabetArray).ToByteArray();
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

            // Only guaranteed entries contribute a minimum. Preserve multiplicities:
            // both "comma" and "commas" contribute two m's, but only the plural has s.
            foreach (var letter in variableLetters.Where(p => p.Char.HasExtendedName() && p.MinimumCount > 0).ToList())
            {
                var singular = letter.Char.GetCharacterName(1, autogramInput.PluralSuffix);
                var plural = letter.Char.GetCharacterName(2, autogramInput.PluralSuffix);
                foreach (var target in letters)
                {
                    target.GuaranteedNameContribution += Math.Min(
                        singular.Count(c => c == target.Char),
                        plural.Count(c => c == target.Char));
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
