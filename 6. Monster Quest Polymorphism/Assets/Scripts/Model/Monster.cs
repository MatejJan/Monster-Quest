using System;
using System.Collections;
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
        public override bool[] deathSavingThrows => _deathSavingThrows;
        
        protected override IEnumerator TakeDamageAtZeroHP(int remainingDamageAmount)
        {
            lifeStatus = LifeStatus.Dead;
            yield return presenter.TakeDamage(remainingDamageAmount >= hitPointsMaximum);
            yield return presenter.Die();
        }
    }
}
