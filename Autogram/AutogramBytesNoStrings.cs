using Autogram.Comparer;

namespace Autogram
{
    public class AutogramBytesNoStrings : IAutogramFinder
    {
        private readonly string Template;
        private const string PluralExtension = "'s";
        private readonly IReadOnlyList<char> FullAlphabet;
        private Random random;

        private byte[] currentGuess;
        private byte[] actualCounts;

        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly int? randomSeed;

        private readonly byte[] baselineCount;
        private readonly byte[][] numericCounts;
        private readonly byte[] pluralCount;

        private readonly int AlphabetCount;

        public AutogramBytesNoStrings(IEnumerable<char> alphabet, string template, int? randomSeed = null)
        {
            Template = template;
            FullAlphabet = alphabet.ToList();
            AlphabetCount = FullAlphabet.Count;
            var alphabetIndex = alphabet.ToDictionary(p => p, p => alphabet.ToList().IndexOf(p));

            currentGuess = new byte[FullAlphabet.Count];
            actualCounts = new byte[FullAlphabet.Count];
            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            this.currentGuess = Randomize();
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
        }

        public void Reset(bool resetRandom = true)
        {
            currentGuess = new byte[FullAlphabet.Count];
            actualCounts = new byte[FullAlphabet.Count];
            if (resetRandom)
            {
                random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
            }
            history.Clear();
        }

        private byte[] Randomize()
        {
            return Enumerable.Range(0, FullAlphabet.Count).Select(p => actualCounts[p] == currentGuess[p] ? currentGuess[p] : NextGuess(actualCounts[p])).ToArray();
        }

        private byte NextGuess(byte acutalCount)
        {
            var nextGuess = acutalCount + random.Next(6) - 3;
            if (nextGuess < 0) nextGuess = 0;
            return (byte)nextGuess;
        }

        public override string ToString()
        {
            var numberItems = currentGuess.Select((p, index) => p == 0 ? string.Empty : p.ToCardinalNumberStringPrecomputed() + " " + FullAlphabet[index] + (p == 1 ? "" : PluralExtension)).Where(p => string.IsNullOrWhiteSpace(p) == false).ToList();
            return string.Format(Template, numberItems.Listify());
        }

        public byte[] GetActualCounts(byte[] currentGuess)
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

                // TODO add conjunction
            }
            return result.ToArray();
        }

        public Status Iterate()
        {
            var nextGuess = GuessAgain();
            var randomReset = false;
            if (history.Contains(nextGuess))
            {
                currentGuess = Randomize();
                randomReset = true;
            }
            else
            {
                currentGuess = nextGuess;
            }

            history.Add(currentGuess);

            actualCounts = GetActualCounts(currentGuess);

            var reorderedEquals = ((ReadOnlySpan<byte>)actualCounts.AsSpan()).UnorderedByteSpanEquals(currentGuess);

            if (reorderedEquals)
            {
                currentGuess = actualCounts.ToArray();
            }

            bool success = actualCounts.AsSpan().SequenceEqual(currentGuess);

            return new Status()
            {
                currentGuess = currentGuess,
                actualCounts = actualCounts,
                CurrentString = success ? this.ToString() : "---",
                Success = success,
                HistoryCount = history.Count,
                RandomReset = randomReset,
                Reordered = reorderedEquals,
            };
        }

        private byte[] GuessAgain()
        {
            var guess = actualCounts.Zip(currentGuess).Select(p => GuessAgain(p.First, p.Second)).ToArray();
            return guess;
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
