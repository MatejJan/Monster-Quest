using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public static class EnglishHelper
    {
        public static readonly char[] vowels =
        {
            'a',
            'e',
            'i',
            'o',
            'u'
        };

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

            // If the noun starts with a vowel add "an" to it, otherwise "a".
            return $"{(StartsWithVowel(noun) ? "an" : "a")} {noun}";
        }

        public static string GetPluralNounForm(string noun)
        {
            // Apply the basic rule of adding an "s" to the end of the noun.
            return $"{noun}s";
        }

        public static string GetNounWithCount(string noun, int count)
        {
            return $"{count} {(count > 1 ? GetPluralNounForm(noun) : noun)}";
        }

        private static bool StartsWithVowel(string word)
        {
            return Array.IndexOf(vowels, word[0]) > -1;
        }

        private static bool StartsWithCapital(string word)
        {
            string firstLetter = word[..1];

            return firstLetter == firstLetter.ToUpper();
        }

        public static string JoinWithAnd(IEnumerable items, bool useSerialComma = true)
        {
            string[] itemsList = (from object item in items select item.ToString()).ToArray();

            int count = itemsList.Length;

            switch (count)
            {
                case 0:
                    return "";

                case 1:
                    return itemsList[0];

                case 2:
                    return $"{itemsList[0]} and {itemsList[1]}";
            }

            List<string> itemsCopy = new(itemsList);

            if (useSerialComma)
            {
                itemsCopy[count - 1] = $"and {itemsList[count - 1]}";
            }
            else
            {
                itemsCopy[count - 2] = $"{itemsList[count - 2]} and {itemsList[count - 1]}";
                itemsCopy.RemoveAt(count - 1);
            }

            return string.Join(", ", itemsCopy);
        }
    }
}
