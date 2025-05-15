namespace Autogram.Comparer
{
    public class IntArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(int[] obj)
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
