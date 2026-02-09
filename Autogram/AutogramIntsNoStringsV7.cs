using System.Diagnostics;
using Autogram.Comparer;

namespace Autogram
{
    public class AutogramIntsNoStringsV7 : IAutogramFinder
    {
        private readonly HashSet<int[]> history = new(new IntArraySpanComparer());
        private readonly Random random;

        private int[] proposedCounts;
        private readonly int[] computedCounts;

        private readonly AutogramConfig config;

        private readonly int variableAlphabetCount;

        // Counts of chars that intersect with the chars that represent the numeric+plural
        private readonly int[] variableBaselineCount; // counts of characters in the template and conjunction PLUS the cardinals of the invariant characters. 
        private readonly int[][][] variableNumericCounts;
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly int[] variableMinimumCount;

        public AutogramIntsNoStringsV7(
            AutogramConfig config,
            int? randomSeed)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            var variableNumericCountsBytes = config.GetVariableNumericCounts();
            variableNumericCounts = new int[variableNumericCountsBytes.Length][][];
            for (int i = 0; i < variableNumericCountsBytes.Length; i++)
            {
                var level2Bytes = variableNumericCountsBytes[i];
                var level2 = new int[level2Bytes.Length][];
                for (int j = 0; j < level2Bytes.Length; j++)
                {
                    var level3Bytes = level2Bytes[j];
                    var level3 = new int[level3Bytes.Length];
                    for (int k = 0; k < level3Bytes.Length; k++)
                    {
                        level3[k] = level3Bytes[k];
                    }
                    level2[j] = level3;
                }
                variableNumericCounts[i] = level2;
            }

            var variableChars = config.VariableChars.ToList();
            variableAlphabetCount = variableChars.Count;
            variableBaselineCount = variableChars.Where(p => p.VariableBaselineCount.HasValue).Select(p => p.VariableBaselineCount!.Value).ToArray();
            variableMinimumCount = variableChars.Select(p => p.MinimumCount).ToArray();

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
                proposedCounts = Randomize();
                randomized = true;
            }
            else
            {
                // Only clone when we need to store in proposedCounts
                proposedCounts = (int[])computedCounts.Clone();
            }

            history.Add(proposedCounts);

            UpdateComputedCounts();

            var reorderedEquals = computedCounts.AsSpan().UnorderedIntSpanEquals2(proposedCounts);

            if (reorderedEquals)
            {
                reorderedEquals = computedCounts.AsSpan().SequenceEqual(proposedCounts) == false;
                // Only clone if arrays have same content but different order
                if (reorderedEquals)
                {
                    proposedCounts = (int[])computedCounts.Clone();
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


        private int[] Randomize()
        {
            var result = new int[proposedCounts.Length];
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

        private int OffsetGuess(int actualCount, int minimumCount, int modifier)
        {
            int min = Math.Max(minimumCount, actualCount - modifier);
            int max = Math.Min(byte.MaxValue, actualCount + modifier + 1); // +1 because Random.Next is exclusive at upper bound
            return random.Next(min, max);
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
            var numberItems = relevantToVariableCharMap.Select(p => NumberToListEntry(p.Count, p.Char)).ToList();
            var arg0 = string.IsNullOrWhiteSpace(conjunction) ? numberItems.Listify(separator) : numberItems.ListifyWithConjunction(separator, conjunction);
            return string.Format(template, arg0);
        }

        private static string NumberToListEntry(int quantity, char character)
        {
            return quantity == 0 ?
                string.Empty :
                ((byte)quantity).ToCardinalNumberStringPrecomputed() + " " + (quantity == 1 ? character.GetCharacterName() : character.GetPluralisedCharacterName());
        }

        public int HistoryCount => history.Count;
    }
}
