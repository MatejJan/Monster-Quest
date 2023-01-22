namespace MonsterQuest
{
    public static class StringHelper
    {
        public static string ToUpperFirst(this string s)
        {
            return $"{char.ToUpper(s[0])}{s[1..]}";
        }
    }
}
