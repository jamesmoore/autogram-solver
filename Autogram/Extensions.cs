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

        static Extensions() {
            precomputed = Enumerable.Range(0,100).Select(p => p.ToCardinalNumberString()).ToArray();
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

        public static string ListifyWithConjunction(this IEnumerable<string> items)
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
                return string.Join(", ", materialized.Take(materialized.Count - 1)) + " and " + materialized.Last();
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
    }
}
