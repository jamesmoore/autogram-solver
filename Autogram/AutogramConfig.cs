
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
            '\'' => CountBasis.PerNonZeroPlural,
            _ => CountBasis.PerInstance,
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
        /// <summary>
        /// For each instance of the counts that are plural excluding zero
        /// </summary>
        PerNonZeroPlural,
    }
}
