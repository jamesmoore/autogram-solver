namespace Autogram
{
    public class Autogram
    {
        private const int AlphabetSize = 26;

        Random random = new();

        private int[] currentGuess = new int[AlphabetSize];

        private readonly HashSet<string> history = [];

        public Autogram()
        {
            this.currentGuess = Randomize();
        }

        private int[] Randomize()
        {
            var acutalCounts = GetActualCounts();

            return Enumerable.Range(0, AlphabetSize).Select(p => acutalCounts[p] == currentGuess[p] ? currentGuess[p] : NextGuess(acutalCounts[p])).ToArray();
        }

        private int NextGuess(int acutalCount)
        {
            var nextGuess = acutalCount + random.Next(6) - 3;
            if (nextGuess < 0) nextGuess = 0;
            return nextGuess;
        }

        private void SetGuess(int[] newGuess)
        {
            if (newGuess == null || newGuess.Length != AlphabetSize)
            {
                throw new ArgumentException("Invalid counts assignment");
            }

            currentGuess = newGuess;
            history.Add(newGuess.ToCSV());
        }

        public override string ToString()
        {
            var numberItems = currentGuess.Select((p, i) => p.ToCardinalNumberString() + " " + (char)('a' + i) + (p == 1 ? "" : "'s")).ToList();
            return "This sentence employs " + numberItems.ListifyWithConjunction();
        }

        public int[] GetActualCounts()
        {
            var formatted = this.ToString().ToLower();
            var currentCountsGrouped = formatted.GroupBy(p => p).Where(p => p.Key >= 'a' && p.Key <= 'z').OrderBy(p => p.Key).Take(AlphabetSize);
            var currentCounts = currentCountsGrouped.Select(p => p.Count()).ToArray();
            return currentCounts;
        }

        public int[] GetGuessError()
        {
            var diff = GetActualCounts().Select((p, i) => currentGuess[i] - p).ToArray();
            return diff;
        }

        public bool Evaluate()
        {
            return Enumerable.SequenceEqual(GetActualCounts(), currentGuess);
        }

        public void Iterate()
        {
            var nextGuess = GuessAgain();

            if (history.Contains(nextGuess.ToCSV()))
            {
                SetGuess(Randomize());
            }
            else
            {
                SetGuess(nextGuess);
            }
        }

        private int[] GuessAgain()
        {
            var currentCounts = GetActualCounts();
            var guess = currentCounts.Zip(currentGuess).Select(p => GuessAgain(p.First, p.Second)).ToArray();
            return guess;
        }

        private static int GuessAgain(int actualCount, int currentGuess)
        {
            if (actualCount == currentGuess)
            {
                return actualCount;
            }
            else if ((actualCount + currentGuess) % 2 == 0 || (actualCount + currentGuess) % 3 == 0)
            {
                return (actualCount + currentGuess) / 2;
            }
            else
            {
                return (actualCount + currentGuess + 1) / 2;
            }
        }

        //private int RandomOffset(int previousGuess, int actual)
        //{
        //    return previousGuess == actual ? 0 : random.Next() % 10 == 0 ? 1 : random.Next(3) - 1;
        //}
    }
}
