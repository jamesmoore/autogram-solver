
using System.Diagnostics;

namespace Autogram
{
    public class AutogramConfig
    {
        public required string Template { get; init; }
        public required string Conjunction { get; init; }
        public required IList<CharacterConfig> AllChars { get; init; }

        public IEnumerable<CharacterConfig> VariableChars => AllChars.Where(p => p.IsVariable);

        public byte[][][] GetNumericCounts()
        {
            return this.AllChars.Select(p => p.GetStringRepresentationFrequencies(this.VariableCharsChars)).ToArray();
        }

        public byte[][][] GetVariableNumericCounts()
        {
            return this.AllChars.Where(p => p.IsVariable).Select(p => p.GetStringRepresentationFrequencies(this.VariableCharsChars)).ToArray();
        }

        private IEnumerable<char> VariableCharsChars => this.AllChars.Where(p => p.IsVariable).Select(p => p.Char);

        public void Validate()
        {
            Debug.Assert(AllChars.All(p => p.MinimumCount >= p.BaselineCount));
            Debug.Assert(VariableChars.All(p => p.MinimumCount >= p.VariableBaselineCount));
        }
    }

    [DebuggerDisplay("{Char.ToString()}")]
    public class CharacterConfig
    {
        public required int Index { get; init; }
        public required char Char { get; init; }
        public required int BaselineCount { get; set; }
        public required int MinimumCount { get; set; }
        public required bool IsVariable { get; init; }
        public required int? VariableIndex { get; init; }
        public required int? VariableBaselineCount { get; set; }

        /// <summary>
        /// For the separator chars (comma and space typically) it should be reduced by 2 because in the itemised string they don't appear on the last two entries.
        /// </summary>
        public int PerDistinctCountModifier => ExtensionsClass.Separator.Contains(this.Char) ? -2 : 0;

        public bool IncludeSelfInCount => Char.HasExtendedName() == false;

        public byte[][] GetStringRepresentationFrequencies(IEnumerable<char> chars)
        {
            return this.GetStringRepresentations().Select(p => p.GetFrequencies(chars).ToByteArray()).ToArray();
        }

        private IList<string> GetStringRepresentations()
        {
            return Enumerable.Range(0, 100).Select(StringRepresentationFor).ToList();
        }

        private string StringRepresentationFor(int i)
        {
            return i.ToCardinalNumberString() + " " + (IncludeSelfInCount ? this.Char.GetCharacterName(i) : string.Empty) + ExtensionsClass.Separator;
        }
    }
}
