
using System.Diagnostics;
using System.Numerics;

namespace Autogram
{
    public class AutogramConfig
    {
        public required IList<CharacterConfig> AllChars { get; init; }
        public required AutogramInput Input { get; init; }

        public IEnumerable<CharacterConfig> VariableChars => AllChars.Where(p => p.IsVariable);

        /// <summary>
        /// Combines proposed variable counts with fixed character counts into an independent snapshot.
        /// </summary>
        /// <param name="proposedCounts">Counts indexed by each variable character in the same order as <see cref="VariableChars"/>.</param>
        public AutogramSnapshot GetAutogramSnapshot<T>(IReadOnlyList<T> proposedCounts)
            where T : IBinaryInteger<T>
        {
            var variableLetters = this.VariableChars.ToList();

            var charCounts = AllChars.Select(p => (
                p.Char,
                Count: p.IsVariable ? int.CreateChecked(proposedCounts[variableLetters.IndexOf(p)]) : p.MinimumCount
            )).Where(p => p.Count > 0);

            return new AutogramSnapshot(Input, charCounts);
        }

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
    public class CharacterConfig(string separator, string pluralSuffix)
    {
        public required int Index { get; init; }
        public required char Char { get; init; }
        
        /// <summary>
        /// Baseline count of the character <c>Char</c> 
        /// </summary>
        /// <remarks>Characters present in the template and conjunction, before generated list entries.</remarks>
        public required int BaselineCount { get; init; }

        /// <summary>
        /// The guaranteed self count of the character <c>Char</c>.
        /// Literal characters contribute a self-reference when present in the template
        /// or conjunction, or when forced. Word-named characters contribute no literal
        /// self-reference; their names are counted separately.
        /// </summary>
        public int GuaranteedSelfCount => IncludeSelfInCount && (BaselineCount > 0 || Forced) ? 1 : 0;

        public int InvariantMinimumContribution { get; set; }

        public int GuaranteedNameContribution { get; set; }

        /// <summary>
        /// The minimum count of the character <c>Char</c> that is required in the autogram, 
        /// </summary>
        /// <remarks>
        /// * The <c>BaselineCount</c><br/>
        /// * plus 1 if guaranteed to be present, to represent itself in the list.<br/>
        /// * plus the counts of the chars in the cardinals of the invariant characters.<br/>
        /// For invariant chars this is the actual count.
        /// </remarks>
        public int MinimumCount => BaselineCount + GuaranteedSelfCount + InvariantMinimumContribution + GuaranteedNameContribution;
        
        /// <summary>
        /// Gets a value indicating whether the character count is variable, or fixed from the outset.
        /// </summary>
        /// <remarks>
        /// Variable means it is present in the numeric alphabet and can vary.<br/>
        /// Non-Variable chars can have their counts precomputed.</remarks>
        public required bool IsVariable { get; init; }
        
        public int InvariantBaselineContribution { get; set; }

        /// <summary>
        /// The variable baseline count
        /// </summary>
        /// <remarks>
        /// The <c>BaselineCount</c>
        /// Plus the counts of the chars in the cardinals of the invariant characters.
        /// Note that this may be one lower than the MinimumCount - on the basis that the solver loop handles the addition of the letter itself.
        /// </remarks>
        public int? VariableBaselineCount => IsVariable ? BaselineCount + InvariantBaselineContribution : null;

        /// <summary>
        /// Omit the separator from this guaranteed entry's lookup table. Two entries
        /// omit it when a conjunction is used; one omits it for a plain joined list.
        /// </summary>
        public bool OmitSeparator { get; set; }

        public string EntrySeparator => OmitSeparator ? string.Empty : separator;

        public bool IncludeSelfInCount => Char.HasWordName() == false;

        public required bool Forced { get; init; }

        public byte[][] GetStringRepresentationFrequencies(IEnumerable<char> chars)
        {
            var list = this.GetStringRepresentations();
            return list.Select(p => p.GetFrequencies(chars).ToByteArray()).ToArray();
        }

        private IList<string> GetStringRepresentations()
        {
            return Enumerable.Range(0, 100).Select(p => StringRepresentationFor((byte)p)).ToList();
        }

        /// <summary>
        /// Formats a lookup entry, optionally excluding an ordinary character whose
        /// self-reference has already been included in the minimum count.
        /// Word names are always retained in full.
        /// </summary>
        internal string StringRepresentationFor(byte i, bool includeSelf = true)
        {
            var name = Char.GetCharacterName(i, pluralSuffix);
            if (!includeSelf && IncludeSelfInCount)
            {
                name = name[1..];
            }
            return i.ToCardinalNumberStringPrecomputed() + " " + name + EntrySeparator;
        }
    }
}
