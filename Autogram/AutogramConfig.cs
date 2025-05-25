
namespace Autogram
{
    public class AutogramConfig
    {
        public string Template { get; set; }
        public string Conjunction { get; set; }
        public string PluralExtension { get; set; }
        public List<LetterConfig> Letters { get; set; }
        public List<byte[]> NumericCounts { get; internal set; }
        public List<byte[]> VariableNumericCounts { get; internal set; }
    }

    public class LetterConfig
    {
        public int Index { get; set; }
        public char Char { get; set; }
        public int BaselineCount { get; set; }
        public int MinimumCount {  get; set; }
        public bool IsVariable { get; set; }
        public int? VariableIndex { get; set; }
        public int? VariableBaselineCount { get; set; }
    }
}
