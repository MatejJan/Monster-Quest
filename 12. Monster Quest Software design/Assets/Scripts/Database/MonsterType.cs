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
    }
}
