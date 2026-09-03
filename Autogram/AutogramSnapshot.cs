namespace Autogram
{
    /// <summary>
    /// A snapshot of the autogram solver state at a point in time, used for rendering the current sentence.
    /// </summary>
    public class AutogramSnapshot
    {
        private readonly IReadOnlyList<(char Char, int Count)> charCounts;
        private readonly AutogramInput autogramInput;

        public AutogramSnapshot(IEnumerable<(char Char, int Count)> charCounts, AutogramInput autogramInput)
        {
            this.charCounts = charCounts.ToList();
            this.autogramInput = autogramInput;
        }

        /// <summary>
        /// Builds the autogram sentence from this snapshot.
        /// </summary>
        /// <returns>The formatted autogram sentence.</returns>
        public override string ToString()
        {
            var numberItems = charCounts.Select(p => ToListEntry(p.Char, p.Count, autogramInput.PluralSuffix)).ToList();
            var arg0 = string.IsNullOrWhiteSpace(autogramInput.Conjunction) ? numberItems.Listify(autogramInput.SeparatorString) : numberItems.ListifyWithConjunction(autogramInput.SeparatorString, autogramInput.Conjunction);
            return string.Format(autogramInput.Template, arg0);
        }

        public static string ToListEntry(char character, int quantity, string pluralSuffix)
        {
            return quantity == 0 ?
                string.Empty :
                ((byte)quantity).ToCardinalNumberStringPrecomputed() + " " + character.GetCharacterName(quantity, pluralSuffix);
        }

    }
}
