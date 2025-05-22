namespace Autogram
{
    public readonly struct Status(bool success, int historyCount, bool randomized, bool reordered)
    {
        public readonly bool Success { get; } = success;
        public readonly int HistoryCount { get; } = historyCount;
        public readonly bool Randomized { get; } = randomized;
        public readonly bool Reordered { get; } = reordered;
    }
}
