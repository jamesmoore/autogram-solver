using System.Diagnostics;
using Autogram.Comparer;

namespace Autogram
{
    public class AutogramBytesNoStringsV4 : IAutogramFinder
    {
        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly Random random;

        private byte[] proposedCounts;
        private readonly byte[] computedCounts;

        private readonly AutogramConfig config;

        private readonly int variableAlphabetCount;

        // Minimum counts required for template, conjunction, plural and the corresponding cardinals.
        // This will be used for the cardinal counts of the invariant characters.
        private readonly byte[] minimumCount;

        // Counts of chars that intersect with the chars that represent the numeric+plural
        private readonly byte[] variableBaselineCount; // counts of characters in the template and conjunction PLUS the cardinals of the invariant characters. 
        private readonly byte[][] variableNumericCounts; // counts of characters per cardnal number plus plural if applicable
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] variableMinimumCount;

        private readonly bool[] includeSelfInCount;

        public AutogramBytesNoStringsV4(
            AutogramConfig config,
            int? randomSeed)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            variableNumericCounts = config.VariableNumericCounts.ToArray();

            // minimum count is baseline + 1 if present, to account for the character itself in the list.
            minimumCount = config.Letters.Select(p => p.MinimumCount).ToByteArray();

            var variableChars = config.Letters.Where(p => p.IsVariable).ToList();
            variableAlphabetCount = variableChars.Count;
            variableBaselineCount = variableChars.Where(p => p.VariableBaselineCount.HasValue).Select(p => p.VariableBaselineCount!.Value).ToByteArray();
            variableMinimumCount = variableChars.Select(p => p.MinimumCount).ToByteArray();
            includeSelfInCount = variableChars.Select(p => p.IncludeSelfInCount).ToArray();

            Debug.Assert(variableBaselineCount.Zip(variableMinimumCount).All(p => p.Second >= p.First));

            proposedCounts = variableBaselineCount.ToArray();
            computedCounts = variableBaselineCount.ToArray();
            UpdateComputedCounts(proposedCounts);
        }

        /// <summary>
        /// Iterates the autogram search process.
        /// </summary>
        /// <returns>The status of the current guess.</returns>
        public Status Iterate()
        {
            var nextGuess = AdjustGuessTowardsActualCounts();
            var randomized = false;
            if (history.Contains(nextGuess))
            {
                proposedCounts = Randomize();
                randomized = true;
            }
            else
            {
                proposedCounts = nextGuess;
            }

            history.Add(proposedCounts);

            UpdateComputedCounts(proposedCounts);

            var reorderedEquals = ((ReadOnlySpan<byte>)computedCounts.AsSpan()).UnorderedByteSpanEquals(proposedCounts);

            if (reorderedEquals)
            {
                reorderedEquals = computedCounts.AsSpan().SequenceEqual(proposedCounts) == false;
                proposedCounts = computedCounts.ToArray();
                return new Status(true, randomized, reorderedEquals);
            }
            else
            {
                return new Status(false, randomized, reorderedEquals);
            }
        }

        private void UpdateComputedCounts(byte[] currentGuess)
        {
            variableBaselineCount.CopyTo(computedCounts.AsSpan());

            for (var i = 0; i < variableAlphabetCount; i++)
            {
                var c = currentGuess[i];
                if (c == 0) continue;

                // numeric + plural part
                var numericCount = variableNumericCounts[c];
                for (var j = 0; j < variableAlphabetCount; j++)
                {
                    computedCounts[j] += numericCount[j];
                }

                // actual letter - for commas, hyphens and apostrophes we don't want to include the char itself.
                if (includeSelfInCount[i])
                {
                    computedCounts[i]++;
                }
            }


#if DEBUG
            for (var i = 0; i < variableAlphabetCount; i++)
            {
                Debug.Assert(computedCounts[i] >= variableMinimumCount[i]);
            }
#endif
        }

        private byte[] AdjustGuessTowardsActualCounts()
        {
            var result = (byte[])computedCounts.Clone();
#if DEBUG
            for (int i = 0; i < computedCounts.Length; i++)
            {
                Debug.Assert(result[i] >= variableMinimumCount[i]);
            }
#endif
            return result;
        }

        private byte[] Randomize()
        {
            var result = new byte[proposedCounts.Length];
            var randomizationLevel = 1;
            while (true)
            {
                for (int i = 0; i < proposedCounts.Length; i++)
                {
                    var computedCount = computedCounts[i];
                    result[i] = computedCount == proposedCounts[i]
                        ? computedCount
                        : OffsetGuess(computedCount, variableMinimumCount[i], randomizationLevel);
                }

                if (history.Contains(result) == false)
                {
                    return result;
                }

                randomizationLevel++;
            }
        }

        private byte OffsetGuess(byte actualCount, byte minimumCount, int modifier)
        {
            int min = Math.Max(minimumCount, actualCount - modifier);
            int max = Math.Min(byte.MaxValue, actualCount + modifier + 1); // +1 because Random.Next is exclusive at upper bound
            return (byte)random.Next(min, max);
        }

        /// <summary>
        /// Builds the sentence based off the current state, and may not be a valid autogram.
        /// </summary>
        /// <returns>The current sentence.</returns>
        public override string ToString()
        {
            var relevantToVariableCharMap = config.Letters.Select(p => new {
                p.Char,
                Count = p.VariableIndex.HasValue ? proposedCounts[p.VariableIndex.Value] : p.MinimumCount,
            }).Where(p => p.Count > 0);
            var numberItems = relevantToVariableCharMap.Select(p => NumberToListEntry((byte)p.Count, p.Char)).ToList();
            var arg0 = string.IsNullOrWhiteSpace(config.Conjunction) ? numberItems.Listify() : numberItems.ListifyWithConjunction(config.Conjunction);
            return string.Format(config.Template, arg0);
        }

        private static string NumberToListEntry(byte quantity, char character)
        {
            return quantity == 0 ?
                string.Empty :
                quantity.ToCardinalNumberStringPrecomputed() + " " + (quantity == 1 ? character.GetCharacterName() : character.GetPluralisedCharacterName());
        }

        public int HistoryCount => history.Count;
    }
}
