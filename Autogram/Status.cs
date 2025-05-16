namespace Autogram
{
    public class Status
    {
        public Status()
        {
            _totalDistance = new Lazy<int>(() => GuessError?.Sum(Math.Abs) ?? 0);
            //_totalDistance = new Lazy<int>(() => Math.Abs( GuessError?.Sum() ?? 0));
        }
        public string CurrentString { get; set; }
        public bool Success { get; set; }
        public int HistoryCount { get; set; }
        public bool RandomReset { get; set; }
        public byte[] currentGuess { get; set; }
        public int[] GuessError { get; set; }

        private Lazy<int> _totalDistance;

        public int TotalDistance => _totalDistance.Value;
    }
}
