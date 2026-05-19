using System.Diagnostics;

namespace Autogram
{
    /// <summary>
    /// Based on version 5c, but reuses proposed counts array.
    /// </summary>
    public class AutogramBytesNoStringsV5d : IAutogramFinder
    {
        private readonly HashSet<ByteHistoryKey64> history = [];
        private readonly Random random;

        private readonly byte[] proposedCounts;
        private readonly byte[] computedCounts;
        private readonly byte[] diffCount;

        private readonly AutogramConfig config;

        private readonly int variableAlphabetCount;

        // Counts of chars that intersect with the chars that represent the numeric+plural
        private readonly byte[] variableBaselineCount; // counts of characters in the template and conjunction PLUS the cardinals of the invariant characters. 
        private readonly byte[][][] variableNumericCounts;
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] variableMinimumCount;

        public AutogramBytesNoStringsV5d(
            AutogramConfig config,
            int? randomSeed)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            variableNumericCounts = config.GetVariableNumericCounts();

            var variableChars = config.VariableChars.ToList();
            variableAlphabetCount = variableChars.Count;
            ArgumentOutOfRangeException.ThrowIfGreaterThan(variableAlphabetCount, ByteHistoryKey64.MaxLength);
            variableBaselineCount = variableChars.Where(p => p.VariableBaselineCount.HasValue).Select(p => p.VariableBaselineCount!.Value).ToByteArray();
            variableMinimumCount = variableChars.Select(p => p.MinimumCount).ToByteArray();

            Debug.Assert(variableBaselineCount.Zip(variableMinimumCount).All(p => p.Second >= p.First));

            proposedCounts = variableMinimumCount.ToArray();
            computedCounts = variableMinimumCount.ToArray();
            diffCount = new byte[proposedCounts.Length];
            UpdateComputedCounts();
        }

        /// <summary>
        /// Iterates the autogram search process.
        /// </summary>
        /// <returns>The status of the current guess.</returns>
        public Status Iterate()
        {
            var randomized = false;
            var computedKey = new ByteHistoryKey64(computedCounts);

            if (history.Contains(computedKey))
            {
                Randomize();
                randomized = true;
            }
            else
            {
                computedCounts.CopyTo(proposedCounts);
                history.Add(new ByteHistoryKey64(proposedCounts));
            }

            UpdateComputedCounts();

            var reorderedEquals = computedCounts.AsSpan().UnorderedByteSpanEquals(proposedCounts);

            if (reorderedEquals)
            {
                reorderedEquals = computedCounts.AsSpan().SequenceEqual(proposedCounts) == false;
                if (reorderedEquals)
                {
                    computedCounts.CopyTo(proposedCounts);
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



        private void Randomize()
        {
            for(int i = 0; i < proposedCounts.Length; i++)
            {
                diffCount[i] = proposedCounts[i] == computedCounts[i] ? (byte)0 : (byte)1;
            }

            var randomizationLevel = 1;
            while (true)
            {
                for (int i = 0; i < proposedCounts.Length; i++)
                {
                    var computedCount = computedCounts[i];
                    proposedCounts[i] = diffCount[i] == 0
                        ? computedCount
                        : OffsetGuess(computedCount, variableMinimumCount[i], randomizationLevel);
                }

                ByteHistoryKey64 proposedKey = new(proposedCounts);
                if (history.Contains(proposedKey) == false)
                {
                    history.Add(proposedKey);
                    return;
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

        public AutogramSnapshot GetAutogramSnapshot()
        {
            return new AutogramSnapshot(config.AllChars.Select(p => (
                p.Char,
                Count: p.VariableIndex.HasValue ? (int)proposedCounts[p.VariableIndex.Value] : p.MinimumCount
            )).Where(p => p.Count > 0));
        }

        public int HistoryCount => history.Count;
    }
}
