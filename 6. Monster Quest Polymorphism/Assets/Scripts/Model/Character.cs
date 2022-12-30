using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class Character : Creature
    {
        private List<bool> _deathSavingThrows;
        
        public Character(string displayName, Sprite bodySprite, int hitPointsMaximum, SizeCategory sizeCategory, WeaponType weaponType, ArmorType armorType) : base(displayName, bodySprite, sizeCategory)
        {
            this.hitPointsMaximum = hitPointsMaximum;
            this.weaponType = weaponType;
            this.armorType = armorType;
            
            Initialize();
        }
        
        public WeaponType weaponType { get; private set; }
        public ArmorType armorType { get; private set; }

        public override bool[] deathSavingThrows => _deathSavingThrows.ToArray();

        protected override IEnumerator TakeDamageAtZeroHP(int remainingDamageAmount)
        {
            yield return presenter.Die();
        }
    }
}
