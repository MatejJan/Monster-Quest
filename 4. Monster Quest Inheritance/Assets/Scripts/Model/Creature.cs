using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonsterQuest
{
    public abstract class Creature
    {
        public Creature(string displayName, Sprite bodySprite, int hitPointsMaximum, SizeCategory sizeCategory)
        {
            this.displayName = displayName;
            this.bodySprite = bodySprite;
            this.hitPointsMaximum = hitPointsMaximum;
            this.sizeCategory = sizeCategory;

            hitPoints = hitPointsMaximum;
        }
        
        public string displayName { get; private set; }
        public Sprite bodySprite { get; private set; }
        public int hitPointsMaximum { get; private set; }
        public SizeCategory sizeCategory { get; private set; }

        public int hitPoints { get; private set; }
        public CreaturePresenter presenter { get; private set; }

        public float spaceInFeet => SizeHelper.spaceInFeetPerSizeCategory[sizeCategory];
        
        public void InitializePresenter(CreaturePresenter creaturePresenter)
        {
            presenter = creaturePresenter;
        }
        
        public IEnumerator ReactToDamage(int damageAmount)
        {
            hitPoints = Math.Max(0, hitPoints - damageAmount);

            if (hitPoints == 0)
            {
                yield return presenter.Die();
            }
            else
            {
                yield return presenter.TakeDamage();
            }
            
        }
    }
}
