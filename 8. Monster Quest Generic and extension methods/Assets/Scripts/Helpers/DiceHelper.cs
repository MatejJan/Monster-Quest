using System;
using System.Text.RegularExpressions;

namespace MonsterQuest
{
    public static class DiceHelper
    {
        private static int Roll(int numberOfRolls, int diceSides, int fixedBonus = 0)
        {
            var random = new Random();

            int result = fixedBonus;

            for (var i = 0; i < numberOfRolls; i++)
            {
                result += random.Next(1, diceSides + 1);
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

            return Roll(numberOfRolls, diceSides, fixedBonus);
        }
    }
}
