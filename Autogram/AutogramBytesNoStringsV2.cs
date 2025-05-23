using System.Diagnostics;
using Autogram.Comparer;

namespace Autogram
{
    public class AutogramBytesNoStringsV2 : IAutogramFinder
    {
        private readonly string Template;
        private readonly string conjunction;
        private const string PluralExtension = "'s";

        private readonly IReadOnlyList<char> RelevantAlphabet;
        private readonly int RelevantAlphabetCount;

        private readonly IReadOnlyList<char> VariableAlphabet;
        private readonly int VariableAlphabetCount;

        private Random random;

        private byte[] proposedCounts;
        private byte[] computedCounts;

        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly int? randomSeed;

        private readonly byte[] baselineCount; // counts of characters in the template and conjunction
        private readonly byte[][] numericCounts; // counts of characters per cardnal number plus plural if applicable
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] minimumCount;

        private readonly byte[] variableBaselineCount; // counts of characters in the template and conjunction
        private readonly byte[][] variableNumericCounts; // counts of characters per cardnal number plus plural if applicable
        // Minimum counts required for template, conjunction and the letter list that represents them.
        // This will be used as the initial guess, and a lower limit for guesses.
        private readonly byte[] variableMinimumCount;

        public AutogramBytesNoStringsV2(
            IEnumerable<char> alphabet,
            string template,
            string conjunction,
            int? randomSeed)
        {
            Template = template;
            var baselineTemplate = string.Format(template, "");

            this.conjunction = conjunction;

            var numericStrings = Enumerable.Range(0, 100).Select(p => ((byte)p).ToCardinalNumberStringPrecomputed()).ToList();

            RelevantAlphabet = (baselineTemplate + conjunction + PluralExtension + numericStrings.Skip(1).Aggregate((p, q) => p + q)).ToLower().Where(alphabet.Contains).Distinct().OrderBy(p => p).ToList();
            RelevantAlphabetCount = RelevantAlphabet.Count();
            var alphabetIndex = RelevantAlphabet.Select((c, i) => (c, i)).ToDictionary(ci => ci.c, ci => ci.i);

            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            this.randomSeed = randomSeed;

            baselineCount = new byte[RelevantAlphabetCount];
            foreach (var c in baselineTemplate.ToLower())
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
                var perCardinalCount = new byte[RelevantAlphabetCount];
                foreach (var c in text)
                {
                    if (alphabetIndex.TryGetValue(c, out int index))
                    {
                        perCardinalCount[index]++;
                    }
                }

                if (i != 1)
                {
                    foreach (var c in PluralExtension)
                    {
                        if (alphabetIndex.TryGetValue(c, out int index))
                        {
                            perCardinalCount[index]++;
                        }
                    }
                }

                numericCounts[i] = perCardinalCount;
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

            minimumCount = new byte[RelevantAlphabetCount];
            for (int i = 0; i < RelevantAlphabetCount; i++)
            {
                minimumCount[i] = baselineCount[i] > 0 ? (byte)(baselineCount[i] + 1) : (byte)0;
            }

            var tmpVariableCharAlphabet = new List<char>();
            var tmpVariableBaselineCount = new List<byte>();
            var tmpVariableNumericCount = numericCounts.Select(p => new List<byte>()).ToList();

            var map = new Dictionary<int, int?>();

            for (int i = 0; i < RelevantAlphabetCount; i++)
            {
                var isVariableChar = numericCounts.Skip(1).Any(p => p[i] > 0); // skip zero which should never be output.

                map.Add(i, isVariableChar ? tmpVariableCharAlphabet.Count : null);

                if (isVariableChar)
                {
                    tmpVariableCharAlphabet.Add(RelevantAlphabet[i]);
                    tmpVariableBaselineCount.Add(baselineCount[i]);
                    for (int j = 0; j < tmpVariableNumericCount.Count; j++)
                    {
                        tmpVariableNumericCount[j].Add(numericCounts[j][i]);
                    }
                }
                else // for invariant count characters, the numbers they represent can be added to the minimum
                {
                    var numberOf = numericCounts[minimumCount[i]];

                    for (int j = 0; j < RelevantAlphabetCount; j++)
                    {
                        minimumCount[j] += numberOf[j];
                    }
                }
            }

            var tmpVariableMinimumCounts = new List<byte>();
            for (int i = 0; i < RelevantAlphabetCount; i++)
            {
                if (tmpVariableCharAlphabet.Contains(RelevantAlphabet[i]))
                {
                    tmpVariableMinimumCounts.Add(minimumCount[i]);
                }

                // we need to add in the invariant characters numeric counts to the tmpVariableBaselineCount
                if (tmpVariableCharAlphabet.Contains(RelevantAlphabet[i]) == false)
                {
                    var numericCounts = baselineCount[i] + 1; // +1 for the character itself
                    var chars = tmpVariableNumericCount[numericCounts];
                    for (int j = 0; j < chars.Count; j++)
                    {
                        tmpVariableBaselineCount[j] += chars[j];
                    }
                }
            }

            VariableAlphabet = tmpVariableCharAlphabet.ToArray();
            VariableAlphabetCount = VariableAlphabet.Count;

            variableBaselineCount = tmpVariableBaselineCount.ToArray();
            variableNumericCounts = tmpVariableNumericCount.Select(p => p.ToArray()).ToArray();
            variableMinimumCount = tmpVariableMinimumCounts.ToArray();

            proposedCounts = variableBaselineCount.ToArray();
            computedCounts = GetActualCounts(proposedCounts);

            Console.WriteLine("Pre-run summary");
            Console.WriteLine("---------------");
            Console.WriteLine("#\tChar\tBase\tMin\tFixed\tVBase\tVMin");

            for (int i = 0; i < RelevantAlphabetCount; i++)
            {
                var index = map[i];
                Console.WriteLine($"{i}\t" +
                    $"{RelevantAlphabet[i]}\t" +
                    $"{baselineCount[i]}\t" +
                    $"{minimumCount[i]}\t" +
                    $"{(VariableAlphabet.Contains(RelevantAlphabet[i]) ? "N" : "Y")}\t" +
                    $"{(VariableAlphabet.Contains(RelevantAlphabet[i]) ? variableBaselineCount[index.Value] : "")}\t" +
                    $"{(VariableAlphabet.Contains(RelevantAlphabet[i]) ? variableMinimumCount[index.Value] : "")}"
                    );
            }
        }

        /// <summary>
        /// Resets state, and optionally random. Primarily for benchmarking.
        /// </summary>
        /// <param name="resetRandom">If true the random state is reset.</param>
        public void Reset(bool resetRandom = true)
        {
            proposedCounts = new byte[VariableAlphabetCount];
            computedCounts = new byte[VariableAlphabetCount];
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
            var numberItems = proposedCounts.Select((p, index) => p == 0 ? string.Empty : p.ToCardinalNumberStringPrecomputed() + " " + RelevantAlphabet[index] + (p == 1 ? "" : PluralExtension)).Where(p => string.IsNullOrWhiteSpace(p) == false).ToList();
            var arg0 = string.IsNullOrWhiteSpace(conjunction) ? numberItems.Listify() : numberItems.ListifyWithConjunction(conjunction);
            return string.Format(Template, arg0);
        }

        private byte[] GetActualCounts(byte[] currentGuess)
        {
            Span<byte> result = stackalloc byte[VariableAlphabetCount];
            variableBaselineCount.CopyTo(result);

            for (var i = 0; i < VariableAlphabetCount; i++)
            {
                var c = currentGuess[i];
                if (c == 0) continue;

                // numeric + plural part
                var numericCount = variableNumericCounts[c];
                for (var j = 0; j < VariableAlphabetCount; j++)
                {
                    result[j] += numericCount[j];
                }

                // actual letter
                result[i]++;
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
                result[i] = GuessAgain(computedCounts[i], proposedCounts[i]);
                Debug.Assert(result[i] >= variableMinimumCount[i]);
            }
            return result;
        }

        private static byte GuessAgain(byte actualCount, byte currentGuess)
        {
            if (actualCount == currentGuess)
            {
                return actualCount;
            }
            else
            {
                return (byte)((actualCount + currentGuess + 1) / 2);
            }
        }
    }
}
