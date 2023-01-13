using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class Monster : Creature
    {
        private static readonly bool[] _deathSavingThrows = Array.Empty<bool>();
        
        public Monster(MonsterType type) : base(type.displayName, type.bodySprite, type.sizeCategory)
        {
            this.type = type;

            hitPointsMaximum = DiceHelper.Roll(type.hitPointsRoll);
            
            Initialize();
        }
        
        public MonsterType type { get; private set; }
        public override IEnumerable<bool> deathSavingThrows => _deathSavingThrows;
    }
}
