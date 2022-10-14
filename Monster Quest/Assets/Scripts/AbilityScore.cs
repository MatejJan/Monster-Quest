using System;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class AbilityScore
    {
        // State properties
        [field: SerializeField] public int score { get; set; }

        // Derived properties
        public int modifier => (score - 10) / 2;

        // Allow the ability score to be used directly as an integer.
        public static implicit operator int(AbilityScore abilityScore)
        {
            return abilityScore.score;
        }
    }
}
