namespace Autogram
{
    /// <summary>
    /// A snapshot of the autogram solver state at a point in time, used for rendering the current sentence.
    /// </summary>
    public class AutogramSnapshot(AutogramInput input, IEnumerable<(char Char, int Count)> charCounts)
    {
        private readonly IReadOnlyList<(char Char, int Count)> charCounts = charCounts.ToList();

        /// <summary>
        /// Builds the autogram sentence from this snapshot.
        /// </summary>
        /// <returns>The formatted autogram sentence.</returns>
        public override string ToString()
        {
            var numberItems = charCounts.Select(p => ToListEntry(p.Char, p.Count, input.PluralSuffix)).ToList();
            var arg0 = string.IsNullOrWhiteSpace(input.Conjunction) ? numberItems.Listify(input.SeparatorString) : numberItems.ListifyWithConjunction(input.SeparatorString, input.Conjunction);
            return string.Format(input.Template, arg0);
        }

        public static string ToListEntry(char character, int quantity, string pluralSuffix)
        {
            return quantity == 0 ?
                string.Empty :
                ((byte)quantity).ToCardinalNumberStringPrecomputed() + " " + character.GetCharacterName(quantity, pluralSuffix);
        }

    }
}
