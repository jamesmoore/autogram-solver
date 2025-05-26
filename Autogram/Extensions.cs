using System.Text.RegularExpressions;

namespace Autogram
{
    public static class Extensions
    {
        public static string ToCardinalNumberString(this int i)
        {
            if (i < 0) throw new NotImplementedException("Negatives not supported");
            if (i > 99) throw new NotImplementedException("Over 100 not supported");

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
                "fourty",
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

        public static string ToCardinalNumberString(this byte i)
        {
            if (i < 0) throw new NotImplementedException("Negatives not supported");
            if (i > 99) throw new NotImplementedException("Over 100 not supported");

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
                "fourty",
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

        private static string[] first20 = [
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

        private static string[] tens = [
            "twenty",
            "thirty",
            "fourty",
            "fifty",
            "sixty",
            "seventy",
            "eighty",
            "ninety"
        ];

        public static string ToCardinalNumberStringPredefined(this byte i)
        {
            if (i < 0) throw new NotImplementedException("Negatives not supported");
            if (i > 99) throw new NotImplementedException("Over 100 not supported");

            if (i < first20.Length)
            {
                return first20[i];
            }

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

        static Extensions()
        {
            precomputed = Enumerable.Range(0, 100).Select(p => p.ToCardinalNumberString()).ToArray();
        }

        public static string ToCardinalNumberStringPrecomputed(this byte i)
        {
            return precomputed[i];
        }

        public static string ToCSV(this int[] array)
        {
            return array.Select(p => p.ToString()).Aggregate((p, q) => p + "," + q);
        }

        public static string Listify(this IEnumerable<string> items)
        {
            var materialized = items?.ToList();

            if (materialized == null || materialized.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return string.Join(", ", materialized);
            }
        }

        public static string ListifyWithConjunction(this IEnumerable<string> items, string conjunction)
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
                return string.Join(", ", materialized.Take(materialized.Count - 1)) + conjunction + materialized.Last();
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

        public static bool ByteArraysHaveSameContents(this Span<byte> a, Span<byte> b)
        {
            if (a.Length != b.Length) return false;

            var counts = new int[256]; // All possible byte values

            foreach (var b1 in a) counts[b1]++;
            foreach (var b2 in b)
            {
                if (--counts[b2] < 0) return false;
            }

            return true;
        }

        public static bool UnorderedByteSpanEquals(this ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a.Length != b.Length) return false;

            Span<int> counts = stackalloc int[256];

            foreach (var b1 in a) counts[b1]++;
            foreach (var b2 in b)
                if (--counts[b2] < 0) return false;

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
            if (string.IsNullOrWhiteSpace(pattern)) return false;

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
    }
}
