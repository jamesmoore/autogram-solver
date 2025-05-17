using Autogram.Comparer;

namespace Autogram
{
    public class AutogramBytes : IAutogramFinder
    {
        private readonly IReadOnlyList<char> FullAlphabet;
        private readonly HashSet<char> AlphabetHashSet;
        private Random random;

        private byte[] currentGuess;
        private byte[] actualCounts;

        private readonly HashSet<byte[]> history = new(new ByteArraySpanComparer());
        private readonly int? randomSeed;

        public AutogramBytes(IEnumerable<char> alphabet, int? randomSeed = null)
        {
            FullAlphabet = alphabet.ToList();
            AlphabetHashSet = [.. alphabet];
            currentGuess = new byte[FullAlphabet.Count];
            actualCounts = new byte[FullAlphabet.Count];
            random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            this.currentGuess = Randomize();
            this.randomSeed = randomSeed;
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
            var numberItems = currentGuess.Select((p, index) => p == 0 ? string.Empty : p.ToCardinalNumberString() + " " + FullAlphabet[index] + (p == 1 ? "" : "'s")).Where(p => string.IsNullOrWhiteSpace(p) == false).ToList();
            return "This sentence employs " + numberItems.Listify() + ", and one z.";
        }

        public byte[] GetActualCounts(string currentString)
        {
            var formatted = currentString.ToLower();
            var currentCountsGrouped = formatted.Where(AlphabetHashSet.Contains).GroupBy(p => p).ToLookup(p => p.Key, p => p.Count());
            var currentCounts = FullAlphabet.Select(p => (byte)(currentCountsGrouped[p]?.FirstOrDefault() ?? 0)).ToArray();
            return currentCounts;
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

            var currentString = this.ToString();
            actualCounts = GetActualCounts(currentString);

            var sortedCurrent = currentGuess.OrderBy(p => p).ToArray();
            var sortedActual = actualCounts.OrderBy(p => p).ToArray();

            var equals = sortedCurrent.AsSpan().SequenceEqual(sortedActual);

            if (equals)
            {
                Console.WriteLine("REORDERING...");
                currentGuess = actualCounts.ToArray();
            }

            return new Status()
            {
                currentGuess = currentGuess,
                CurrentString = currentString,
                Success = actualCounts.AsSpan().SequenceEqual(currentGuess),
                HistoryCount = history.Count,
                RandomReset = randomReset,
                GuessError = actualCounts.Select((p, i) => currentGuess[i] - p).ToArray(),
            };
        }

        private byte[] GuessAgain()
        {
            var guess = actualCounts.Zip(currentGuess).Select(p => GuessAgain(p.First, p.Second)).ToArray();
            //guess[4] = 28;
            //guess[5] = 5;
            //guess[6] = 3;
            //guess[19] = 23;
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

        //private int RandomOffset(int previousGuess, int actual)
        //{
        //    return previousGuess == actual ? 0 : random.Next() % 10 == 0 ? 1 : random.Next(3) - 1;
        //}
    }
}
