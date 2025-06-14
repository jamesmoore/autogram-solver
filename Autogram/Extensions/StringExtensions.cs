using System.Text.RegularExpressions;

namespace Autogram.Extensions
{
    public static class StringExtensions
    {
        private static readonly Dictionary<string, int> wordToNumber = Enumerable.Range(0, 100).ToDictionary(
            p => ((byte)p).ToCardinalNumberStringPrecomputed(),
            p => p
        );

        public static bool IsAutogram(this string sentence)
        {
            var lower = sentence.ToLowerInvariant();

            // Actual letter counts in the sentence
            Dictionary<char, int> actualCounts = new();
            foreach (char c in lower)
            {
                if (char.IsLetter(c) || c == '-' || c == ',' || c == '\'')
                {
                    if (!actualCounts.ContainsKey(c))
                        actualCounts[c] = 0;
                    actualCounts[c]++;
                }
            }

            lower = lower.Replace("commas", ",'s");
            lower = lower.Replace("hyphens", "-'s");
            lower = lower.Replace("apostrophes", "''s");

            // Match patterns like: "eight a's", "twenty-seven s's", etc.
            var pattern = @"\b(" + string.Join("|", wordToNumber.Keys) + @") ([a-z,\-'])'?s\b";
            var matches = Regex.Matches(lower, pattern);

            foreach (Match match in matches)
            {
                string wordNumber = match.Groups[1].Value;
                char letter = match.Groups[2].Value[0];

                if (!wordToNumber.TryGetValue(wordNumber, out int declaredCount))
                    return false; // Should never happen due to regex

                actualCounts.TryGetValue(letter, out int actualCount);

                if (declaredCount != actualCount)
                    return false;
            }

            return true;
        }
    }
}
