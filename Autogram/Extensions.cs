﻿using System.Text.RegularExpressions;

namespace Autogram
{
    public static class ExtensionsClass
    {
        public static string ToCardinalNumberString(this int i)
        {
            if (i < 0)
            {
                throw new NotImplementedException("Negatives not supported");
            }

            if (i > 99)
            {
                throw new NotImplementedException("Over 100 not supported");
            }

            string[] first20 = [
                "zero",
                "one",
                "two",
                "three",
                "four",
                "five",
                "six",
                "seven",
                "eight",
                "nine",
                "ten",
                "eleven",
                "twelve",
                "thirteen",
                "fourteen",
                "fifteen",
                "sixteen",
                "seventeen",
                "eighteen",
                "nineteen",
                ];

            if (i < first20.Length)
            {
                return first20[i];
            }

            string[] tens = [
                "twenty",
                "thirty",
                "forty",
                "fifty",
                "sixty",
                "seventy",
                "eighty",
                "ninety"
                ];

            var unit = i % 10;
            var firstPart = tens[((i - unit) / 10) - 2];

            if (unit == 0)
            {
                return firstPart;
            }
            else
            {
                return $"{firstPart}-{first20[unit]}";
            }
        }

        private static string[] precomputed;

        static ExtensionsClass()
        {
            precomputed = Enumerable.Range(0, 100).Select(p => p.ToCardinalNumberString()).ToArray();
        }

        public static string ToCardinalNumberStringPrecomputed(this byte i)
        {
            return precomputed[i];
        }

        public static string Listify(this IEnumerable<string> items, string separator)
        {
            var materialized = items?.ToList();

            if (materialized == null || materialized.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return string.Join(separator, materialized);
            }
        }

        public static string ListifyWithConjunction(this IEnumerable<string> items, string separator, string conjunction)
        {
            var materialized = items?.ToList();

            if (materialized == null || materialized.Count == 0)
            {
                return string.Empty;
            }
            else if (materialized.Count == 1)
            {
                return materialized[0];
            }
            else
            {
                return string.Join(separator, materialized.Take(materialized.Count - 1)) + conjunction + materialized.Last();
            }
        }

        public static string Humanize(this double number)
        {
            string[] suffix = { "f", "a", "p", "n", "μ", "m", string.Empty, "k", "M", "G", "T", "P", "E" };

            var absnum = Math.Abs(number);

            int mag;
            if (absnum < 1)
            {
                mag = (int)Math.Floor(Math.Floor(Math.Log10(absnum)) / 3);
            }
            else
            {
                mag = (int)(Math.Floor(Math.Log10(absnum)) / 3);
            }

            var shortNumber = number / Math.Pow(10, mag * 3);

            return $"{shortNumber:0.###}{suffix[mag + 6]}";
        }

        public static string Humanize(this int number)
        {
            return ((double)number).Humanize();
        }

        public static double StandardDeviation(this double[] values)
        {
            double mean = values.Average();
            double variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Length;
            return Math.Sqrt(variance);
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

        public static int[] GetFrequencies(this string referenceString, IEnumerable<char> alphabet)
        {
            return alphabet.Select(p => referenceString.Where(q => q == p).Count()).ToArray();
        }

        public static byte[] ToByteArray(this IEnumerable<int> intarray)
        {
            return intarray.Select(p => (byte)p).ToArray();
        }

        public static bool IsValidRegex(this string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return false;
            }

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static string GetRelativePathTo(this DirectoryInfo baseDir, FileInfo targetFile)
        {
            var basePath = Path.GetFullPath(baseDir.FullName + Path.DirectorySeparatorChar);
            var targetPath = targetFile.FullName;

            if (!string.Equals(Path.GetPathRoot(basePath), Path.GetPathRoot(targetPath), StringComparison.OrdinalIgnoreCase))
            {
                // Different drives, return full path
                return targetPath;
            }

            var baseUri = new Uri(basePath);
            var targetUri = new Uri(targetPath);

            var relativePath = Uri.UnescapeDataString(baseUri.MakeRelativeUri(targetUri).ToString())
                                     .Replace('/', Path.DirectorySeparatorChar);
            return relativePath;
        }
    }
}
