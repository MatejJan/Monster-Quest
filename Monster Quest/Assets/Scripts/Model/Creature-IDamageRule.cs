using MonsterQuest.Events;

namespace MonsterQuest
{
    public abstract partial class Creature : IReactToDamageRule
    {
        public void ReactToDamage(Damage damage)
        {
            // Only react to damage if we are the target.
            if (damage.hit.target != this) return;

            // Don't react to further damage once we're dead.
            if (lifeStatus == LifeStatus.Dead) return;

            DamageEvent damageEvent = new()
            {
                damage = damage,
                creature = this,
                damageAmountResults = new DamageEvent.DamageAmountResult[damage.amounts.Length],
                hitPointsStart = hitPoints,
                hitPointsMaximum = hitPointsMaximum,
                attacker = damage.hit.attackAction.attacker
            };

            for (int i = 0; i < damage.amounts.Length; i++)
            {
                DamageAmount damageAmount = damage.amounts[i];

                DamageEvent.DamageAmountResult damageAmountResult = new()
                {
                    damageAmount = damageAmount,
                    modifiedDamageAmount = damageAmount
                };

                damageEvent.damageAmountResults[i] = damageAmountResult;

                // Some damage rolls can be halved with a saving throw.
                if (damageAmount.roll.savingThrowAbility != Ability.None)
                {
                    DebugHelper.StartLog($"Performing a DC {damageAmount.roll.savingThrowDC} {damageAmount.roll.savingThrowAbility} saving throw …");
                    damageAmountResult.savingThrowResult = new SavingThrowResult(this, damageAmount.roll.savingThrowAbility, damageAmount.roll.savingThrowDC);
                    DebugHelper.EndLog();

                    if (damageAmountResult.savingThrowResult.success)
                    {
                        damageAmountResult.modifiedDamageAmount = damageAmountResult.modifiedDamageAmount.CloneWithValue(damageAmount.value / 2);
                    }
                }

                DebugHelper.StartLog($"Determining damage amount alteration for {damageAmountResult.modifiedDamageAmount} on {definiteName} … ");
                damageAmountResult.damageAlteration = damage.hit.attackAction.gameState.GetRuleValues((IDamageAmountAlterationRule rule) => rule.GetDamageAlteration(damageAmountResult.modifiedDamageAmount)).Resolve();
                DebugHelper.EndLog();

                damageAmountResult.finalDamageAmount = damageAmountResult.modifiedDamageAmount;

                // Apply damage immunities (ignore the damage).
                if (damageAmountResult.damageAlteration.immunity)
                {
                    damageAmountResult.finalDamageAmount = damageAmountResult.finalDamageAmount.CloneWithValue(0);
                }

                // Apply damage resistances (halve the damage).
                if (damageAmountResult.damageAlteration.resistance)
                {
                    damageAmountResult.finalDamageAmount = damageAmountResult.finalDamageAmount.CloneWithValue(damageAmountResult.finalDamageAmount.value / 2);
                }

                // Apply damage vulnerabilities (double the damage).
                if (damageAmountResult.damageAlteration.vulnerability)
                {
                    damageAmountResult.finalDamageAmount = damageAmountResult.finalDamageAmount.CloneWithValue(damageAmountResult.finalDamageAmount.value * 2);
                }

                hitPoints -= damageAmountResult.finalDamageAmount.value;
            }

            // Determine the outcome.
            damageEvent.remainingDamageAmount = 0;

            if (hitPoints < 0)
            {
                damageEvent.remainingDamageAmount = -hitPoints;
                hitPoints = 0;
            }

            damageEvent.hitPointsEnd = hitPoints;

            ReportStateEvent(damageEvent);

            if (hitPoints == 0)
            {
                TakeDamageAtZeroHitPoints(damageEvent.remainingDamageAmount, damage.hit);
            }
        }
    }
}
