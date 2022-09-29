using System;

namespace MonsterQuest
{
    public static class EnglishHelpers
    {
        public static readonly char[] vowels = { 'a', 'e', 'i', 'o', 'u' };

        public static string GetDefiniteNounForm(string noun)
        {
            // If the noun starts with a capital, it's already definite.
            if (StartsWithCapital(noun)) return noun;

            // Add "the" to the noun.
            return $"the {noun}";
        }

        public static string GetIndefiniteNounForm(string noun)
        {
            // If the noun starts with a capital, we can leave it as is.
            if (StartsWithCapital(noun)) return noun;

            // If the noun starts with a vowel, add "an" to it.
            if (StartsWithVowel(noun)) return $"an {noun}";

            // Add "a" to the noun.
            return $"a {noun}";
        }

        private static bool StartsWithVowel(string word)
        {
            return Array.IndexOf(vowels, word[0]) > -1;
        }

        private static bool StartsWithCapital(string word)
        {
            string firstLetter = word.Substring(0, 1);

            return firstLetter == firstLetter.ToUpper();
        }
    }
}
