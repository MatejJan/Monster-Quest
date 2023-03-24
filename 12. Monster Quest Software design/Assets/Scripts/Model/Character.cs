using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonsterQuest
{
    [Serializable]
    public class Character : Creature
    {
        private readonly List<bool> _deathSavingThrows = new();
        
        public Character(string displayName, Sprite bodySprite, SizeCategory sizeCategory, WeaponType weaponType, ArmorType armorType, ClassType classType) : base(displayName, bodySprite, sizeCategory)
        {
            this.weaponType = weaponType;
            this.armorType = armorType;
            this.classType = classType;
            
            for (var i = 0; i < 6; i++)
            {
                var rolls = new List<int>();

                for (var j = 0; j < 4; j++)
                {
                    rolls.Add(Random.Range(1, 7));
                }

                rolls.Sort();
                int score = rolls[1] + rolls[2] + rolls[3];

                abilityScores[(Ability)(i+1)].score = score;
            }

            experiencePoints = 0;
            level = 1;
            availableHitDice = level;
            hitPointsMaximum = classType.hitDiceSides + abilityScores.constitution.modifier;

            Initialize();
        }

        public int experiencePoints { get; private set; }
        public int level { get; private set; }
        public int availableHitDice { get; private set; }
        public WeaponType weaponType { get; }
        public ArmorType armorType { get; }

        public override AbilityScores abilityScores { get; } = new();

        public override IEnumerable<bool> deathSavingThrows => _deathSavingThrows;
        public override int armorClass => armorType.armorClass;

        protected override int proficiencyBonusBase => level;
        
        public ClassType classType { get; }

        public override bool IsProficientWithWeaponType(WeaponType weaponType)
        {
            return classType.weaponProficiencies.Intersect(weaponType.categories).Any();
        } 

        public override IAction TakeTurn(GameState gameState)
        {
            if (lifeStatus != LifeStatus.Conscious)
            {
                return new BeUnconsciousAction(this);
            }
            
            return CreateAttack(gameState.combat.monster, weaponType);
        }
        
        protected override IEnumerator TakeDamageAtZeroHitPoints(bool wasCriticalHit)
        {
            if (hitPoints <= -hitPointsMaximum)
            {
                Console.WriteLine($"{displayName.ToUpperFirst()} takes so much damage they immediately die.");
                yield return base.TakeDamageAtZeroHitPoints(wasCriticalHit);
                yield break;
            }

            hitPoints = 0;
            
            if (lifeStatus == LifeStatus.Conscious)
            {
                Console.WriteLine($"{displayName.ToUpperFirst()} falls unconscious.");
                lifeStatus = LifeStatus.UnconsciousUnstable;
                yield return presenter.TakeDamage();
                yield break;
            }
            
            Console.WriteLine($"{displayName.ToUpperFirst()} fails a death saving throw.");
            lifeStatus = LifeStatus.UnconsciousUnstable;
            yield return presenter.TakeDamage();

            int deathSavingThrowFailuresCount = wasCriticalHit ? 2 : 1;

            yield return ApplyDeathSavingThrows(deathSavingThrowFailuresCount, false);

            yield return HandleDeathSavingThrows();
        }
        
        public override IEnumerator Heal(int amount)
        {
            // Characters reset saving throws when healing.
            ResetDeathSavingThrows();

            yield return base.Heal(amount);
        }
        
        public IEnumerator HandleUnconsciousState()
        {
            // Unstable unconscious characters must make a death saving throw.
            if (lifeStatus != LifeStatus.UnconsciousUnstable) yield break;

            int deathSavingThrowRollResult = DiceHelper.Roll("d20");

            switch (deathSavingThrowRollResult)
            {
                case 1:
                    Console.WriteLine($"{displayName.ToUpperFirst()} critically fails a death saving throw.");

                    // Critical fails add 2 saving throw failures.
                    yield return ApplyDeathSavingThrows(2, false, deathSavingThrowRollResult);

                    break;

                case 20:
                    // Critical successes regain consciousness with 1 HP.
                    Console.WriteLine($"{displayName.ToUpperFirst()} critically succeeds a death saving throw.");

                    yield return ApplyDeathSavingThrows(1, true, deathSavingThrowRollResult);

                    ResetDeathSavingThrows();

                    yield return Heal(1);

                    break;

                case < 10:
                    Console.WriteLine($"{displayName.ToUpperFirst()} fails a death saving throw.");

                    yield return ApplyDeathSavingThrows(1, false, deathSavingThrowRollResult);

                    break;

                default:
                    Console.WriteLine($"{displayName.ToUpperFirst()} succeeds a death saving throw.");

                    yield return ApplyDeathSavingThrows(1, true, deathSavingThrowRollResult);

                    break;
            }

            yield return HandleDeathSavingThrows();
        }

        public IEnumerator GainExperiencePoints(int amount)
        {
            Console.WriteLine($"{displayName.ToUpperFirst()} gains {amount} experience points.");

            if (presenter is not null && lifeStatus == LifeStatus.Conscious) presenter.GainExperiencePoints();
            
            experiencePoints += amount;
            
            // Determine new level based on new experience points.
            int newLevel = CharacterRules.GetLevelForExperiencePoints(experiencePoints);

            while(level < newLevel)
            {
                yield return LevelUp();
            }
        }

        private IEnumerator LevelUp()
        {
            level++;
            availableHitDice++;
            int hitDiceRoll = DiceHelper.Roll(classType.hitDice);
            int hitPointsIncrease = Math.Max(1, hitDiceRoll + abilityScores.constitution.modifier);
            hitPointsMaximum += hitPointsIncrease;
            
            Console.WriteLine($"{displayName.ToUpperFirst()} levels up to level {level}! Their maximum HP increases to {hitPointsMaximum}.");

            if (presenter is not null && isAlive) yield return presenter.LevelUp();
        }

        public IEnumerator TakeShortRest()
        {
            while (hitPoints <= hitPointsMaximum * 0.75f && availableHitDice > 0)
            {
                yield return SpendHitDice();
            }
        }

        private IEnumerator SpendHitDice()
        {
            availableHitDice--;
            int healAmount = Math.Max(0, DiceHelper.Roll(classType.hitDice) + abilityScores.constitution.modifier);
            yield return Heal(healAmount);
        }
        
        private IEnumerator HandleDeathSavingThrows()
        {
            // If the character fails 3 death saving throws, they die.
            if (deathSavingThrowFailures >= 3)
            {
                Console.WriteLine($"{displayName.ToUpperFirst()} meets an untimely end.");
                lifeStatus = LifeStatus.Dead;
                yield return presenter.Die();
            }

            // If the character succeeds 3 death saving throws, they stabilize.
            if (deathSavingThrowSuccesses >= 3)
            {
                Console.WriteLine($"{displayName.ToUpperFirst()} stabilizes.");
                lifeStatus = LifeStatus.UnconsciousStable;
                ResetDeathSavingThrows();
            }
        }

        private IEnumerator ApplyDeathSavingThrows(int amount, bool success, int? rollResult = null)
        {
            for (int i = 0; i < amount; i++)
            {
                _deathSavingThrows.Add(success);

                yield return presenter.PerformDeathSavingThrow(success, i == 0 ? rollResult : null);
            }
        }

        private void ResetDeathSavingThrows()
        {
            _deathSavingThrows.Clear();
            if (presenter is not null) presenter.ResetDeathSavingThrows();
        }
    }
}
