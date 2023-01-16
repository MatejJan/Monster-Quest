using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonsterQuest
{
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

        public int hitPoints { get; protected set; }
        public CreaturePresenter presenter { get; private set; }

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
        
        public IEnumerator ReactToDamage(int damageAmount)
        {
            hitPoints -= damageAmount;
            
            if (hitPoints <= 0)
            {
                yield return TakeDamageAtZeroHP();
            }
            else
            {
                Console.WriteLine($"{displayName} has {hitPoints} HP left.");
                yield return presenter.TakeDamage();
            }
        }
        
        protected virtual IEnumerator TakeDamageAtZeroHP()
        {
            lifeStatus = LifeStatus.Dead;
            yield return presenter.TakeDamage(hitPoints <= -hitPointsMaximum);
            yield return presenter.Die();
        }
    }
}
