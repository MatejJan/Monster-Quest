using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
    public class MonsterType : ScriptableObject
    {
        public string displayName;
        public SizeCategory sizeCategory;
        public string alignment;
        public string hitPointsRoll;
        public int armorClass;
        public ArmorType armorType;
        public WeaponType[] weaponTypes;
        public Sprite bodySprite;
    }
}
