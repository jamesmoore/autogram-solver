namespace Autogram
{
    public readonly struct Status(bool success, bool randomized, bool reordered)
    {
        public readonly bool Success { get; } = success;
        public readonly bool Randomized { get; } = randomized;
        public readonly bool Reordered { get; } = reordered;
    }
}
