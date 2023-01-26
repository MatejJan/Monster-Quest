using System.Linq;

namespace MonsterQuest
{
    public static class StringHelper
    {
        public static string ToUpperFirst(this string s)
        {
            if (string.IsNullOrEmpty(s)) return "";

            return $"{char.ToUpper(s[0])}{s[1..]}";
        }

        public static string ToStartCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return "";

            string[] words = s.Split(" ");

            return string.Join(" ", words.Select(word => word.ToUpperFirst()));
        }
    }
}
