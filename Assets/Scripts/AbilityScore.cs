using System;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class AbilityScore
    {
        [field: SerializeField] public int score { get; set; }

        public int modifier => (score - 10) / 2;

        public static implicit operator int(AbilityScore abilityScore)
        {
            return abilityScore.score;
        }
    }
}
