using System.Diagnostics;
using Autogram.Comparer;

namespace Autogram
{
    public class AutogramBytesNoStringsV8 : IAutogramFinder
    {
        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly Random random;

        private byte[] proposedCounts;
        private readonly byte[] lastProposedCounts;
        private readonly byte[] computedCounts;

        private readonly AutogramConfig config;

        private readonly int variableAlphabetCount;

        // Counts of chars that intersect with the chars that represent the numeric+plural
        private readonly byte[] variableBaselineCount; // counts of characters in the template and conjunction PLUS the cardinals of the invariant characters. 
        private readonly byte[][][] variableNumericCounts;
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] variableMinimumCount;

        public AutogramBytesNoStringsV8(
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
            lastProposedCounts = new byte[proposedCounts.Length];
            computedCounts = ReCalculateComputedCounts();
        }

        /// <summary>
        /// Iterates the autogram search process.
        /// </summary>
        /// <returns>The status of the current guess.</returns>
        public Status Iterate()
        {
            var randomized = false;

            proposedCounts.CopyTo(lastProposedCounts.AsSpan());

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

            DeltaUpdateComputedCounts();

            var reorderedEquals = computedCounts.AsSpan().UnorderedByteSpanEquals(proposedCounts);

            if (reorderedEquals)
            {
                var wasReordered = computedCounts.AsSpan().SequenceEqual(proposedCounts) == false;
                // Only clone if arrays have same content but different order
                if (wasReordered)
                {
                    proposedCounts = (byte[])computedCounts.Clone();
                }
                return new Status(true, randomized, wasReordered);
            }
            else
            {
                return new Status(false, randomized, reorderedEquals);
            }
        }

        private void DeltaUpdateComputedCounts()
        {
            var alphabetCount = variableAlphabetCount;
            var computed = computedCounts;
            var vnc = variableNumericCounts;

            for (int i = 0; i < alphabetCount; i++)
            {
                byte lastCount = lastProposedCounts[i];
                byte currentCount = proposedCounts[i];
                if (lastCount == currentCount) continue;

                if (currentCount == 0)
                {
                    var toDeduct = vnc[i][lastCount];
                    for (int j = 0; j < alphabetCount; j++)
                    {
                        computed[j] = (byte)(computed[j] - toDeduct[j]);
                    }
                }
                else if (lastCount == 0)
                {
                    var toAdd = vnc[i][currentCount];
                    for (int j = 0; j < alphabetCount; j++)
                    {
                        computed[j] = (byte)(computed[j] + toAdd[j]);
                    }
                }
                else
                {
                    var toAdd = vnc[i][currentCount];
                    var toDeduct = vnc[i][lastCount];
                    for (int j = 0; j < alphabetCount; j++)
                    {
                        computed[j] = (byte)(computed[j] + toAdd[j] - toDeduct[j]);
                    }
                }
            }

#if DEBUG
            var test = ReCalculateComputedCounts();

            Debug.Assert(test.SequenceEqual(computedCounts));

            for (var i = 0; i < variableAlphabetCount; i++)
            {
                Debug.Assert(computedCounts[i] >= variableMinimumCount[i]);
            }
#endif
        }

        private byte[] ReCalculateComputedCounts()
        {
            var newCount = new byte[variableAlphabetCount];
            variableBaselineCount.CopyTo(newCount.AsSpan());

            for (var i = 0; i < variableAlphabetCount; i++)
            {
                var c = proposedCounts[i];
                if (c == 0) continue;

                var numericCount = variableNumericCounts[i][c];
                for (var j = 0; j < variableAlphabetCount; j++)
                {
                    newCount[j] += numericCount[j];
                }
            }

#if DEBUG
            for (var i = 0; i < variableAlphabetCount; i++)
            {
                Debug.Assert(newCount[i] >= variableMinimumCount[i]);
            }
#endif
            return newCount;
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
