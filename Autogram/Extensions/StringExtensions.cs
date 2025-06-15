using System.Text.RegularExpressions;

namespace Autogram.Extensions
{
    public static class StringExtensions
    {
        public static Dictionary<char, int> GetCharFrequency(this string sentence)
        {
            return sentence.GroupBy(p => p).ToDictionary(p => p.Key, p => p.Count());
        }

        private static readonly Dictionary<string, int> wordToNumber = Enumerable.Range(0, 100).ToDictionary(
            p => ((byte)p).ToCardinalNumberStringPrecomputed(),
            p => p
        );

        public static Dictionary<char, int> GetStatedFrequency(this string sentence)
        {
            var lower = sentence.ToLower();
            lower = lower.Replace("commas", ",'s");
            lower = lower.Replace("hyphens", "-'s");
            lower = lower.Replace("apostrophes", "''s");
            lower = lower.Replace("spaces", " 's");

            // Match patterns like: "eight a's", "twenty-seven s's", etc.
            var pattern = @"\b(" + string.Join("|", wordToNumber.Keys) + @") ([a-z,\-' ])(?:'s)?\b";
            var matches = Regex.Matches(lower, pattern);

            var dictionary = new Dictionary<char, int>();

            foreach (Match match in matches)
            {
                string wordNumber = match.Groups[1].Value;
                char letter = match.Groups[2].Value[0];

                if (!wordToNumber.TryGetValue(wordNumber, out int declaredCount))
                    throw new InvalidOperationException();

                dictionary.Add(letter, declaredCount);
            }
            return dictionary;
        }

        public static bool IsAutogram(this string sentence)
        {
            var lower = sentence.ToLowerInvariant();

            var actualCounts = lower.GetCharFrequency();
            var declared = lower.GetStatedFrequency();

            foreach(var item in declared)
            {
                if(!actualCounts.TryGetValue(item.Key, out int count))
                {
                    return false;
                }

                if (item.Value != count)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
