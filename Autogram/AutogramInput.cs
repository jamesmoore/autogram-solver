namespace Autogram
{
    public class AutogramInput
    {
        public required string Alphabet { get; init; }
        public required string Template { get; init; }
        public required string Conjunction { get; init; }
        public required string SeparatorString { get; init; }
        public required string PluralSuffix { get; init; }
        public required string Forced { get; init; }
    }
}
