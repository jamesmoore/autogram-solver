namespace Autogram.Comparer
{
    public class ByteArraySpanWithChunkedHashCodeComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            return x.AsSpan().SequenceEqual(y);
        }

        public int GetHashCode(byte[] obj)
        {
            unchecked
            {
                int hash = 17;
                int i = 0;

                for (; i + 3 < obj.Length; i += 4)
                {
                    int chunk =
                        obj[i]
                        | (obj[i + 1] << 8)
                        | (obj[i + 2] << 16)
                        | (obj[i + 3] << 24);

                    hash = hash * 31 + chunk;
                }

                for (; i < obj.Length; i++)
                    hash = hash * 31 + obj[i];

                return hash;
            }
        }
    }
}
