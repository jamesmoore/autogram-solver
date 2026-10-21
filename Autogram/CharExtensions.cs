namespace Autogram
{
    public static class CharExtensions
    {
        private static string GetCharacterName(this char character)
        {
            return CharacterNames.NamedCharacters.TryGetValue(character, out var names)
                ? names.Singular
                : character.ToString();
        }

        private static string GetPluralisedCharacterName(this char character, string pluralSuffix)
        {
            return CharacterNames.NamedCharacters.TryGetValue(character, out var names)
                ? names.Plural
                : character + pluralSuffix;
        }

        public static string GetCharacterName(this char character, int numberOf, string pluralSuffix)
        {
            if (numberOf == 1)
            {
                return character.GetCharacterName();
            }
            else
            {
                return character.GetPluralisedCharacterName(pluralSuffix);
            }
        }

        public static bool HasWordName(this char character)
        {
            return CharacterNames.NamedCharacters.ContainsKey(character);
        }
    }
}
