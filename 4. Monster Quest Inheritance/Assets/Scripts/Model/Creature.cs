using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonsterQuest
{
    public abstract class Creature
    {
        public Creature(string displayName, Sprite bodySprite, int hitPointsMaximum, SizeCategory size)
        {
            this.displayName = displayName;
            this.bodySprite = bodySprite;
            this.hitPointsMaximum = hitPointsMaximum;
            this.size = size;

            hitPoints = hitPointsMaximum;
        }
        
        public string displayName { get; private set; }
        public Sprite bodySprite { get; private set; }
        public int hitPointsMaximum { get; private set; }
        public SizeCategory size { get; private set; }

        public int hitPoints { get; private set; }
        public CreaturePresenter presenter { get; private set; }

        public float spaceTaken => SizeHelper.spaceTakenPerSize[size];
        
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
