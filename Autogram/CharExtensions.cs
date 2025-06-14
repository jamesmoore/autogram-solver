namespace Autogram
{
    internal static class CharExtensions
    {
        public static string GetCharacterName(this char character)
        {
            if (character == ',') return "comma";
            if (character == '-') return "hyphen";
            if (character == '\'') return "apostrophe";
            return character.ToString();
        }

        public static string GetPluralisedCharacterName(this char character)
        {
            if (character == ',') return "commas";
            if (character == '-') return "hyphens";
            if (character == '\'') return "apostrophes";
            return character + "\'s";
        }

        public static string GetCharacterName(this char character, int numberOf)
        {
            if(numberOf == 1)
            {
                return character.GetCharacterName();
            }
            else
            {
                return character.GetPluralisedCharacterName();
            }
        }

        public static bool HasExtendedName(this char character)
        {
            return character.ToString() != character.GetCharacterName();
        }
    }
}
