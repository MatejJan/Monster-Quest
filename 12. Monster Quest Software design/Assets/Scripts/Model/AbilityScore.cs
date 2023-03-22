using System;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class AbilityScore
    {   
        [field: SerializeField] public int score { get; set; }
        
        public int modifier => Mathf.FloorToInt((score - 10) / 2f);

        public static implicit operator int(AbilityScore abilityScore) => abilityScore.score;
    }
}
