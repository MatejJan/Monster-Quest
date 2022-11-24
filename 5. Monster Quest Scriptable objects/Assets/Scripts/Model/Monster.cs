using System;
using UnityEngine;

namespace MonsterQuest
{
    public class Monster : Creature
    {
        public Monster(MonsterType type) : base(type.displayName, type.bodySprite, type.sizeCategory)
        {
            this.type = type;

            hitPointsMaximum = DiceHelper.Roll(type.hitPointsRoll);
            
            Initialize();
        }
        
        public MonsterType type { get; private set; }
    }
}
