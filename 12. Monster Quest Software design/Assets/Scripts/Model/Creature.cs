using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public abstract class Creature
    {
        private LifeStatus _lifeStatus;
        
        public Creature(string displayName, Sprite bodySprite, SizeCategory sizeCategory)
        {
            this.displayName = displayName;
            this.bodySprite = bodySprite;
            this.sizeCategory = sizeCategory;
        }

        public string displayName { get; protected set; }
        public Sprite bodySprite { get; protected set; }
        public int hitPointsMaximum { get; protected set; }
        public SizeCategory sizeCategory { get; protected set; }

        public abstract AbilityScores abilityScores { get; }
        public int hitPoints { get; protected set; }
        [field: NonSerialized] public CreaturePresenter presenter { get; private set; }

        public float spaceInFeet => SizeHelper.spaceInFeetPerSizeCategory[sizeCategory];
        
        public abstract IEnumerable<bool> deathSavingThrows { get; }
        public int deathSavingThrowSuccesses => deathSavingThrows.Count(result => result);
        public int deathSavingThrowFailures => deathSavingThrows.Count(result => !result);
        
        public abstract int armorClass { get; }
        
        public LifeStatus lifeStatus
        {
            get => _lifeStatus;
            protected set
            {
                _lifeStatus = value;
                if (presenter is not null) presenter.UpdateStableStatus();
            }
        }
        
        public bool isUnconscious => lifeStatus is LifeStatus.UnconsciousUnstable or LifeStatus.UnconsciousStable;
        public bool isAlive => lifeStatus is not LifeStatus.Dead;
        
        protected void Initialize()
        {
            hitPoints = hitPointsMaximum;
        }
        
        public void InitializePresenter(CreaturePresenter creaturePresenter)
        {
            presenter = creaturePresenter;
        }

        public abstract IAction TakeTurn(GameState gameState);

        protected AttackAction CreateAttack(Creature target, WeaponType weaponType)
        {
            Ability? ability = null;

            if (weaponType.isFinesse) ability = abilityScores.strength > abilityScores.dexterity ? Ability.Strength : Ability.Dexterity;
            
            return new AttackAction(this, target, weaponType, ability);
        }
        
        public IEnumerator ReactToDamage(int damageAmount, bool wasCriticalHit)
        {
            hitPoints -= damageAmount;
            
            if (hitPoints <= 0)
            {
                yield return TakeDamageAtZeroHitPoints(wasCriticalHit);
            }
            else
            {
                Console.WriteLine($"{displayName.ToUpperFirst()} has {hitPoints} HP left.");
                yield return presenter.TakeDamage();
            }
        }
        
        protected virtual IEnumerator TakeDamageAtZeroHitPoints(bool wasCriticalHit)
        {
            lifeStatus = LifeStatus.Dead;
            yield return presenter.TakeDamage(hitPoints <= -hitPointsMaximum);
            yield return presenter.Die();
        }
        
        public virtual IEnumerator Heal(int amount)
        {
            hitPoints = Math.Min(hitPoints + amount, hitPointsMaximum);

            Console.Write($"{displayName.ToUpperFirst()} heals {amount} HP and ");

            if (lifeStatus == LifeStatus.Conscious)
            {
                Console.WriteLine($"is at {(hitPoints == hitPointsMaximum ? "full health" : $"{hitPoints} HP")}.");
            }
            else
            {
                lifeStatus = LifeStatus.Conscious;

                Console.WriteLine($"regains consciousness. They are at {(hitPoints == hitPointsMaximum ? "full health" : $"{hitPoints} HP")}.");

                if (presenter is not null) yield return presenter.RegainConsciousness();
            }

            if (presenter is not null) yield return presenter.Heal();
        }
        
        public int MakeAbilityRoll(Ability ability)
        {
            return DiceHelper.Roll("d20") + abilityScores[ability].modifier;
        }
    }
}
