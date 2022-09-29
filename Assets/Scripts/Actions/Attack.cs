using System;
using System.Linq;
using MonsterQuest.Effects;

namespace MonsterQuest.Actions
{
    public class Attack : IAction
    {
        public Attack(Battle battle, Creature attacker, Creature target, Effects.Attack effect, Item weapon, Ability? ability = null, bool isBonusAction = false)
        {
            this.battle = battle;
            this.attacker = attacker;
            this.target = target;
            this.effect = effect;
            this.weapon = weapon;
            this.ability = ability;
            this.isBonusAction = isBonusAction;
        }

        public Battle battle { get; }
        public Creature attacker { get; }
        public Creature target { get; private set; }
        public Effects.Attack effect { get; }
        public Item weapon { get; }
        public Ability? ability { get; }
        public bool isBonusAction { get; }

        public void Execute()
        {
            DebugHelper.StartLog($"Performing attack action by {attacker.definiteName} targeting {target.definiteName} … ");

            // Determine if a target redirect occurs.
            DebugHelper.StartLog("Determining target redirect … ");
            Creature newTarget = battle.GetRuleValues((ITargetRedirectionRule rule) => rule.RedirectTarget(this)).Resolve();
            target = newTarget ?? target;
            DebugHelper.EndLog();

            // Determine whether the attack is a hit or a miss.
            bool wasHit = false;
            bool wasCritical = false;

            // Query what attack roll method will be used for this attack (normal, advantage, disadvantage).
            DebugHelper.StartLog("Determining advantage or disadvantage for the attack roll … ");
            AttackRollMethod[] attackRollMethods = battle.GetRuleValues((IAttackRollMethodRule rule) => rule.GetAttackRollMethod(this)).Resolve();
            DebugHelper.EndLog();

            bool advantage = attackRollMethods.Contains(AttackRollMethod.Advantage);
            bool disadvantage = attackRollMethods.Contains(AttackRollMethod.Disadvantage);

            int attackRoll = Dice.Roll("d20");

            if (advantage && !disadvantage)
            {
                // We have an advantage, roll again and take the maximum.
                DebugHelper.StartLog("Rolling again for advantage.");
                attackRoll = Math.Max(attackRoll, Dice.Roll("d20"));
                DebugHelper.EndLog();
            }
            else if (disadvantage && !advantage)
            {
                // We have a disadvantage, roll again and take the minimum.
                DebugHelper.StartLog("Rolling again for disadvantage.");
                attackRoll = Math.Min(attackRoll, Dice.Roll("d20"));
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
                int attackRollModifier = battle.GetRuleValues((IAttackRollModifierRule rule) => rule.GetAttackRollModifier(this)).Resolve();
                DebugHelper.EndLog();

                attackRoll += attackRollModifier;

                // Determine the target's armor class.
                DebugHelper.StartLog("Determining target's armor class … ");
                int armorClass = battle.GetRuleValues((IArmorClassRule rule) => rule.GetArmorClass(this)).Resolve();
                DebugHelper.EndLog();

                // Determine result.
                wasHit = attackRoll >= armorClass;
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
                if (descriptionObject != null)
                {
                    Console.Write($"{descriptionObject} ");
                }

                Console.Write($"at {target.definiteName} ");
            }
            else
            {
                Console.Write($"{target.definiteName} ");

                if (descriptionObject != null)
                {
                    Console.Write($"with {descriptionObject} ");
                }
            }

            Console.Write("and ");

            if (!wasHit)
            {
                Console.WriteLine($"{(wasCritical ? "gets a critical miss" : "misses")}.");

                return;
            }

            Console.WriteLine($"{(wasCritical ? "gets a critical hit!" : "hits.")}");

            // Create the hit.
            Hit hit = new(this, wasCritical);

            // Query what kind of damage rolls need to be performed.
            DebugHelper.StartLog("Determining damage rolls … ");
            DamageRoll[] damageRolls = battle.GetRuleValues((IDamageRollRule rule) => rule.GetDamageRolls(hit)).Resolve();
            DebugHelper.EndLog();

            // Roll for damages.
            Damage[] damages = new Damage[damageRolls.Length];

            for (int i = 0; i < damageRolls.Length; i++)
            {
                DamageRoll damageRoll = damageRolls[i];
                string roll = damageRoll.roll;

                // Roll the damage.
                int amount = Dice.Roll(roll);

                // Roll twice for critical hits.
                if (wasCritical)
                {
                    amount += Dice.Roll(roll);
                }

                // Add damage modifiers to base attacks (not extra).
                if (!damageRoll.isExtraDamage)
                {
                    DebugHelper.StartLog("Determining damage modifiers … ");
                    int damageModifier = battle.GetRuleValues((IDamageRollModifierRule rule) => rule.GetDamageRollModifier(this)).Resolve();
                    DebugHelper.EndLog();

                    // The resulting amount cannot be negative.
                    amount = Math.Max(0, amount + damageModifier);
                }

                // Create the damage.
                damages[i] = new Damage(hit, damageRoll, amount);
            }

            DebugHelper.StartLog($"Dealing damages: {string.Join<Damage>(", ", damages)}.");
            DebugHelper.EndLog();

            // Apply the damages.
            foreach (Damage damage in damages)
            {
                battle.QueryRules((IDamageRule rule) => rule.ReactToDamage(damage));
            }
        }
    }
}
