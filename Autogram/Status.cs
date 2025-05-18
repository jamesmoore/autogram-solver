namespace Autogram
{
    public class Status
    {
        public Status()
        {
            _totalDistance = new Lazy<int>(() => GuessError?.Sum(Math.Abs) ?? 0);
            //_totalDistance = new Lazy<int>(() => Math.Abs( GuessError?.Sum() ?? 0));
            _guessError = new Lazy<int[]>(() => ActualCounts.Select((p, i) => CurrentGuess[i] - p).ToArray());
        }
        public bool Success { get; set; }
        public int HistoryCount { get; set; }
        public bool RandomReset { get; set; }
        public byte[] CurrentGuess { get; set; }
        public byte[] ActualCounts { get; set; }


        private Lazy<int[]> _guessError;
        public int[] GuessError => _guessError.Value;

        private Lazy<int> _totalDistance;

        public int TotalDistance => _totalDistance.Value;

        public bool Reordered { get; internal set; }
    }
}
