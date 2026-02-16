
using System.Diagnostics;

namespace Autogram
{
    public class AutogramConfig
    {
        public required IList<CharacterConfig> AllChars { get; init; }

        public IEnumerable<CharacterConfig> VariableChars => AllChars.Where(p => p.IsVariable);

        public byte[][][] GetNumericCounts()
        {
            return this.AllChars.Select(p => p.GetStringRepresentationFrequencies(this.VariableCharsChars)).ToArray();
        }

        public byte[][][] GetVariableNumericCounts()
        {
            return this.VariableChars.Select(p => p.GetStringRepresentationFrequencies(this.VariableCharsChars)).ToArray();
        }

        private IEnumerable<char> VariableCharsChars => this.VariableChars.Select(p => p.Char);

        public void Validate()
        {
            Debug.Assert(AllChars.All(p => p.MinimumCount >= p.BaselineCount));
            Debug.Assert(VariableChars.All(p => p.MinimumCount >= p.VariableBaselineCount));
        }
    }

    [DebuggerDisplay("{Char.ToString()}")]
    public class CharacterConfig(string separator)
    {
        public required int Index { get; init; }
        public required char Char { get; init; }
        
        /// <summary>
        /// Baseline count of the character <c>Char</c> 
        /// </summary>
        /// <remarks>Baseline is defined as present in template, conjunction or any of the pluralised extended chars (eg, 'commas')</remarks>
        public required int BaselineCount { get; set; }

        /// <summary>
        /// The minimum count of the character <c>Char</c> that is required in the autogram, 
        /// </summary>
        /// <remarks>
        /// * The <c>BaselineCount</c><br/>
        /// * plus 1 if guaranteed to be present, to represent itself in the list.<br/>
        /// * plus the counts of the chars in the cardinals of the invariant characters.<br/>
        /// For invariant chars this is the actual count.
        /// </remarks>
        public required int MinimumCount { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether the character count is variable, or fixed from the outset.
        /// </summary>
        /// <remarks>
        /// Variable means it is present in the numeric alphabet and can vary.<br/>
        /// Non-Variable chars can have their counts precomputed.</remarks>
        public required bool IsVariable { get; init; }
        public required int? VariableIndex { get; init; }

        /// <summary>
        /// The variable baseline count
        /// </summary>
        /// <remarks>
        /// The <c>BaselineCount</c>
        /// Plus the counts of the chars in the cardinals of the invariant characters.
        /// </remarks>
        public required int? VariableBaselineCount { get; set; }

        /// <summary>
        /// For the separator chars (comma and space typically) it should be reduced by 2 because in the itemised string they don't appear on the last two entries.
        /// </summary>
        public int PerDistinctCountModifier => separator.Contains(this.Char) ? -2 : 0;

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
            return i.ToCardinalNumberString() + " " + (IncludeSelfInCount ? this.Char.GetCharacterName(i) : string.Empty) + separator;
        }
    }
}
