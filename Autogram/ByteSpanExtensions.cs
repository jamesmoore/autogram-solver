using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Autogram
{
    public static class ByteSpanExtensions
    {
        public static Vector256<byte> ToVector256(this ReadOnlySpan<byte> values)
        {
            Span<byte> buffer = stackalloc byte[Vector256<byte>.Count];
            values.CopyTo(buffer);
            return MemoryMarshal.Cast<byte, Vector256<byte>>(buffer)[0];
        }

        public static Vector512<byte> ToVector512(this ReadOnlySpan<byte> values)
        {
            Span<byte> buffer = stackalloc byte[Vector512<byte>.Count];
            values.CopyTo(buffer);
            return MemoryMarshal.Cast<byte, Vector512<byte>>(buffer)[0];
        }

        public static bool ByteArraysHaveSameContents(this ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var counts = new int[256]; // All possible byte values

            foreach (var b1 in a)
            {
                counts[b1]++;
            }

            foreach (var b2 in b)
            {
                if (--counts[b2] < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool UnorderedByteSpanEquals(this ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            Span<int> counts = stackalloc int[256];

            foreach (var b1 in a)
            {
                counts[b1]++;
            }

            foreach (var b2 in b)
            {
                if (--counts[b2] < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool UnorderedByteSpanEqualsWithSum(this ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            int sumA = 0;
            int sumB = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sumA += a[i];
                sumB += b[i];
            }

            if (sumA != sumB)
            {
                return false;
            }

            Span<int> counts = stackalloc int[256];

            foreach (var b1 in a)
            {
                counts[b1]++;
            }

            foreach (var b2 in b)
            {
                if (--counts[b2] < 0)
                {
                    return false;
                }
            }

            return true;
        }


        public static bool UnorderedByteSpanEquals2(this ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }

            Span<byte> xSorted = stackalloc byte[x.Length];
            Span<byte> ySorted = stackalloc byte[y.Length];
            x.CopyTo(xSorted);
            y.CopyTo(ySorted);
            xSorted.Sort();
            ySorted.Sort();

            for (int i = 0; i < xSorted.Length; i++)
            {
                if (xSorted[i] != ySorted[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool UnorderedIntSpanEquals2(this ReadOnlySpan<int> x, ReadOnlySpan<int> y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }

            Span<int> xSorted = stackalloc int[x.Length];
            Span<int> ySorted = stackalloc int[y.Length];
            x.CopyTo(xSorted);
            y.CopyTo(ySorted);
            xSorted.Sort();
            ySorted.Sort();

            for (int i = 0; i < xSorted.Length; i++)
            {
                if (xSorted[i] != ySorted[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
