namespace Autogram
{
    public readonly struct ByteHistoryKey32 : IEquatable<ByteHistoryKey32>, IByteHistoryKey<ByteHistoryKey32>
    {
        public static int MaxLength => 32;

        private readonly ulong chunk0;
        private readonly ulong chunk1;
        private readonly ulong chunk2;
        private readonly ulong chunk3;
        private readonly int length;

        public ByteHistoryKey32(ReadOnlySpan<byte> values)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(values.Length, MaxLength);

            length = values.Length;
            chunk0 = ByteHistoryKeyChunks.ReadChunk(values, 0);
            chunk1 = ByteHistoryKeyChunks.ReadChunk(values, 8);
            chunk2 = ByteHistoryKeyChunks.ReadChunk(values, 16);
            chunk3 = ByteHistoryKeyChunks.ReadChunk(values, 24);
        }

        public static ByteHistoryKey32 Create(ReadOnlySpan<byte> values)
        {
            return new ByteHistoryKey32(values);
        }

        public bool Equals(ByteHistoryKey32 other)
        {
            return length == other.length
                && chunk0 == other.chunk0
                && chunk1 == other.chunk1
                && chunk2 == other.chunk2
                && chunk3 == other.chunk3;
        }

        public override bool Equals(object? obj)
        {
            return obj is ByteHistoryKey32 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(chunk0, chunk1, chunk2, chunk3, length);
        }
    }
}
