using System.Collections;

namespace MonsterQuest
{
    public abstract partial class Creature : IDamageRule
    {
        public IEnumerator ReactToDamage(Damage damage)
        {
            // Only react to damage if we are the target.
            if (damage.hit.target != this) yield break;

            // Don't react to further damage once we're dead.
            if (lifeStatus == LifeStatus.Dead) yield break;

            foreach (DamageAmount damageAmount in damage.amounts)
            {
                DamageAmount modifiedDamageAmount = damageAmount;

                // Some damage rolls can be halved with a saving throw.
                if (damageAmount.roll.savingThrowAbility != Ability.None)
                {
                    DebugHelpers.StartLog($"Performing a DC {damageAmount.roll.savingThrowDC} {damageAmount.roll.savingThrowAbility} saving throw …");
                    bool savingThrowSucceeded = MakeSavingThrow(damageAmount.roll.savingThrowAbility, damageAmount.roll.savingThrowDC);
                    DebugHelpers.EndLog();

                    if (savingThrowSucceeded)
                    {
                        modifiedDamageAmount = modifiedDamageAmount.CloneWithValue(damageAmount.value / 2);

                        Console.WriteLine($"{definiteName.ToUpperFirst()} is hit with {damageAmount} and succeeds on a saving throw to halve the damage.");
                    }
                    else
                    {
                        Console.WriteLine($"{definiteName.ToUpperFirst()} is hit with {damageAmount} and fails on a saving throw to resist it.");
                    }
                }

                DebugHelpers.StartLog($"Determining damage amount alteration for {modifiedDamageAmount} on {definiteName} … ");
                DamageAlteration damageAlteration = damage.hit.attack.gameState.GetRuleValues((IDamageAmountAlterationRule rule) => rule.GetDamageAlteration(modifiedDamageAmount)).Resolve();
                DebugHelpers.EndLog();

                DamageAmount finalDamageAmount = modifiedDamageAmount;

                // Apply damage immunities (ignore the damage).
                if (damageAlteration.immunity)
                {
                    finalDamageAmount = finalDamageAmount.CloneWithValue(0);
                }

                // Apply damage resistances (halve the damage).
                if (damageAlteration.resistance)
                {
                    finalDamageAmount = finalDamageAmount.CloneWithValue(finalDamageAmount.value / 2);
                }

                // Apply damage vulnerabilities (double the damage).
                if (damageAlteration.vulnerability)
                {
                    finalDamageAmount = finalDamageAmount.CloneWithValue(finalDamageAmount.value * 2);
                }

                if (damageAmount.roll.savingThrowAbility != Ability.None)
                {
                    Console.Write($"{definiteName.ToUpperFirst()} receives {modifiedDamageAmount}");
                }
                else
                {
                    Console.Write($"{definiteName.ToUpperFirst()} is hit with {modifiedDamageAmount}");
                }

                if (damageAlteration.immunity)
                {
                    Console.WriteLine(" but is immune and takes no damage.");
                }
                else if (damageAlteration.resistance && damageAlteration.vulnerability)
                {
                    Console.WriteLine($". {definiteName.ToUpperFirst()} is both resistant and vulnerable to it and takes {finalDamageAmount.value} damage.");
                }
                else if (damageAlteration.resistance)
                {
                    Console.WriteLine($" but due to their resistance only takes {finalDamageAmount.value} damage.");
                }
                else if (damageAlteration.vulnerability)
                {
                    Console.WriteLine($" but due to their vulnerability takes {finalDamageAmount.value} damage.");
                }
                else
                {
                    Console.WriteLine(".");
                }

                hitPoints -= finalDamageAmount.value;
            }

            // Determine the outcome.
            int remainingAmount = 0;

            if (hitPoints < 0)
            {
                remainingAmount = -hitPoints;
                hitPoints = 0;
            }

            if (hitPoints == 0)
            {
                yield return TakeDamageAtZeroHP(remainingAmount, damage.hit);
            }
            else
            {
                Console.WriteLine($"{definiteName.ToUpperFirst()} has {hitPoints} HP left.");

                yield return presenter.TakeDamage();
            }
        }
    }
}
