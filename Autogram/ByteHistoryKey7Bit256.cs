namespace Autogram
{
    /// <summary>
    /// This will work for byte sequences of length up to 36, where each byte is less than 128.
    /// </summary>
    internal readonly struct ByteHistoryKey7Bit256 : IEquatable<ByteHistoryKey7Bit256>
    {
        private const int BitsPerValue = 7;
        private const int BitsPerChunk = sizeof(ulong) * 8;

        public const int MaxLength = (BitsPerChunk * 4) / BitsPerValue;

        private readonly ulong chunk0;
        private readonly ulong chunk1;
        private readonly ulong chunk2;
        private readonly ulong chunk3;
        private readonly int length;

        public ByteHistoryKey7Bit256(ReadOnlySpan<byte> values)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(values.Length, MaxLength);

            Span<ulong> chunks = stackalloc ulong[4];
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, (byte)128);

                var bitIndex = i * BitsPerValue;
                var chunkIndex = bitIndex / BitsPerChunk;
                var bitOffset = bitIndex % BitsPerChunk;

                chunks[chunkIndex] |= (ulong)value << bitOffset;
                if (bitOffset > BitsPerChunk - BitsPerValue)
                {
                    chunks[chunkIndex + 1] |= (ulong)value >> (BitsPerChunk - bitOffset);
                }
            }

            length = values.Length;
            chunk0 = chunks[0];
            chunk1 = chunks[1];
            chunk2 = chunks[2];
            chunk3 = chunks[3];
        }

        public bool Equals(ByteHistoryKey7Bit256 other)
        {
            return length == other.length
                && chunk0 == other.chunk0
                && chunk1 == other.chunk1
                && chunk2 == other.chunk2
                && chunk3 == other.chunk3;
        }

        public override bool Equals(object? obj)
        {
            return obj is ByteHistoryKey7Bit256 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(length, chunk0, chunk1, chunk2, chunk3);
        }
    }
}
