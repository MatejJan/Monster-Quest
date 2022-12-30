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
        
        public abstract bool[] deathSavingThrows { get; }
        public int deathSavingThrowSuccesses => deathSavingThrows.Count(result => result);
        public int deathSavingThrowFailures => deathSavingThrows.Count(result => !result);
        
        public LifeStatus lifeStatus
        {
            get => _lifeStatus;
            protected set
            {
                _lifeStatus = value;
                if (presenter is not null) presenter.UpdateStableStatus();
            }
        }
        
        protected void Initialize()
        {
            hitPoints = hitPointsMaximum;
        }
        
        public void InitializePresenter(CreaturePresenter creaturePresenter)
        {
            presenter = creaturePresenter;
        }
        
        public IEnumerator ReactToDamage(int damageAmount)
        {
            hitPoints -= damageAmount;
            
            if (hitPoints <= 0)
            {
                int remainingDamageAmount = Math.Abs(hitPoints);
                hitPoints = 0;

                TakeDamageAtZeroHP(remainingDamageAmount);
            }
            else
            {
                yield return presenter.TakeDamage();
            }
        }
        
        protected abstract IEnumerator TakeDamageAtZeroHP(int remainingDamageAmount);
    }
}
