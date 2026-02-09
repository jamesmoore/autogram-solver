namespace Autogram.Comparer
{
    public sealed class IntArraySpanComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null || x.Length != y.Length) return false;
            return x.AsSpan().SequenceEqual(y);
        }

        public int GetHashCode(int[] obj)
        {
            if (obj == null) return 0;
            unchecked
            {
                int hash = 17;
                for (int i = 0; i < obj.Length; i++)
                    hash = hash * 31 + obj[i];
                return hash;
            }
        }
    }
}
