using System;

namespace MonsterQuest
{
    public static class CreatureRules
    {
        public static int GetProficiencyBonus(int bonusBase)
        {
            return 2 + Math.Max(0, (bonusBase - 1) / 4);
        }
    }
}
