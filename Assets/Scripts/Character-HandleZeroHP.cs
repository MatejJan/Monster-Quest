using UnityEngine;

namespace MonsterQuest
{
    public partial class Character
    {
        [SerializeField] private int _deathSavingThrowFailures;
        [SerializeField] private int _deathSavingThrowSuccesses;

        public void HandleUnconsciousState()
        {
            HandleZeroHP(0, null);
        }

        protected override void HandleZeroHP(int remainingDamageAmount, Hit hit)
        {
            // Characters instantly die if the remaining damage they would have taken is greater or equal to their maximum HP.
            if (remainingDamageAmount >= hitPointsMaximum)
            {
                lifeStatus = LifeStatus.Dead;
                Console.WriteLine($"{definiteName.ToUpperFirst()} instantly dies.");

                return;
            }

            // Alive characters fall unconscious.
            if (lifeStatus == LifeStatus.Alive)
            {
                lifeStatus = LifeStatus.UnstableUnconscious;
                Console.WriteLine($"{definiteName.ToUpperFirst()} falls unconscious.");

                return;
            }

            // Unstable unconscious characters must make a death saving throw.
            if (lifeStatus == LifeStatus.UnstableUnconscious)
            {
                if (remainingDamageAmount > 0)
                {
                    // When damage was dealt at 0 HP, they receive a death saving throw failure (2 on a critical hit).
                    if (hit.wasCritical)
                    {
                        _deathSavingThrowFailures += 2;

                        Console.WriteLine($"{definiteName.ToUpperFirst()} suffers two death saving throw failures.");
                    }
                    else
                    {
                        _deathSavingThrowFailures++;

                        Console.WriteLine($"{definiteName.ToUpperFirst()} suffers a death saving throw failure.");
                    }
                }
                else
                {
                    int deathSavingThrow = Dice.Roll("d20");

                    if (deathSavingThrow == 1)
                    {
                        _deathSavingThrowFailures += 2;

                        Console.WriteLine($"{definiteName.ToUpperFirst()} critically fails a death saving throw.");
                    }
                    else if (deathSavingThrow < 10)
                    {
                        _deathSavingThrowFailures++;

                        Console.WriteLine($"{definiteName.ToUpperFirst()} fails a death saving throw.");
                    }
                    else if (deathSavingThrow == 20)
                    {
                        Console.WriteLine($"{definiteName.ToUpperFirst()} critically succeeds a death saving throw.");

                        // Gain 1 HP.
                        Heal(1);
                    }
                    else
                    {
                        _deathSavingThrowSuccesses++;

                        Console.WriteLine($"{definiteName.ToUpperFirst()} succeeds a death saving throw.");
                    }
                }

                // If the character fails 3 death saving throws, they die.
                if (_deathSavingThrowFailures >= 3)
                {
                    lifeStatus = LifeStatus.Dead;
                    Console.WriteLine($"{definiteName.ToUpperFirst()} dies.");
                }

                // If the character succeeds 3 death saving throws, they stabilize.
                if (_deathSavingThrowSuccesses >= 3)
                {
                    lifeStatus = LifeStatus.StableUnconscious;
                    Console.WriteLine($"{definiteName.ToUpperFirst()} stabilizes.");
                }
            }
        }
    }
}
