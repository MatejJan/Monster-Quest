using System;
using System.Collections;
using System.Linq;
using MonsterQuest.Effects;

namespace MonsterQuest
{
    public class AttackAction : IAction
    {
        public AttackAction(GameState gameState, Creature attacker, Creature target, Attack effect, Item weapon, Ability? ability = null, bool isBonusAction = false)
        {
            this.gameState = gameState;
            this.attacker = attacker;
            this.target = target;
            this.effect = effect;
            this.weapon = weapon;
            this.ability = ability;
            this.isBonusAction = isBonusAction;
        }

        public GameState gameState { get; }
        public Creature attacker { get; }
        public Creature target { get; private set; }
        public Attack effect { get; }
        public Item weapon { get; }
        public Ability? ability { get; }
        public bool isBonusAction { get; }

        public IEnumerator Execute()
        {
            DebugHelper.StartLog($"Performing attack action by {attacker.definiteName} targeting {target.definiteName} … ");

            // Determine if a target redirect occurs.
            DebugHelper.StartLog("Determining target redirect … ");
            Creature newTarget = gameState.GetRuleValues((ITargetRedirectionRule rule) => rule.RedirectTarget(this)).Resolve();
            target = newTarget ?? target;
            DebugHelper.EndLog();

            // Determine whether the attack is a hit or a miss.
            bool wasHit = false;
            bool wasCritical = false;

            // Attacks on unconscious targets is always a critical hit.
            if (target.isUnconscious)
            {
                wasHit = true;
                wasCritical = true;
            }
            else
            {
                // Query what attack roll method will be used for this attack (normal, advantage, disadvantage).
                DebugHelper.StartLog("Determining advantage or disadvantage for the attack roll … ");
                AttackRollMethod[] attackRollMethods = gameState.GetRuleValues((IAttackRollMethodRule rule) => rule.GetAttackRollMethod(this)).Resolve();
                DebugHelper.EndLog();

                bool advantage = attackRollMethods.Contains(AttackRollMethod.Advantage);
                bool disadvantage = attackRollMethods.Contains(AttackRollMethod.Disadvantage);

                int attackRoll = DiceHelper.Roll("d20");

                if (advantage && !disadvantage)
                {
                    // We have an advantage, roll again and take the maximum.
                    DebugHelper.StartLog("Rolling again for advantage.");
                    attackRoll = Math.Max(attackRoll, DiceHelper.Roll("d20"));
                    DebugHelper.EndLog();
                }
                else if (disadvantage && !advantage)
                {
                    // We have a disadvantage, roll again and take the minimum.
                    DebugHelper.StartLog("Rolling again for disadvantage.");
                    attackRoll = Math.Min(attackRoll, DiceHelper.Roll("d20"));
                    DebugHelper.EndLog();
                }

                // The attack always misses on a critical miss.
                if (attackRoll == 1)
                {
                    wasCritical = true;
                }
                // The attack always hits on a critical hit.
                else if (attackRoll == 20)
                {
                    wasHit = true;
                    wasCritical = true;
                }
                // Otherwise the attack value must be greater than or equal to the target's armor class.
                else
                {
                    // Add attack roll modifiers.
                    DebugHelper.StartLog("Determining attack roll modifiers … ");
                    int attackRollModifier = gameState.GetRuleValues((IAttackRollModifierRule rule) => rule.GetAttackRollModifier(this)).Resolve();
                    DebugHelper.EndLog();

                    attackRoll += attackRollModifier;

                    // Determine the target's armor class.
                    DebugHelper.StartLog("Determining target's armor class … ");
                    int armorClass = gameState.GetRuleValues((IArmorClassRule rule) => rule.GetArmorClass(this)).Resolve();
                    DebugHelper.EndLog();

                    // Determine result.
                    wasHit = attackRoll >= armorClass;
                }
            }

            DebugHelper.EndLog();

            // Describe the outcome of the attack.
            string descriptionVerb;

            if (!string.IsNullOrEmpty(effect.attackType.descriptionVerb))
            {
                descriptionVerb = effect.attackType.descriptionVerb;
            }
            else
            {
                descriptionVerb = effect is RangedAttack ? "shoots" : "attacks";
            }

            Console.Write($"{attacker.definiteName.ToUpperFirst()} {descriptionVerb} ");

            string descriptionObject = string.IsNullOrEmpty(effect.attackType.descriptionObject) ? weapon?.indefiniteName : effect.attackType.descriptionObject;

            if (effect is RangedAttack)
            {
                if (descriptionObject is not null)
                {
                    Console.Write($"{descriptionObject} ");
                }

                Console.Write($"at {target.definiteName} ");
            }
            else
            {
                Console.Write($"{target.definiteName} ");

                if (descriptionObject is not null)
                {
                    Console.Write($"with {descriptionObject} ");
                }
            }

            Console.WriteLine($"and {(wasHit ? $"{(wasCritical ? "gets a critical hit!" : "hits.")}" : $"{(wasCritical ? "gets a critical miss" : "misses")}.")}");

            if (attacker.presenter is not null) yield return attacker.presenter.Attack();

            // End the attack if it was a miss.
            if (!wasHit)
            {
                yield break;
            }

            // Create the hit.
            Hit hit = new(this, wasCritical);

            // Query what kind of damage rolls need to be performed.
            DebugHelper.StartLog("Determining damage rolls … ");
            DamageRoll[] damageRolls = gameState.GetRuleValues((IDamageRollRule rule) => rule.GetDamageRolls(hit)).Resolve();
            DebugHelper.EndLog();

            // Roll for damages.
            DamageAmount[] damageAmounts = new DamageAmount[damageRolls.Length];

            for (int i = 0; i < damageRolls.Length; i++)
            {
                DamageRoll damageRoll = damageRolls[i];
                string roll = damageRoll.roll;

                // Roll the damage.
                int amount = DiceHelper.Roll(roll);

                // Roll twice for critical hits.
                if (wasCritical)
                {
                    amount += DiceHelper.Roll(roll);
                }

                // Add damage modifiers to base attacks (not extra).
                if (!damageRoll.isExtraDamage)
                {
                    DebugHelper.StartLog("Determining damage modifiers … ");
                    int damageModifier = gameState.GetRuleValues((IDamageRollModifierRule rule) => rule.GetDamageRollModifier(this)).Resolve();
                    DebugHelper.EndLog();

                    // The resulting amount cannot be negative.
                    amount = Math.Max(0, amount + damageModifier);
                }

                // Create the damage.
                damageAmounts[i] = new DamageAmount(hit, damageRoll, amount);
            }

            Damage damage = new(damageAmounts);

            DebugHelper.StartLog($"Dealing damage: {damage}.");
            DebugHelper.EndLog();

            // Apply the damage.
            yield return gameState.CallRules((IDamageRule rule) => rule.ReactToDamage(damage));
        }
    }
}
