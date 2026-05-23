namespace Autogram
{
    public readonly struct ByteHistoryKey16 : IEquatable<ByteHistoryKey16>, IByteHistoryKey<ByteHistoryKey16>
    {
        public static int MaxLength => 16;

        private readonly ulong chunk0;
        private readonly ulong chunk1;
        private readonly int length;

        public ByteHistoryKey16(ReadOnlySpan<byte> values)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(values.Length, MaxLength);

            length = values.Length;
            chunk0 = ByteHistoryKeyChunks.ReadChunk(values, 0);
            chunk1 = ByteHistoryKeyChunks.ReadChunk(values, 8);
        }

        public static ByteHistoryKey16 Create(ReadOnlySpan<byte> values)
        {
            return new ByteHistoryKey16(values);
        }

        public bool Equals(ByteHistoryKey16 other)
        {
            return length == other.length
                && chunk0 == other.chunk0
                && chunk1 == other.chunk1;
        }

        public override bool Equals(object? obj)
        {
            return obj is ByteHistoryKey16 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(chunk0, chunk1, length);
        }
    }
}
