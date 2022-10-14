using System;
using System.Text.RegularExpressions;
using Random = UnityEngine.Random;

namespace MonsterQuest
{
    public static class Dice
    {
        private static int Roll(int numberOfRolls, int diceSides, int fixedBonus = 0, int multiplier = 1, int divider = 1)
        {
            if (Console.verbose)
            {
                Console.Indent();
                Console.Write($"Rolling {numberOfRolls}d{diceSides}{(fixedBonus == 0 ? "" : fixedBonus.ToString("+#;-#;"))}{(multiplier > 1 ? $"*{multiplier}" : "")}{(divider > 1 ? $"/{divider}" : "")} …");
            }

            int result = fixedBonus;

            for (int i = 0; i < numberOfRolls; i++)
            {
                int roll = Random.Range(1, diceSides + 1);
                if (Console.verbose) Console.Write($" {roll}");

                result += roll;
            }

            result = result * multiplier / divider;

            if (Console.verbose)
            {
                Console.WriteLine($" = {result}");
                Console.Outdent();
            }

            return result;
        }

        public static int Roll(string diceNotation)
        {
            Match match = Regex.Match(diceNotation, @"(\d+)?d(\d+)([+-]\d+)?(?:\*(\d+))?(?:\/(\d+))?");

            if (!match.Success) throw new ArgumentException($"Invalid dice notation was provided ({diceNotation}).");

            int numberOfRolls = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 1;
            int diceSides = int.Parse(match.Groups[2].Value);
            int fixedBonus = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;
            int multiplier = match.Groups[4].Success ? int.Parse(match.Groups[4].Value) : 1;
            int divider = match.Groups[5].Success ? int.Parse(match.Groups[5].Value) : 1;

            return Roll(numberOfRolls, diceSides, fixedBonus, multiplier, divider);
        }

        public static string GetRollWithHalfTheDice(string diceNotation)
        {
            // Extract the number before the d.
            int dIndex = diceNotation.IndexOf('d');

            if (dIndex <= 0) throw new ArgumentException("Dice roll has only one dice and cannot be halved.");

            int numberOfRolls = int.Parse(diceNotation[..dIndex]);

            if (numberOfRolls % 2 == 1) throw new ArgumentException("Dice roll has an odd amount of dice and cannot be halved.");

            return $"{numberOfRolls / 2}d{diceNotation[(dIndex + 1)..]}";
        }
    }
}
