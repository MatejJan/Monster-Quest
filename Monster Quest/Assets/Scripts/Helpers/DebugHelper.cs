namespace MonsterQuest
{
    public static class DebugHelper
    {
        public static void Log(string text)
        {
            Console.WriteLine(text);
        }

        public static void StartLog(string text = "")
        {
            Console.Indent(true);
            if (text.Length > 0) Console.WriteLine(text);
        }

        public static void EndLog(string text = "")
        {
            if (text.Length > 0) Console.WriteLine(text);
            Console.Outdent();
        }
    }
}
