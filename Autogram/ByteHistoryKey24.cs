namespace Autogram
{
    public readonly struct ByteHistoryKey24 : IEquatable<ByteHistoryKey24>, IByteHistoryKey<ByteHistoryKey24>
    {
        public static int MaxLength => 24;

        private readonly ulong chunk0;
        private readonly ulong chunk1;
        private readonly ulong chunk2;
        private readonly int length;

        public ByteHistoryKey24(ReadOnlySpan<byte> values)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(values.Length, MaxLength);

            length = values.Length;
            chunk0 = ByteHistoryKeyChunks.ReadChunk(values, 0);
            chunk1 = ByteHistoryKeyChunks.ReadChunk(values, 8);
            chunk2 = ByteHistoryKeyChunks.ReadChunk(values, 16);
        }

        public static ByteHistoryKey24 Create(ReadOnlySpan<byte> values)
        {
            return new ByteHistoryKey24(values);
        }

        public bool Equals(ByteHistoryKey24 other)
        {
            return length == other.length
                && chunk0 == other.chunk0
                && chunk1 == other.chunk1
                && chunk2 == other.chunk2;
        }

        public override bool Equals(object? obj)
        {
            return obj is ByteHistoryKey24 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(chunk0, chunk1, chunk2, length);
        }
    }
}
