using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu]
    public class MonsterType : ScriptableObject
    {
        public string displayName;
        public SizeCategory sizeCategory;
        public string alignment;
        public string hitPointsRoll;
        public int armorClass;
        public AbilityScores abilityScores;
        public ArmorType armorType;
        public WeaponType[] weaponTypes;
        public Sprite bodySprite;
        public float challengeRating;
        
        public int experiencePoints
        {
            get
            {
                return challengeRating switch
                {
                    0 => hasEffectiveAttacks ? 10 : 0,
                    0.125f => 25,
                    0.25f => 50,
                    0.5f => 100,
                    1 => 200,
                    2 => 450,
                    3 => 700,
                    4 => 1100,
                    5 => 1800,
                    6 => 2300,
                    7 => 2900,
                    8 => 3900,
                    9 => 5000,
                    10 => 5900,
                    11 => 7200,
                    12 => 8400,
                    13 => 10000,
                    14 => 11500,
                    15 => 13000,
                    16 => 15000,
                    17 => 18000,
                    18 => 20000,
                    19 => 22000,
                    20 => 25000,
                    21 => 33000,
                    22 => 41000,
                    23 => 50000,
                    24 => 62000,
                    25 => 75000,
                    26 => 90000,
                    27 => 105000,
                    28 => 120000,
                    29 => 135000,
                    30 => 155000,
                    _ => 0
                };
            }
        }

        private bool hasEffectiveAttacks => weaponTypes.Length > 0;
    }
}
