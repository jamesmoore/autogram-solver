using Autogram.Comparer;

namespace Autogram
{
    public class AutogramBytesNoStrings : IAutogramFinder
    {
        private readonly string Template;
        private readonly string conjunction;
        private const string PluralExtension = "'s";
        private readonly IReadOnlyList<char> FullAlphabet;
        private Random random;

        private byte[] proposedCounts;
        private byte[] computedCounts;

        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly int? randomSeed;

        private readonly byte[] baselineCount;
        private readonly byte[][] numericCounts;
        private readonly byte[] pluralCount;

        private readonly int AlphabetCount;

        public AutogramBytesNoStrings(
            IEnumerable<char> alphabet,
            string template,
            string conjunction,
            int? randomSeed)
        {
            Template = template;
            this.conjunction = conjunction;
            FullAlphabet = alphabet.ToList();
            AlphabetCount = FullAlphabet.Count;
            var alphabetIndex = FullAlphabet.Select((c, i) => (c, i)).ToDictionary(ci => ci.c, ci => ci.i);

            proposedCounts = new byte[FullAlphabet.Count];
            computedCounts = new byte[FullAlphabet.Count];
            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            this.randomSeed = randomSeed;

            baselineCount = new byte[FullAlphabet.Count];
            foreach (var c in Template.Replace("{0}", "").ToLower())
            {
                if (alphabetIndex.TryGetValue(c, out int index))
                {
                    baselineCount[index]++;
                }
            }

            numericCounts = new byte[100][];
            for (byte i = 0; i < 100; i++)
            {
                var text = i.ToCardinalNumberStringPrecomputed();
                var perCardinalCount = new byte[FullAlphabet.Count];
                foreach (var c in text)
                {
                    if (alphabetIndex.TryGetValue(c, out int index))
                    {
                        perCardinalCount[index]++;
                    }
                }
                numericCounts[i] = perCardinalCount;
            }

            pluralCount = new byte[FullAlphabet.Count];
            foreach (var c in PluralExtension)
            {
                if (alphabetIndex.TryGetValue(c, out int index))
                {
                    pluralCount[index]++;
                }
            }

            if (string.IsNullOrWhiteSpace(conjunction) == false)
            {
                // add conjunction to baseline on the basis that there will almost certainly be more than one characters listed.
                foreach (var c in conjunction)
                {
                    if (alphabetIndex.TryGetValue(c, out int index))
                    {
                        baselineCount[index]++;
                    }
                }
            }
        }

        /// <summary>
        /// Resets state, and optionally random. Primarily for benchmarking.
        /// </summary>
        /// <param name="resetRandom">If true the random state is reset.</param>
        public void Reset(bool resetRandom = true)
        {
            proposedCounts = new byte[FullAlphabet.Count];
            computedCounts = new byte[FullAlphabet.Count];
            if (resetRandom)
            {
                random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
            }
            history.Clear();
        }

        private byte[] Randomize()
        {
            var result = new byte[proposedCounts.Length];
            for (int i = 0; i < proposedCounts.Length; i++)
            {
                result[i] = computedCounts[i] == proposedCounts[i]
                    ? proposedCounts[i]
                    : PerturbCount(computedCounts[i]);
            }
            return result;
        }

        private byte PerturbCount(byte acutalCount)
        {
            var nextGuess = acutalCount + random.Next(6) - 3;
            if (nextGuess < 0) nextGuess = 0;
            return (byte)nextGuess;
        }

        /// <summary>
        /// Builds the sentence based off the current state, and may not be a valid autogram.
        /// </summary>
        /// <returns>The current sentence.</returns>
        public override string ToString()
        {
            var numberItems = proposedCounts.Select((p, index) => p == 0 ? string.Empty : p.ToCardinalNumberStringPrecomputed() + " " + FullAlphabet[index] + (p == 1 ? "" : PluralExtension)).Where(p => string.IsNullOrWhiteSpace(p) == false).ToList();
            var arg0 = string.IsNullOrWhiteSpace(conjunction) ? numberItems.Listify() : numberItems.ListifyWithConjunction(conjunction);
            return string.Format(Template, arg0);
        }

        private byte[] GetActualCounts(byte[] currentGuess)
        {
            Span<byte> result = stackalloc byte[AlphabetCount];
            baselineCount.CopyTo(result);

            for (var i = 0; i < AlphabetCount; i++)
            {
                var c = currentGuess[i];
                if (c == 0) continue;

                // numeric part
                var numericCount = numericCounts[c];
                for (var j = 0; j < AlphabetCount; j++)
                {
                    result[j] += numericCount[j];
                }

                // actual letter
                result[i]++;

                if (c != 1)
                {
                    // plural
                    for (var j = 0; j < AlphabetCount; j++)
                    {
                        result[j] += pluralCount[j];
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Iterates the autogram search process.
        /// </summary>
        /// <returns>The status of the current guess.</returns>
        public Status Iterate()
        {
            var nextGuess = AdjustGuessTowardsActualCounts();
            var randomReset = false;
            if (history.Contains(nextGuess))
            {
                proposedCounts = Randomize();
                randomReset = true;
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

            return new Status()
            {
                CurrentGuess = proposedCounts,
                ActualCounts = computedCounts,
                Success = success,
                HistoryCount = history.Count,
                RandomReset = randomReset,
                Reordered = reorderedEquals,
            };
        }

        private byte[] AdjustGuessTowardsActualCounts()
        {
            var result = new byte[computedCounts.Length];
            for (int i = 0; i < computedCounts.Length; i++)
            {
                result[i] = GuessAgain(computedCounts[i], proposedCounts[i]);
            }
            return result;
        }

        private static byte GuessAgain(byte actualCount, byte currentGuess)
        {
            if (actualCount == currentGuess)
            {
                return actualCount;
            }
            else if ((actualCount + currentGuess) % 2 == 0)
            {
                return (byte)((actualCount + currentGuess) / 2);
            }
            else
            {
                var v = actualCount + currentGuess;
                var increment = (v - 1) % 2 == 0 ? 1 : 0;
                return (byte)((v + 1) / 2);
            }
        }
    }
}
