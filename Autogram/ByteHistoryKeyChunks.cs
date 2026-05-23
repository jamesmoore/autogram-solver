using System.Buffers.Binary;

namespace Autogram
{
    internal static class ByteHistoryKeyChunks
    {
        public static ulong ReadChunk(ReadOnlySpan<byte> values, int offset)
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
