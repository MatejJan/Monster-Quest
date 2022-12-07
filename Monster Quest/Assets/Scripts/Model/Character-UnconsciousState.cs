using System.Collections;
using UnityEngine;

namespace MonsterQuest
{
    public partial class Character
    {
        [SerializeField] private int _deathSavingThrowFailures;
        [SerializeField] private int _deathSavingThrowSuccesses;

        public IEnumerator HandleUnconsciousState()
        {
            // Unstable unconscious characters must make a death saving throw.
            if (lifeStatus != LifeStatus.UnstableUnconscious) yield break;

            int deathSavingThrow = DiceHelper.Roll("d20");

            switch (deathSavingThrow)
            {
                case 1:
                    // Critical fails add 2 saving throw failures.
                    _deathSavingThrowFailures += 2;

                    yield return PresentDeathSavingThrows(2, false);

                    Console.WriteLine($"{definiteName.ToUpperFirst()} critically fails a death saving throw.");

                    break;

                case 20:
                    // Critical successes regain consciousness with 1 HP.
                    Console.WriteLine($"{definiteName.ToUpperFirst()} critically succeeds a death saving throw.");

                    if (presenter is not null) presenter.ResetDeathSavingThrows();

                    yield return Heal(1);

                    if (presenter is not null) yield return presenter.RegainConsciousness();

                    break;

                case < 10:
                    _deathSavingThrowFailures++;

                    Console.WriteLine($"{definiteName.ToUpperFirst()} fails a death saving throw.");

                    yield return PresentDeathSavingThrows(1, false);

                    break;

                default:
                    _deathSavingThrowSuccesses++;

                    Console.WriteLine($"{definiteName.ToUpperFirst()} succeeds a death saving throw.");

                    yield return PresentDeathSavingThrows(1, true);

                    break;
            }

            yield return HandleDeathSavingThrows();
        }

        protected override IEnumerator TakeDamageAtZeroHP(int remainingDamageAmount, Hit hit)
        {
            // Characters instantly die if the remaining damage they would have taken is greater or equal to their maximum HP.
            if (remainingDamageAmount >= hitPointsMaximum)
            {
                lifeStatus = LifeStatus.Dead;
                Console.WriteLine($"{definiteName.ToUpperFirst()} instantly dies.");

                if (presenter is not null) yield return presenter.Die();

                yield break;
            }

            // Alive characters fall unconscious.
            if (lifeStatus == LifeStatus.Alive)
            {
                lifeStatus = LifeStatus.UnstableUnconscious;
                Console.WriteLine($"{definiteName.ToUpperFirst()} falls unconscious.");

                yield break;
            }

            // The creature is unconscious. See if damage was dealt.
            if (remainingDamageAmount > 0)
            {
                yield return presenter.GetAttacked();

                // When damage was dealt when unconscious, they receive a death saving throw failure (2 on a critical hit).
                if (hit.wasCritical)
                {
                    _deathSavingThrowFailures += 2;

                    Console.WriteLine($"{definiteName.ToUpperFirst()} suffers two death saving throw failures.");

                    yield return PresentDeathSavingThrows(2, false);
                }
                else
                {
                    _deathSavingThrowFailures++;

                    Console.WriteLine($"{definiteName.ToUpperFirst()} suffers a death saving throw failure.");

                    yield return PresentDeathSavingThrows(1, false);
                }

                // Destabilize if necessary.
                if (lifeStatus == LifeStatus.StableUnconscious)
                {
                    lifeStatus = LifeStatus.UnstableUnconscious;
                }
            }

            yield return HandleDeathSavingThrows();
        }

        private IEnumerator HandleDeathSavingThrows()
        {
            // If the character fails 3 death saving throws, they die.
            if (_deathSavingThrowFailures >= 3)
            {
                lifeStatus = LifeStatus.Dead;

                yield return Die();

                yield break;
            }

            // If the character succeeds 3 death saving throws, they stabilize.
            if (_deathSavingThrowSuccesses >= 3)
            {
                lifeStatus = LifeStatus.StableUnconscious;
                Console.WriteLine($"{definiteName.ToUpperFirst()} stabilizes.");

                _deathSavingThrowFailures = 0;
                _deathSavingThrowSuccesses = 0;

                if (presenter is not null) presenter.ResetDeathSavingThrows();
            }
        }

        private IEnumerator PresentDeathSavingThrows(int amount, bool success)
        {
            if (presenter is not null)
            {
                for (int i = 0; i < amount; i++)
                {
                    yield return presenter.PerformDeathSavingThrow(success);
                }
            }
        }
    }
}
