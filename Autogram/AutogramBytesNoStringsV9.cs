using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Autogram
{
    /// <summary>
    /// Based on version 5d, but .
    /// </summary>
    public class AutogramBytesNoStringsV9 : IAutogramFinder
    {
        private readonly HashSet<ByteHistoryKey64> history = [];
        private readonly Random random;

        private readonly byte[] proposedCounts;
        private readonly byte[] computedCounts;
        private readonly byte[] proposedCountsBackup;

        private readonly AutogramConfig config;

        private readonly int variableAlphabetCount;

        // Counts of chars that intersect with the chars that represent the numeric+plural
        private readonly byte[] variableBaselineCount; // counts of characters in the template and conjunction PLUS the cardinals of the invariant characters. 
        private readonly byte[][][] variableNumericCounts;
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] variableMinimumCount;

        public AutogramBytesNoStringsV9(
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
            proposedCountsBackup = new byte[proposedCounts.Length];
        }

        /// <summary>
        /// Iterates the autogram search process.
        /// </summary>
        /// <returns>The status of the current guess.</returns>
        public Status Iterate()
        {
            var depth = 0;

            char letter = config.VariableChars.ElementAt(depth).Char;
            Console.WriteLine("Examining letter " + letter);

            while (true)
            {
                var proposedCount = proposedCounts[depth];

                Console.Write($"\tTrying {proposedCount} of {letter}...");

                var isValid = IsValid(depth, proposedCount);

                Console.WriteLine(isValid ? "✅" : "❌");

                if (isValid)
                {
                    proposedCounts[depth]++;
                }
                else
                {
                    proposedCounts[depth]++;
                }
            }

            return new Status(false, false, false);
        }

        private bool IsValid(int position, byte proposedCount)
        {
            var counts = variableNumericCounts[position][proposedCount];

            for (int i = 0; i < variableBaselineCount.Length; i++)
            {
                var adjustment = (i == position) ? counts[i] - 1 : counts[i];
                if (i < position && adjustment > 0)
                {
                    return false;
                }
                if (i == position && variableMinimumCount[position] + adjustment > proposedCount)
                {
                    return false; // Console.WriteLine("❌");
                }
            }
            return true;
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
