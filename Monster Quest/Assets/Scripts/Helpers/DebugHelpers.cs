namespace MonsterQuest
{
    public static class DebugHelpers
    {
        public static void Log(string text)
        {
            if (!Console.verbose) return;

            Console.WriteLine(text);
        }

        public static void StartLog(string text = "")
        {
            if (!Console.verbose) return;

            Console.Indent();
            if (text.Length > 0) Console.WriteLine(text);
        }

        public static void EndLog(string text = "")
        {
            if (!Console.verbose) return;

            if (text.Length > 0) Console.WriteLine(text);
            Console.Outdent();
        }
    }
}
