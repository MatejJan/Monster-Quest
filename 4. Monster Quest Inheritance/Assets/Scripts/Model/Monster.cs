using System;
using UnityEngine;

namespace MonsterQuest
{
    public class Monster : Creature
    {
        public Monster(string displayName, Sprite bodySprite, int hitPointsMaximum, SizeCategory size, int savingThrowDC) : base(displayName, bodySprite, hitPointsMaximum, size)
        {
            this.savingThrowDC = savingThrowDC;
        }
        
        public int savingThrowDC { get; private set; }
    }
}
