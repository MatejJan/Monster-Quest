namespace MonsterQuest
{
    public abstract partial class Creature : IDamageRule
    {
        public void ReactToDamage(Damage damage)
        {
            // Only react to damage if we are the target.
            if (damage.hit.target != this) return;

            Damage modifiedDamage = damage;

            // Some damage rolls can be halved with a saving throw.
            if (damage.roll.savingThrowAbility != Ability.None)
            {
                DebugHelper.StartLog($"Performing a DC {damage.roll.savingThrowDC} {damage.roll.savingThrowAbility} saving throw …");
                bool savingThrowSucceeded = MakeSavingThrow(damage.roll.savingThrowAbility, damage.roll.savingThrowDC);
                DebugHelper.EndLog();

                if (savingThrowSucceeded)
                {
                    modifiedDamage = modifiedDamage.CloneWithAmount(damage.amount / 2);

                    Console.WriteLine($"{definiteName.ToUpperFirst()} is hit with {damage} and succeeds on a saving throw to halve the damage.");
                }
                else
                {
                    Console.WriteLine($"{definiteName.ToUpperFirst()} is hit with {damage} and fails on a saving throw to resist it.");
                }
            }

            DebugHelper.StartLog($"Determining damage alteration for {modifiedDamage} on {definiteName} … ");
            DamageAlteration damageAlteration = modifiedDamage.hit.battle.GetRuleValues((IDamageAlterationRule rule) => rule.GetDamageAlteration(modifiedDamage)).Resolve();
            DebugHelper.EndLog();

            Damage finalDamage = modifiedDamage;

            // Apply damage immunities (ignore the damage).
            if (damageAlteration.immunity)
            {
                finalDamage = finalDamage.CloneWithAmount(0);
            }

            // Apply damage resistances (halve the damage).
            if (damageAlteration.resistance)
            {
                finalDamage = finalDamage.CloneWithAmount(finalDamage.amount / 2);
            }

            // Apply damage vulnerabilities (double the damage).
            if (damageAlteration.vulnerability)
            {
                finalDamage = finalDamage.CloneWithAmount(finalDamage.amount * 2);
            }

            if (damage.roll.savingThrowAbility != Ability.None)
            {
                Console.Write($"{definiteName.ToUpperFirst()} receives {modifiedDamage}");
            }
            else
            {
                Console.Write($"{definiteName.ToUpperFirst()} is hit with {modifiedDamage}");
            }

            if (damageAlteration.immunity)
            {
                Console.WriteLine(" but is immune and takes no damage.");
            }
            else if (damageAlteration.resistance && damageAlteration.vulnerability)
            {
                Console.WriteLine($". {definiteName.ToUpperFirst()} is both resistant and vulnerable to it and takes {finalDamage.amount} damage.");
            }
            else if (damageAlteration.resistance)
            {
                Console.WriteLine($" but due to their resistance only takes {finalDamage.amount} damage.");
            }
            else if (damageAlteration.vulnerability)
            {
                Console.WriteLine($" but due to their vulnerability takes {finalDamage.amount} damage.");
            }
            else
            {
                Console.WriteLine(".");
            }

            // Determine the outcome.
            hitPoints -= finalDamage.amount;
            int remainingAmount = 0;

            if (hitPoints < 0)
            {
                remainingAmount = -hitPoints;
                hitPoints = 0;
            }

            if (hitPoints == 0)
            {
                HandleZeroHP(remainingAmount, finalDamage.hit);
            }
            else
            {
                Console.WriteLine($"{definiteName.ToUpperFirst()} has {hitPoints} HP left.");
            }
        }
    }
}
