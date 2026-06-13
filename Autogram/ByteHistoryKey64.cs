namespace Autogram
{
    public readonly struct ByteHistoryKey64 : IEquatable<ByteHistoryKey64>, IByteHistoryKey<ByteHistoryKey64>
    {
        public static int MaxLength => 64;

        private readonly ulong chunk0;
        private readonly ulong chunk1;
        private readonly ulong chunk2;
        private readonly ulong chunk3;
        private readonly ulong chunk4;
        private readonly ulong chunk5;
        private readonly ulong chunk6;
        private readonly ulong chunk7;
        private readonly int length;

        public ByteHistoryKey64(ReadOnlySpan<byte> values)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(values.Length, MaxLength);

            length = values.Length;
            chunk0 = ByteHistoryKeyChunks.ReadChunk(values, 0);
            chunk1 = ByteHistoryKeyChunks.ReadChunk(values, 8);
            chunk2 = ByteHistoryKeyChunks.ReadChunk(values, 16);
            chunk3 = ByteHistoryKeyChunks.ReadChunk(values, 24);
            chunk4 = ByteHistoryKeyChunks.ReadChunk(values, 32);
            chunk5 = ByteHistoryKeyChunks.ReadChunk(values, 40);
            chunk6 = ByteHistoryKeyChunks.ReadChunk(values, 48);
            chunk7 = ByteHistoryKeyChunks.ReadChunk(values, 56);
        }

        public static ByteHistoryKey64 Create(ReadOnlySpan<byte> values)
        {
            return new ByteHistoryKey64(values);
        }

        public byte[] Decode()
        {
            var values = new byte[length];

            WriteChunk(values, 0, chunk0);
            WriteChunk(values, 8, chunk1);
            WriteChunk(values, 16, chunk2);
            WriteChunk(values, 24, chunk3);
            WriteChunk(values, 32, chunk4);
            WriteChunk(values, 40, chunk5);
            WriteChunk(values, 48, chunk6);
            WriteChunk(values, 56, chunk7);

            return values;
        }

        public bool Equals(ByteHistoryKey64 other)
        {
            return length == other.length
                && chunk0 == other.chunk0
                && chunk1 == other.chunk1
                && chunk2 == other.chunk2
                && chunk3 == other.chunk3
                && chunk4 == other.chunk4
                && chunk5 == other.chunk5
                && chunk6 == other.chunk6
                && chunk7 == other.chunk7;
        }

        public override bool Equals(object? obj)
        {
            return obj is ByteHistoryKey64 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(chunk0, chunk1, chunk2, chunk3, chunk4, chunk5, chunk6, chunk7);
        }

        private static void WriteChunk(Span<byte> values, int offset, ulong chunk)
        {
            var remaining = Math.Min(sizeof(ulong), values.Length - offset);
            if (remaining <= 0)
            {
                return;
            }

            for (var i = 0; i < remaining; i++)
            {
                values[offset + i] = (byte)(chunk >> (i * 8));
            }
        }
    }
}
