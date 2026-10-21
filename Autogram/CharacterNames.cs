using System.Collections.ObjectModel;

namespace Autogram
{
    internal static class CharacterNames
    {
        internal static readonly IReadOnlyDictionary<char, (string Singular, string Plural)> NamedCharacters =
            new ReadOnlyDictionary<char, (string Singular, string Plural)>(
                new Dictionary<char, (string Singular, string Plural)>
                {
                    [','] = ("comma", "commas"),
                    ['-'] = ("hyphen", "hyphens"),
                    ['\''] = ("apostrophe", "apostrophes"),
                    [' '] = ("space", "spaces"),
                });
    }
}
