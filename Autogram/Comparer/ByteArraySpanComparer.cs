namespace Autogram.Comparer
{
    public class ByteArraySpanComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            return x.AsSpan().SequenceEqual(y);
        }

        public int GetHashCode(byte[] obj)
        {
            if (obj == null) return 0;
            unchecked
            {
                int hash = 17;
                foreach (int i in obj)
                    hash = hash * 31 + i;
                return hash;
            }
        }
    }
}
