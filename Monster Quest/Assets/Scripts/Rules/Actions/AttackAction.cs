using System;
using System.Linq;
using MonsterQuest.Effects;
using MonsterQuest.Events;

namespace MonsterQuest
{
    public class AttackAction : IAction, IStateEventProvider
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

        // Methods

        public void Execute()
        {
            AttackEvent attackEvent = new()
            {
                attackAction = this
            };

            DebugHelper.StartLog($"Performing attack action by {attacker.definiteName} targeting {target.definiteName} … ");

            // Determine if a target redirect occurs.
            DebugHelper.StartLog("Determining target redirect … ");
            attackEvent.targetRedirectionValues = gameState.GetRuleValues((ITargetRedirectionRule rule) => rule.RedirectTarget(this));
            Creature newTarget = attackEvent.targetRedirectionValues.Resolve();

            if (newTarget is not null)
            {
                attackEvent.redirectedFromTarget = target;
                target = newTarget;
            }

            DebugHelper.EndLog();

            // Determine whether the attack is a hit or a miss.
            // Attacks on unconscious targets within 5 feet is always a critical hit.
            if (target.isUnconscious && gameState.combat.GetDistance(attacker, target) <= 5)
            {
                attackEvent.wasHit = true;
                attackEvent.wasCritical = true;
            }
            else
            {
                // Query what attack roll method will be used for this attack (normal, advantage, disadvantage).
                DebugHelper.StartLog("Determining advantage or disadvantage for the attack roll … ");
                attackEvent.attackRollMethodValues = gameState.GetRuleValues((IAttackRollMethodRule rule) => rule.GetAttackRollMethod(this));
                attackEvent.attackRollMethods = attackEvent.attackRollMethodValues.Resolve();
                DebugHelper.EndLog();

                attackEvent.hadAdvantage = attackEvent.attackRollMethods.Contains(AttackRollMethod.Advantage);
                attackEvent.hadDisadvantage = attackEvent.attackRollMethods.Contains(AttackRollMethod.Disadvantage);

                attackEvent.firstAttackRollResult = new RollResult("d20");
                attackEvent.attackRollTotal = attackEvent.firstAttackRollResult.result;

                if (attackEvent.hadAdvantage && !attackEvent.hadDisadvantage)
                {
                    // We have an advantage, roll again and take the maximum.
                    DebugHelper.StartLog("Rolling again for advantage.");
                    attackEvent.secondAttackRollResult = new RollResult("d20");
                    attackEvent.attackRollTotal = Math.Max(attackEvent.attackRollTotal, attackEvent.secondAttackRollResult.result);
                    DebugHelper.EndLog();
                }
                else if (attackEvent.hadDisadvantage && !attackEvent.hadAdvantage)
                {
                    // We have a disadvantage, roll again and take the minimum.
                    DebugHelper.StartLog("Rolling again for disadvantage.");
                    attackEvent.secondAttackRollResult = new RollResult("d20");
                    attackEvent.attackRollTotal = Math.Min(attackEvent.attackRollTotal, attackEvent.secondAttackRollResult.result);
                    DebugHelper.EndLog();
                }

                // The attack always misses on a critical miss.
                if (attackEvent.attackRollTotal == 1)
                {
                    attackEvent.wasCritical = true;
                }
                // The attack always hits on a critical hit.
                else if (attackEvent.attackRollTotal == 20)
                {
                    attackEvent.wasHit = true;
                    attackEvent.wasCritical = true;
                }
                // Otherwise the attack value must be greater than or equal to the target's armor class.
                else
                {
                    // Add attack roll modifiers.
                    DebugHelper.StartLog("Determining attack roll modifiers … ");
                    attackEvent.attackRollModifierValues = gameState.GetRuleValues((IAttackRollModifierRule rule) => rule.GetAttackRollModifier(this));
                    attackEvent.attackRollModifier = attackEvent.attackRollModifierValues.Resolve();
                    DebugHelper.EndLog();

                    attackEvent.attackRollTotal += attackEvent.attackRollModifier;

                    // Determine the target's armor class.
                    DebugHelper.StartLog("Determining target's armor class … ");
                    attackEvent.targetArmorClassValues = gameState.GetRuleValues((IArmorClassRule rule) => rule.GetArmorClass(this));
                    attackEvent.targetArmorClass = attackEvent.targetArmorClassValues.Resolve();
                    DebugHelper.EndLog();

                    // Determine result.
                    attackEvent.wasHit = attackEvent.attackRollTotal >= attackEvent.targetArmorClass;
                }
            }

            DebugHelper.EndLog();

            // End the attack if it was a miss.
            if (!attackEvent.wasHit)
            {
                ReportStateEvent(attackEvent);

                return;
            }

            // Create the hit.
            Hit hit = new(this, attackEvent.wasCritical);

            // Query what kind of damage rolls need to be performed.
            DebugHelper.StartLog("Determining damage rolls … ");
            attackEvent.damageRollValues = gameState.GetRuleValues((IDamageRollRule rule) => rule.GetDamageRolls(hit));
            DamageRoll[] damageRolls = attackEvent.damageRollValues.Resolve();
            attackEvent.damageRollResults = new AttackEvent.DamageRollResult[damageRolls.Length];
            DebugHelper.EndLog();

            // Roll for damages.
            DamageAmount[] damageAmounts = new DamageAmount[damageRolls.Length];

            for (int i = 0; i < damageRolls.Length; i++)
            {
                DamageRoll damageRoll = damageRolls[i];

                AttackEvent.DamageRollResult damageRollResult = new()
                {
                    damageRoll = damageRoll
                };

                attackEvent.damageRollResults[i] = damageRollResult;

                // Roll the damage.
                damageRollResult.firstDamageRollResult = new RollResult(damageRoll.roll);
                damageRollResult.amount = damageRollResult.firstDamageRollResult.result;

                // Roll twice for critical hits.
                if (attackEvent.wasCritical)
                {
                    damageRollResult.secondDamageRollResult = new RollResult(damageRoll.roll);
                    damageRollResult.amount += damageRollResult.secondDamageRollResult.result;
                }

                // Add damage modifiers to base attacks (not extra).
                if (!damageRoll.isExtraDamage)
                {
                    DebugHelper.StartLog("Determining damage modifiers … ");
                    damageRollResult.damageRollModifierValues = gameState.GetRuleValues((IDamageRollModifierRule rule) => rule.GetDamageRollModifier(this));
                    damageRollResult.damageRollModifier = damageRollResult.damageRollModifierValues.Resolve();
                    DebugHelper.EndLog();

                    // The resulting amount cannot be negative.
                    damageRollResult.amount = Math.Max(0, damageRollResult.amount + damageRollResult.damageRollModifier.Value);
                }

                // Create the damage.
                damageAmounts[i] = new DamageAmount(hit, damageRoll, damageRollResult.amount);
            }

            Damage damage = new(damageAmounts);

            ReportStateEvent(attackEvent);

            DebugHelper.StartLog($"Dealing damage: {damage}.");
            DebugHelper.EndLog();

            // Apply the damage.
            gameState.CallRules((IReactToDamageRule rule) => rule.ReactToDamage(damage));

            // Inform that damage was dealt.
            gameState.CallRules((IReactToDamageRule rule) => rule.ReactToDamageDealt(damage));
        }

        // Events 

        public event Action<object> stateEvent;

        private void ReportStateEvent(object eventData)
        {
            stateEvent?.Invoke(eventData);
        }
    }
}
