using System.Collections.Generic;

namespace MonsterQuest
{
    public static class CharacterRules
    {
        private static readonly Dictionary<int, int> _levelExperiencePointsRequirements = new();

        static CharacterRules()
        {
            _levelExperiencePointsRequirements[1] = 0;
            _levelExperiencePointsRequirements[2] = 300;
            _levelExperiencePointsRequirements[3] = 900;
            _levelExperiencePointsRequirements[4] = 2700;
            _levelExperiencePointsRequirements[5] = 6500;
            _levelExperiencePointsRequirements[6] = 14000;
            _levelExperiencePointsRequirements[7] = 23000;
            _levelExperiencePointsRequirements[8] = 34000;
            _levelExperiencePointsRequirements[9] = 64000;
            _levelExperiencePointsRequirements[10] = 64000;
            _levelExperiencePointsRequirements[11] = 85000;
            _levelExperiencePointsRequirements[12] = 100000;
            _levelExperiencePointsRequirements[13] = 120000;
            _levelExperiencePointsRequirements[14] = 140000;
            _levelExperiencePointsRequirements[15] = 165000;
            _levelExperiencePointsRequirements[16] = 195000;
            _levelExperiencePointsRequirements[17] = 225000;
            _levelExperiencePointsRequirements[18] = 265000;
            _levelExperiencePointsRequirements[19] = 305000;
            _levelExperiencePointsRequirements[20] = 355000;
        }

        public static int GetLevelForExperiencePoints(int experiencePoints)
        {
            for (int level = 20; level > 1; level--)
            {
                if (experiencePoints >= _levelExperiencePointsRequirements[level]) return level;
            }

            return 1;
        }

        public static int GetExperiencePointsForLevel(int level)
        {
            return _levelExperiencePointsRequirements[level];
        }
    }
}
