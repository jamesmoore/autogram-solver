using System.Diagnostics;
using Autogram.Comparer;

namespace Autogram
{
    public class AutogramBytesNoStringsV3 : IAutogramFinder
    {
        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly int? randomSeed;
        private Random random;

        private byte[] proposedCounts;
        private byte[] computedCounts;

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

        public AutogramBytesNoStringsV3(
            AutogramConfig config,
            int? randomSeed)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
            this.randomSeed = randomSeed;

            variableNumericCounts = config.VariableNumericCounts.ToArray();

            // minimum count is baseline + 1 if present, to account for the character itself in the list.
            minimumCount = config.Letters.Select(p => p.MinimumCount).ToByteArray();

            variableAlphabetCount = config.Letters.Where(p => p.IsVariable).Count();

            variableBaselineCount = config.Letters.Where(p => p.IsVariable).Select(p => p.VariableBaselineCount.Value).ToByteArray();
            variableMinimumCount = config.Letters.Where(p => p.IsVariable).Select(p => p.MinimumCount).ToByteArray();

            Debug.Assert(variableBaselineCount.Zip(variableMinimumCount).All(p => p.Second >= p.First));

            proposedCounts = variableBaselineCount.ToArray();
            computedCounts = GetActualCounts(proposedCounts);
        }

        private byte[] Randomize()
        {
            var result = new byte[proposedCounts.Length];
            for (int i = 0; i < proposedCounts.Length; i++)
            {
                var computedCount = computedCounts[i];
                result[i] = computedCount == proposedCounts[i]
                    ? computedCount
                    : OffsetGuess(computedCount, variableMinimumCount[i]);
            }
            return result;
        }

        private byte OffsetGuess(byte actualCount, byte minimumCount)
        {
            var nextGuess = actualCount + random.Next(6) - 3;
            if (nextGuess < minimumCount) nextGuess = minimumCount;
            return (byte)nextGuess;
        }

        /// <summary>
        /// Builds the sentence based off the current state, and may not be a valid autogram.
        /// </summary>
        /// <returns>The current sentence.</returns>
        public override string ToString()
        {
            var RelevantToVariableCharMap = config.Letters.ToDictionary(p => p.Char, p => p.VariableIndex); //   relevantAlphabet.ToDictionary(p => p, p => variableAlphabet.Contains(p) ? variableAlphabet.IndexOf(p) : (int?)null);
            var numberItems = RelevantToVariableCharMap.Select((p, index) => NumberToListEntry(p.Value == null ? minimumCount[index] : proposedCounts[p.Value.Value], p.Key, config.PluralExtension)).Where(p => string.IsNullOrWhiteSpace(p) == false).ToList();
            var arg0 = string.IsNullOrWhiteSpace(config.Conjunction) ? numberItems.Listify() : numberItems.ListifyWithConjunction(config.Conjunction);
            return string.Format(config.Template, arg0);
        }

        private static string NumberToListEntry(byte quantity, char character, string pluralExtension)
        {
            return quantity == 0 ? string.Empty : quantity.ToCardinalNumberStringPrecomputed() + " " + character + (quantity == 1 ? "" : pluralExtension);
        }

        private byte[] GetActualCounts(byte[] currentGuess)
        {
            Span<byte> result = stackalloc byte[variableAlphabetCount];
            variableBaselineCount.CopyTo(result);

            for (var i = 0; i < variableAlphabetCount; i++)
            {
                var c = currentGuess[i];
                if (c == 0) continue;

                // numeric + plural part
                var numericCount = variableNumericCounts[c];
                for (var j = 0; j < variableAlphabetCount; j++)
                {
                    result[j] += numericCount[j];
                }

                // actual letter
                result[i]++;
            }

#if DEBUG
            for (var i = 0; i < variableAlphabetCount; i++)
            {
                Debug.Assert(result[i] >= variableMinimumCount[i]);
            }
#endif
            return result.ToArray();
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
                //do
                //{
                    proposedCounts = Randomize();
                //} while (history.Contains(proposedCounts));
                randomized = true;
            }
            else
            {
                proposedCounts = nextGuess;
            }

            history.Add(proposedCounts);

            computedCounts = GetActualCounts(proposedCounts);

            var reorderedEquals = ((ReadOnlySpan<byte>)computedCounts.AsSpan()).UnorderedByteSpanEquals(proposedCounts);

            if (reorderedEquals)
            {
                proposedCounts = computedCounts.ToArray();
            }

            bool success = computedCounts.AsSpan().SequenceEqual(proposedCounts);

            return new Status(success, history.Count, randomized, reorderedEquals);
        }

        private byte[] AdjustGuessTowardsActualCounts()
        {
            var result = new byte[computedCounts.Length];
            for (int i = 0; i < computedCounts.Length; i++)
            {
                result[i] = computedCounts[i];
                Debug.Assert(result[i] >= variableMinimumCount[i]);
            }
            return result;
        }
    }
}
