
using System.Diagnostics;

namespace Autogram
{
    public class AutogramConfig
    {
        public required string Template { get; init; }
        public required string Conjunction { get; init; }
        public required IList<CharacterConfig> Letters { get; init; }
        public IEnumerable<char> VariableChars => this.Letters.Where(p => p.IsVariable).Select(p => p.Char);

        public byte[][][] GetVariableNumericCounts()
        {
            return this.Letters.Where(p => p.IsVariable).Select(p => p.GetStringRepresentationFrequencies(this.VariableChars)).ToArray();
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

        public int PerDistinctCountModifier => Char switch
        {
            ',' => -2, // for commas the penultimate and final don't have a separator.
            ' ' => -2, // ditto for spaces
            _ => 0,
        };

        public bool IncludeSelfInCount => Char.HasExtendedName() == false;

        public IList<string> GetStringRepresentations()
        {
            return Enumerable.Range(0, 100).Select(StringRepresentationFor).ToList();
        }

        private string StringRepresentationFor(int i)
        {
            return i.ToCardinalNumberString() + " " + (IncludeSelfInCount ? this.Char.GetCharacterName(i) : string.Empty) + ExtensionsClass.Separator;
        }

        public byte[][] GetStringRepresentationFrequencies(IEnumerable<char> chars)
        {
            return this.GetStringRepresentations().Select(p => p.GetFrequencies(chars).ToByteArray()).ToArray();
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
