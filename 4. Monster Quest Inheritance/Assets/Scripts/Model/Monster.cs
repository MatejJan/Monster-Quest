using System;
using UnityEngine;

namespace MonsterQuest
{
    public class Monster : Creature
    {
        public Monster(string displayName, Sprite bodySprite, int hitPointsMaximum, SizeCategory sizeCategory, int savingThrowDC) : base(displayName, bodySprite, hitPointsMaximum, sizeCategory)
        {
            this.savingThrowDC = savingThrowDC;
        }
        
        public int savingThrowDC { get; private set; }
    }
}
