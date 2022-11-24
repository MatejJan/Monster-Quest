using System.Collections.Generic;

namespace MonsterQuest
{
    public static class SizeHelper
    {
        public static readonly Dictionary<SizeCategory, float> spaceInFeetPerSizeCategory;

        static SizeHelper()
        {
            // Define how much space each size category takes up.
            spaceInFeetPerSizeCategory = new Dictionary<SizeCategory, float>
            {
                { SizeCategory.Tiny, 2.5f },
                { SizeCategory.Small, 5f },
                { SizeCategory.Medium, 5f },
                { SizeCategory.Large, 10f },
                { SizeCategory.Huge, 15f },
                { SizeCategory.Gargantuan, 20f }
            };
        }
    }
}
