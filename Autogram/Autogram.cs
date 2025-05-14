namespace Autogram
{
    public class Autogram
    {
        private readonly IReadOnlyList<char> FullAlphabet;
        private readonly HashSet<char> AlphabetHashSet;
        private readonly Random random = new();

        private int[] currentGuess;
        private int[] acutalCounts;

        private readonly HashSet<int[]> history = new(new IntArrayComparer());

        public Autogram(IEnumerable<char> alphabet)
        {
            FullAlphabet = alphabet.ToList();
            AlphabetHashSet = [.. alphabet];
            currentGuess = new int[FullAlphabet.Count];
            acutalCounts = new int[FullAlphabet.Count];

            this.currentGuess = Randomize();
        }

        private int[] Randomize()
        {
            return Enumerable.Range(0, FullAlphabet.Count).Select(p => acutalCounts[p] == currentGuess[p] ? currentGuess[p] : NextGuess(acutalCounts[p])).ToArray();
        }

        private int NextGuess(int acutalCount)
        {
            var nextGuess = acutalCount + random.Next(10) - 5;
            if (nextGuess < 0) nextGuess = 0;
            return nextGuess;
        }

        public override string ToString()
        {
            var numberItems = currentGuess.Select((p, index) => p == 0 ? string.Empty : p.ToCardinalNumberString() + " " + FullAlphabet[index] + (p == 1 ? "" : "'s")).Where(p => string.IsNullOrWhiteSpace(p) == false).ToList();
            return "This sentence employs " + numberItems.Listify() + ", and one z.";
        }

        public int[] GetActualCounts(string currentString)
        {
            var formatted = currentString.ToLower();
            var currentCountsGrouped = formatted.Where(AlphabetHashSet.Contains).GroupBy(p => p).ToLookup(p => p.Key, p => p.Count());
            var currentCounts = FullAlphabet.Select(p => currentCountsGrouped[p]?.FirstOrDefault() ?? 0).ToArray();
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
            acutalCounts = GetActualCounts(currentString);

            return new Status()
            {
                CurrentString = currentString,
                Success = Enumerable.SequenceEqual(acutalCounts, currentGuess),
                HistoryCount = history.Count,
                RandomReset = randomReset,
                GuessError = acutalCounts.Select((p, i) => currentGuess[i] - p).ToArray(),
            };
        }

        private int[] GuessAgain()
        {
            var guess = acutalCounts.Zip(currentGuess).Select(p => GuessAgain(p.First, p.Second)).ToArray();
            //guess[4] = 28;
            //guess[5] = 5;
            //guess[6] = 3;
            //guess[19] = 23;
            return guess;
        }

        private static int GuessAgain(int actualCount, int currentGuess)
        {
            if (actualCount == currentGuess)
            {
                return actualCount;
            }
            else if ((actualCount + currentGuess) % 2 == 0)
            {
                return (actualCount + currentGuess) / 2;
            }
            else
            {
                var v = actualCount + currentGuess;
                var increment = (v - 1) % 2 == 0 ? 1 : 0;
                return (v + 1) / 2;
            }
        }

        //private int RandomOffset(int previousGuess, int actual)
        //{
        //    return previousGuess == actual ? 0 : random.Next() % 10 == 0 ? 1 : random.Next(3) - 1;
        //}
    }
}
