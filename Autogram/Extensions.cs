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
    }
}
