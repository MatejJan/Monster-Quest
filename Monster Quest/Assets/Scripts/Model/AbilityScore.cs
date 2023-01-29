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
        public int modifier => Mathf.FloorToInt((score - 10) / 2f);

        // Allow the ability score to be used directly as an integer.
        public static implicit operator int(AbilityScore abilityScore)
        {
            return abilityScore.score;
        }
    }
}
