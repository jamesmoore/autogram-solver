
namespace Autogram
{
    public class AutogramConfig
    {
        public required string Template { get; init; }
        public required string Conjunction { get; init; }
        public required string PluralExtension { get; init; }
        public required IList<LetterConfig> Letters { get; init; }
        public required IList<byte[]> NumericCounts { get; init; }
        public required IList<byte[]> VariableNumericCounts { get; init; }
    }

    public class LetterConfig
    {
        public required int Index { get; init; }
        public required char Char { get; init; }
        public required int BaselineCount { get; init; }
        public required int MinimumCount {  get; set; }
        public required bool IsVariable { get; init; }
        public required int? VariableIndex { get; init; }
        public required int? VariableBaselineCount { get; set; }
    }
}
