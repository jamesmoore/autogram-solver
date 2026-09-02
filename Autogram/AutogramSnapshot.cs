namespace Autogram
{
    /// <summary>
    /// A snapshot of the autogram solver state at a point in time, used for rendering the current sentence.
    /// </summary>
    public class AutogramSnapshot
    {
        private readonly IReadOnlyList<(char Char, int Count)> charCounts;
        private readonly string pluralSuffix;

        public AutogramSnapshot(IEnumerable<(char Char, int Count)> charCounts, string pluralSuffix)
        {
            this.charCounts = charCounts.ToList();
            this.pluralSuffix = pluralSuffix;
        }

        /// <summary>
        /// Builds the autogram sentence from this snapshot.
        /// </summary>
        /// <param name="template">The sentence template containing a {0} placeholder.</param>
        /// <param name="conjunction">The conjunction inserted before the final item in the list.</param>
        /// <param name="separator">The separator between items in the list.</param>
        /// <returns>The formatted autogram sentence.</returns>
        public string ToString(string template, string conjunction, string separator)
        {
            var numberItems = charCounts.Select(p => ToListEntry(p.Char, p.Count, pluralSuffix)).ToList();
            var arg0 = string.IsNullOrWhiteSpace(conjunction) ? numberItems.Listify(separator) : numberItems.ListifyWithConjunction(separator, conjunction);
            return string.Format(template, arg0);
        }

        public static string ToListEntry(char character, int quantity, string pluralSuffix)
        {
            return quantity == 0 ?
                string.Empty :
                ((byte)quantity).ToCardinalNumberStringPrecomputed() + " " + character.GetCharacterName(quantity, pluralSuffix);
        }

    }
}
