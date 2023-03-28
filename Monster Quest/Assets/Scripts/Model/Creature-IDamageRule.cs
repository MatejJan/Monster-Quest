using System.Collections;

namespace MonsterQuest
{
    public abstract partial class Creature : IReactToDamageRule
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
                    DebugHelper.StartLog($"Performing a DC {damageAmount.roll.savingThrowDC} {damageAmount.roll.savingThrowAbility} saving throw …");
                    bool savingThrowSucceeded = MakeSavingThrow(damageAmount.roll.savingThrowAbility, damageAmount.roll.savingThrowDC);
                    DebugHelper.EndLog();

                    if (savingThrowSucceeded)
                    {
                        modifiedDamageAmount = modifiedDamageAmount.CloneWithValue(damageAmount.value / 2);

                        ReportStateEvent($"{definiteName.ToUpperFirst()} is hit with {damageAmount} and succeeds on a saving throw to halve the damage.");
                    }
                    else
                    {
                        ReportStateEvent($"{definiteName.ToUpperFirst()} is hit with {damageAmount} and fails on a saving throw to resist it.");
                    }
                }

                DebugHelper.StartLog($"Determining damage amount alteration for {modifiedDamageAmount} on {definiteName} … ");
                DamageAlteration damageAlteration = damage.hit.attackAction.gameState.GetRuleValues((IDamageAmountAlterationRule rule) => rule.GetDamageAlteration(modifiedDamageAmount)).Resolve();
                DebugHelper.EndLog();

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

                string damageMessage = "";

                if (damageAmount.roll.savingThrowAbility != Ability.None)
                {
                    damageMessage += $"{definiteName.ToUpperFirst()} receives {modifiedDamageAmount}";
                }
                else
                {
                    damageMessage += $"{definiteName.ToUpperFirst()} is hit with {modifiedDamageAmount}";
                }

                if (damageAlteration.immunity)
                {
                    damageMessage += " but is immune and takes no damage.";
                }
                else if (damageAlteration.resistance && damageAlteration.vulnerability)
                {
                    damageMessage += $". {definiteName.ToUpperFirst()} is both resistant and vulnerable to it and takes {finalDamageAmount.value} damage.";
                }
                else if (damageAlteration.resistance)
                {
                    damageMessage += $" but due to their resistance only takes {finalDamageAmount.value} damage.";
                }
                else if (damageAlteration.vulnerability)
                {
                    damageMessage += $" but due to their vulnerability takes {finalDamageAmount.value} damage.";
                }
                else
                {
                    damageMessage += ".";
                }

                ReportStateEvent(damageMessage);

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
                yield return TakeDamageAtZeroHitPoints(remainingAmount, damage.hit);
            }
            else
            {
                ReportStateEvent($"{definiteName.ToUpperFirst()} has {hitPoints} HP left.");

                if (presenter is not null) yield return presenter.GetAttacked();
            }
        }
    }
}
