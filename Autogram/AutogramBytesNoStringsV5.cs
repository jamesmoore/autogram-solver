using System.Diagnostics;
using Autogram.Comparer;

namespace Autogram
{
    public class AutogramBytesNoStringsV5 : IAutogramFinder
    {
        private sealed class StateSnapshot
        {
            public required byte[] ProposedCounts { get; set; }
            public required byte[] ComputedCounts { get; set; }
        }

        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly Random random;

        private readonly StateSnapshot state;

        private readonly AutogramConfig config;

        private readonly int variableAlphabetCount;

        // Counts of chars that intersect with the chars that represent the numeric+plural
        private readonly byte[] variableBaselineCount; // counts of characters in the template and conjunction PLUS the cardinals of the invariant characters. 
        private readonly byte[][][] variableNumericCounts;
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] variableMinimumCount;

        public AutogramBytesNoStringsV5(
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

            state = new StateSnapshot
            {
                ProposedCounts = variableMinimumCount.ToArray(),
                ComputedCounts = variableMinimumCount.ToArray()
            };
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
            if (history.Contains(state.ComputedCounts))
            {
                state.ProposedCounts = Randomize();
                randomized = true;
            }
            else
            {
                // Only clone when we need to store in proposedCounts
                state.ProposedCounts = (byte[])state.ComputedCounts.Clone();
            }

            history.Add(state.ProposedCounts);

            UpdateComputedCounts();

            var reorderedEquals = state.ComputedCounts.AsSpan().UnorderedByteSpanEquals(state.ProposedCounts);

            if (reorderedEquals)
            {
                reorderedEquals = state.ComputedCounts.AsSpan().SequenceEqual(state.ProposedCounts) == false;
                // Only clone if arrays have same content but different order
                if (reorderedEquals)
                {
                    state.ProposedCounts = (byte[])state.ComputedCounts.Clone();
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
            variableBaselineCount.CopyTo(state.ComputedCounts.AsSpan());

            for (var i = 0; i < variableAlphabetCount; i++)
            {
                var c = state.ProposedCounts[i];
                if (c == 0) continue;

                var numericCount = variableNumericCounts[i][c];
                for (var j = 0; j < variableAlphabetCount; j++)
                {
                    state.ComputedCounts[j] += numericCount[j];
                }
            }

#if DEBUG
            for (var i = 0; i < variableAlphabetCount; i++)
            {
                Debug.Assert(state.ComputedCounts[i] >= variableMinimumCount[i]);
            }
#endif
        }


        private byte[] Randomize()
        {
            var result = new byte[state.ProposedCounts.Length];
            var randomizationLevel = 1;
            while (true)
            {
                for (int i = 0; i < state.ProposedCounts.Length; i++)
                {
                    var computedCount = state.ComputedCounts[i];
                    result[i] = computedCount == state.ProposedCounts[i]
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
            var relevantToVariableCharMap = config.AllChars.Select(p => new {
                p.Char,
                Count = p.VariableIndex.HasValue ? state.ProposedCounts[p.VariableIndex.Value] : p.MinimumCount,
            }).Where(p => p.Count > 0);
            var numberItems = relevantToVariableCharMap.Select(p => p.Char.ToListEntry(p.Count)).ToList();
            var arg0 = string.IsNullOrWhiteSpace(conjunction) ? numberItems.Listify(separator) : numberItems.ListifyWithConjunction(separator, conjunction);
            return string.Format(template, arg0);
        }

        public int HistoryCount => history.Count;
    }
}
