
namespace Autogram
{
    public class AutogramConfig
    {
        public required string Template { get; init; }
        public required string Conjunction { get; init; }
        public required string PluralExtension { get; init; }
        public required IList<CharacterConfig> Letters { get; init; }
        public required IList<byte[]> VariableNumericCounts { get; init; }
    }

    public class CharacterConfig
    {
        public required int Index { get; init; }
        public required char Char { get; init; }
        public required int BaselineCount { get; set; }
        public required int MinimumCount { get; set; }
        public required bool IsVariable { get; init; }
        public required int? VariableIndex { get; init; }
        public required int? VariableBaselineCount { get; set; }
        public CountBasis CountBasis => Char switch
        {
            ',' => CountBasis.PerDistinctCountOfOthers,
            ' ' => CountBasis.PerDistinctCountOfOthers,
            _ => CountBasis.PerInstance,
        };

        public int PerDistinctCountModifier => Char switch
        {
            ',' => -2, // for commas the penultimate and final don't have a separator.
            ' ' => -2, // ditto for spaces
            _ => 0,
        };

        public int PerDistinctCountMultiplier => Char switch
        {
            ',' => 1, // one comma per itemised count
            ' ' => 2, // spaces have two per itemised count "{count} {letter}, "
            _ => 0,
        };

        public bool IncludeSelfInCount => Char.HasExtendedName() == false;

        public IList<string> StringRepresentations
        {
            get
            {
                return Enumerable.Range(0, 100).Select(x => x.ToCardinalNumberString() + this.Char.GetCharacterName(x)).ToList();
            }
        }
    }

    public enum CountBasis
    {
        /// <summary>
        /// the count is for each instance of character in the sentence (Regular letters).
        /// </summary>
        PerInstance,
        /// <summary>
        /// for each instance of the character PLUS one for each non zero of the others (eg for commas)
        /// </summary>
        PerDistinctCountOfOthers,
    }
}
