using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Autogram
{
    /// <summary>
    /// Based on version 5i, but uses a Vector512<byte> for optimized addition.
    /// This will only use the Vector512<byte>. If the length of the variable counts is less than 64 then it will use Vector512<byte> with the remaining bytes set to 0. 
    /// If the length of the variable counts is greater than 64 throw an exception in the constructor.
    /// </summary>
    public class AutogramVector512 : IAutogramFinder
    {
        private readonly HashSet<ByteHistoryKey64> history = [];
        private readonly Random random;

        private readonly byte[] proposedCounts;
        private readonly byte[] computedCounts;
        private readonly byte[] proposedCountsBackup;

        private readonly AutogramConfig config;

        private readonly int variableAlphabetCount;
        private readonly int variableNumericCountStride;

        // Counts of chars that intersect with the chars that represent the numeric+plural
        private readonly byte[] variableBaselineCount; // counts of characters in the template and conjunction PLUS the cardinals of the invariant characters. 
        private readonly Vector512<byte>[] variableNumericCountsVectors; // Flattened from [a][b][c] to [a*b][c]
        private readonly Vector512<byte> variableBaselineCountVector;
        private readonly byte[] computedCountsVectorBuffer;
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] variableMinimumCount;

        public AutogramVector512(
            AutogramConfig config,
            int? randomSeed)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            var variableChars = config.VariableChars.ToList();
            variableAlphabetCount = variableChars.Count;
            ArgumentOutOfRangeException.ThrowIfGreaterThan(variableAlphabetCount, ByteHistoryKey64.MaxLength);

            var variableNumericCounts = config.GetVariableNumericCounts();
            var variableNumericCountsFlattened = variableNumericCounts.SelectMany(p => p).ToArray();
            variableNumericCountStride = variableNumericCounts.FirstOrDefault()?.Length ?? 0;

            variableBaselineCount = variableChars.Where(p => p.VariableBaselineCount.HasValue).Select(p => p.VariableBaselineCount!.Value).ToByteArray();
            variableMinimumCount = variableChars.Select(p => p.MinimumCount).ToByteArray();
            variableBaselineCountVector = CreateVector(variableBaselineCount);
            variableNumericCountsVectors = variableNumericCountsFlattened.Select(p => CreateVector(p)).ToArray();
            computedCountsVectorBuffer = new byte[Vector512<byte>.Count];

            Debug.Assert(variableBaselineCount.Zip(variableMinimumCount).All(p => p.Second >= p.First));

            proposedCounts = variableMinimumCount.ToArray();
            computedCounts = variableMinimumCount.ToArray();
            proposedCountsBackup = new byte[proposedCounts.Length];
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

            var reorderedEquals = computedCounts.AsSpan().UnorderedByteSpanEqualsWithSum(proposedCounts);

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
            var computedCountVector = variableBaselineCountVector;

            for (var i = 0; i < variableAlphabetCount; i++)
            {
                var c = proposedCounts[i];
                if (c == 0) continue;

                computedCountVector = Vector512.Add(computedCountVector, variableNumericCountsVectors[(i * variableNumericCountStride) + c]);
            }

            computedCountVector.CopyTo(computedCountsVectorBuffer);
            computedCountsVectorBuffer.AsSpan(0, variableAlphabetCount).CopyTo(computedCounts);

#if DEBUG
            for (var i = 0; i < variableAlphabetCount; i++)
            {
                Debug.Assert(computedCounts[i] >= variableMinimumCount[i]);
            }
#endif
        }

        private static Vector512<byte> CreateVector(ReadOnlySpan<byte> values)
        {
            Span<byte> buffer = stackalloc byte[Vector512<byte>.Count];
            values.CopyTo(buffer);
            return MemoryMarshal.Cast<byte, Vector512<byte>>(buffer)[0];
        }

        private void Randomize()
        {
            proposedCounts.CopyTo(proposedCountsBackup);

            var randomizationLevel = 1;
            while (true)
            {
                for (int i = 0; i < proposedCounts.Length; i++)
                {
                    var computedCount = computedCounts[i];
                    proposedCounts[i] = proposedCountsBackup[i] == computedCount
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
