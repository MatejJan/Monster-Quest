using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class Character : Creature
    {
        private List<bool> _deathSavingThrows = new();
        
        public Character(string displayName, Sprite bodySprite, int hitPointsMaximum, SizeCategory sizeCategory, WeaponType weaponType, ArmorType armorType) : base(displayName, bodySprite, sizeCategory)
        {
            this.hitPointsMaximum = hitPointsMaximum;
            this.weaponType = weaponType;
            this.armorType = armorType;
            
            Initialize();
        }
        
        public WeaponType weaponType { get; private set; }
        public ArmorType armorType { get; private set; }

        public override IEnumerable<bool> deathSavingThrows => _deathSavingThrows;

        protected override IEnumerator TakeDamageAtZeroHP()
        {
            if (hitPoints <= -hitPointsMaximum)
            {
                Console.WriteLine($"{displayName} takes so much damage they immediately die.");
                yield return base.TakeDamageAtZeroHP();
                yield break;
            }

            hitPoints = 0;
            
            if (lifeStatus == LifeStatus.Alive)
            {
                Console.WriteLine($"{displayName} falls unconscious.");
                lifeStatus = LifeStatus.UnstableUnconscious;
                yield return presenter.TakeDamage();
                yield break;
            }
            
            Console.WriteLine($"{displayName} fails a death saving throw.");
            lifeStatus = LifeStatus.UnstableUnconscious;
            yield return presenter.TakeDamage();
            
            _deathSavingThrows.Add(false);
            yield return presenter.PerformDeathSavingThrow(false);
            
            if (deathSavingThrowFailures >= 3)
            {
                Console.WriteLine($"{displayName} meets an untimely end.");
                lifeStatus = LifeStatus.Dead;
                yield return presenter.Die();
            }
        }
    }
}
