using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class Character : Creature
    {
        private readonly List<bool> _deathSavingThrows = new();
        
        public Character(string displayName, Sprite bodySprite, int hitPointsMaximum, SizeCategory sizeCategory, WeaponType weaponType, ArmorType armorType) : base(displayName, bodySprite, sizeCategory)
        {
            this.hitPointsMaximum = hitPointsMaximum;
            this.weaponType = weaponType;
            this.armorType = armorType;
            
            List<int> availableAbilityScores = new ();

            for (var i = 0; i < 6; i++)
            {
                var rolls = new List<int>();

                for (var j = 0; j < 4; j++)
                {
                    rolls.Add(Random.Range(1, 7));
                }

                rolls.Sort();
                int score = rolls[1] + rolls[2] + rolls[3];

                availableAbilityScores.Add(score);
            }

            abilityScores.strength.score = availableAbilityScores[0];
            abilityScores.dexterity.score = availableAbilityScores[1];
            abilityScores.constitution.score = availableAbilityScores[2];
            abilityScores.intelligence.score = availableAbilityScores[3];
            abilityScores.wisdom.score = availableAbilityScores[4];
            abilityScores.charisma.score = availableAbilityScores[5];

            Initialize();
        }
        
        public WeaponType weaponType { get; }
        public ArmorType armorType { get; }

        public override AbilityScores abilityScores { get; } = new();

        public override IEnumerable<bool> deathSavingThrows => _deathSavingThrows;
        public override int armorClass => armorType.armorClass;

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
