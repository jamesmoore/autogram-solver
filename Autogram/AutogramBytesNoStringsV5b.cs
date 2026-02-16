using System.Diagnostics;
using Autogram.Comparer;

namespace Autogram
{
    /// <summary>
    /// Same as v5 except there is a 50/50 chance of doing a somewhat deterministic walk when Randomize is called.
    /// </summary>
    public class AutogramBytesNoStringsV5b : IAutogramFinder
    {
        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly Random random;

        private byte[] proposedCounts;
        private readonly byte[] computedCounts;

        private readonly AutogramConfig config;

        private readonly int variableAlphabetCount;

        // Counts of chars that intersect with the chars that represent the numeric+plural
        private readonly byte[] variableBaselineCount; // counts of characters in the template and conjunction PLUS the cardinals of the invariant characters. 
        private readonly byte[][][] variableNumericCounts;
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] variableMinimumCount;

        public AutogramBytesNoStringsV5b(
            AutogramConfig config,
            int? randomSeed)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            variableNumericCounts = config.GetVariableNumericCounts();

            var variableChars = config.VariableChars.ToList();
            variableAlphabetCount = variableChars.Count;
            variableBaselineCount = variableChars.Where(p => p.VariableBaselineCount.HasValue).Select(p => p.VariableBaselineCount!.Value).ToByteArray();
            variableMinimumCount = variableChars.Select(p => p.MinimumCount).ToByteArray();

            Debug.Assert(variableBaselineCount.Zip(variableMinimumCount).All(p => p.Second >= p.First));

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
                proposedCounts = random.Next(0, 2) == 0 ? RandomizeDiffs() : Randomize(); // 50/50 
                randomized = true;
            }
            else
            {
                // Only clone when we need to store in proposedCounts
                proposedCounts = (byte[])computedCounts.Clone();
            }

            history.Add(proposedCounts);

            UpdateComputedCounts();

            var reorderedEquals = computedCounts.AsSpan().UnorderedByteSpanEquals(proposedCounts);

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
            variableBaselineCount.CopyTo(computedCounts.AsSpan());

            for (var i = 0; i < variableAlphabetCount; i++)
            {
                var c = proposedCounts[i];
                if (c == 0) continue;

                var numericCount = variableNumericCounts[i][c];
                for (var j = 0; j < variableAlphabetCount; j++)
                {
                    computedCounts[j] += numericCount[j];
                }
            }

#if DEBUG
            for (var i = 0; i < variableAlphabetCount; i++)
            {
                Debug.Assert(computedCounts[i] >= variableMinimumCount[i]);
            }
#endif
        }

        // Special case Randomize that does an iterative history walk.
        // Some randomness is present to pick the initial mismatch index.
        private byte[] Randomize()
        {
            var result = new byte[proposedCounts.Length];
            Buffer.BlockCopy(computedCounts, 0, result, 0, computedCounts.Length);

            var adjustment = new sbyte[proposedCounts.Length];
            var adjustmentIndex = new byte[proposedCounts.Length]; // limited to mismatched
            var mismatchCount = 0;

            for (var i = 0; i < proposedCounts.Length; i++)
            {
                if (computedCounts[i] != proposedCounts[i])
                {
                    adjustmentIndex[mismatchCount++] = (byte)i;
                }
            }

            var mismatchIndex = random.Next(0, mismatchCount);

            while (true)
            {
                var realIndex = adjustmentIndex[mismatchIndex];

                var currentAdjustment = adjustment[realIndex];
                var newAdjustment = 0;
                if (currentAdjustment == 0) // Initially go upwards
                {
                    newAdjustment = 1;
                }
                else if (currentAdjustment < 0) // If negative go upwards
                {
                    newAdjustment = (currentAdjustment * -1) + 1;
                }
                else // if positive go negative - except if it would go below minimum
                {
                    newAdjustment = computedCounts[realIndex] - currentAdjustment < variableMinimumCount[realIndex]
                        ? currentAdjustment + 1
                        : currentAdjustment * -1;
                }

                adjustment[realIndex] = (sbyte)newAdjustment;

                result[realIndex] = (byte)(computedCounts[realIndex] + newAdjustment);

                if (history.Contains(result) == false)
                {
                    return result;
                }

                mismatchIndex++;
                if (mismatchIndex >= mismatchCount)
                {
                    mismatchIndex = 0;
                }
            }
        }

        // Regular randomization of non-matching counts
        private byte[] RandomizeDiffs()
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
                quantity.ToCardinalNumberStringPrecomputed() + " " + character.GetCharacterName(quantity);
        }

        public int HistoryCount => history.Count;
    }
}
