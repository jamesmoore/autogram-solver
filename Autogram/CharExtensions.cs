namespace Autogram
{
    public static class CharExtensions
    {
        private static string GetCharacterName(this char character)
        {
            if (character == ',') return "comma";
            if (character == '-') return "hyphen";
            if (character == '\'') return "apostrophe";
            if (character == ' ') return "space";
            return character.ToString();
        }

        private static string GetPluralisedCharacterName(this char character, string pluralSuffix)
        {
            if (character == ',') return "commas";
            if (character == '-') return "hyphens";
            if (character == '\'') return "apostrophes";
            if (character == ' ') return "spaces";
            return character + pluralSuffix;
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

        public static bool HasExtendedName(this char character)
        {
            return character.ToString() != character.GetCharacterName();
        }
    }
}
