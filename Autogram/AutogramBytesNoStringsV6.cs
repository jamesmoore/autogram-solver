using Autogram.Comparer;
using System.Diagnostics;

namespace Autogram
{
    public class AutogramBytesNoStringsV6 : IAutogramFinder
    {
        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly Random random;

        private byte[] proposedCounts;
        private readonly byte[] computedCounts;

        private readonly AutogramConfig config;

        private readonly int variableAlphabetCount;

        // Counts of chars that intersect with the chars that represent the numeric+plural
        private readonly byte[] variableBaselineCount; // counts of characters in the template and conjunction PLUS the cardinals of the invariant characters. 


        // First level = letter in question(eg "a")
        // Second level = quantity of those letters(eg, 3 for 3 "a's")
        // Third level = quantity of letters in that number(eg 3 => 1xt, 1xh, 1xr, 2xe)
        private readonly byte[][][] variableNumericCounts;
        private readonly byte[] variableNumericCountsFlattened;

        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] variableMinimumCount;

        public AutogramBytesNoStringsV6(
            AutogramConfig config,
            int? randomSeed)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            variableNumericCounts = config.GetVariableNumericCounts();
            variableNumericCountsFlattened = variableNumericCounts.SelectMany(p => p).SelectMany(p => p).ToArray();


            var variableChars = config.VariableChars.ToList();
            variableAlphabetCount = variableChars.Count;
            variableBaselineCount = variableChars.Where(p => p.VariableBaselineCount.HasValue).Select(p => p.VariableBaselineCount!.Value).ToByteArray();
            variableMinimumCount = variableChars.Select(p => p.MinimumCount).ToByteArray();

            Debug.Assert(variableBaselineCount.Zip(variableMinimumCount).All(p => p.Second >= p.First));

            for (int i = 0; i < variableNumericCounts.Length; i++)
            {
                int offset = i * 100 * variableAlphabetCount;
                for (int k = 0; k < variableNumericCounts.Length; k++)
                {
                    variableNumericCountsFlattened[offset + k] = 0;
                }
            }
            proposedCounts = variableMinimumCount.ToArray();
            computedCounts = variableMinimumCount.ToArray();
            UpdateComputedCounts();
        }

        /// <summary>
        /// Iterates the autogram search process.
        /// </summary>
        /// <returns>The status of the current guess.</returns>
        public Status Iterate()
        {
            var randomized = false;

            // Check if computedCounts is already in history before cloning
            if (history.Contains(computedCounts))
            {
                proposedCounts = Randomize();
                randomized = true;
            }
            else
            {
                // Only clone when we need to store in proposedCounts
                proposedCounts = (byte[])computedCounts.Clone();
            }

            history.Add(proposedCounts);

            UpdateComputedCounts();

            var reorderedEquals = ((ReadOnlySpan<byte>)computedCounts.AsSpan()).UnorderedByteSpanEquals(proposedCounts);

            if (reorderedEquals)
            {
                reorderedEquals = computedCounts.AsSpan().SequenceEqual(proposedCounts) == false;
                // Only clone if arrays have same content but different order
                if (reorderedEquals)
                {
                    proposedCounts = (byte[])computedCounts.Clone();
                }

                return new Status(true, randomized, reorderedEquals);
            }
            else
            {
                return new Status(false, randomized, reorderedEquals);
            }
        }

        private void UpdateComputedCounts()
        {
            for (var i = 0; i < variableAlphabetCount; i++)
            {
                computedCounts[i] = variableBaselineCount[i];
            }
            for (var i = 0; i < variableAlphabetCount; i++)
            {
                var c = proposedCounts[i];
                int startIdx = (i * 100 + c) * variableAlphabetCount;

                for (var j = 0; j < variableAlphabetCount; j++)
                {
                    computedCounts[j] += variableNumericCountsFlattened[startIdx + j];
                }
            }

#if DEBUG
            for (var i = 0; i < variableAlphabetCount; i++)
            {
                Debug.Assert(computedCounts[i] >= variableMinimumCount[i]);
            }
#endif
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
        public string ToString(string template, string conjunction, string separator)
        {
            var relevantToVariableCharMap = config.AllChars.Select(p => new
            {
                p.Char,
                Count = p.VariableIndex.HasValue ? proposedCounts[p.VariableIndex.Value] : p.MinimumCount,
            }).Where(p => p.Count > 0);
            var numberItems = relevantToVariableCharMap.Select(p => NumberToListEntry((byte)p.Count, p.Char)).ToList();
            var arg0 = string.IsNullOrWhiteSpace(conjunction) ? numberItems.Listify(separator) : numberItems.ListifyWithConjunction(separator, conjunction);
            return string.Format(template, arg0);
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
