using System.Buffers.Binary;

namespace Autogram
{
    internal readonly struct ByteHistoryKey64 : IEquatable<ByteHistoryKey64>
    {
        public const int MaxLength = 64;

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
            chunk0 = ReadChunk(values, 0);
            chunk1 = ReadChunk(values, 8);
            chunk2 = ReadChunk(values, 16);
            chunk3 = ReadChunk(values, 24);
            chunk4 = ReadChunk(values, 32);
            chunk5 = ReadChunk(values, 40);
            chunk6 = ReadChunk(values, 48);
            chunk7 = ReadChunk(values, 56);
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

        private static ulong ReadChunk(ReadOnlySpan<byte> values, int offset)
        {
            var remaining = values.Length - offset;
            if (remaining <= 0)
            {
                return 0;
            }

            var slice = values[offset..];
            if (remaining >= sizeof(ulong))
            {
                return BinaryPrimitives.ReadUInt64LittleEndian(slice);
            }

            ulong chunk = 0;
            for (var i = 0; i < remaining; i++)
            {
                chunk |= (ulong)slice[i] << (i * 8);
            }

            return chunk;
        }
    }
}
