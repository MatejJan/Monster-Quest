using MonsterQuest.Actions;

namespace MonsterQuest
{
    public class AttackAbilityModifier : IAttackRollModifierRule, IDamageRollModifierRule, IRulesProvider
    {
        public IntegerValue GetAttackRollModifier(Attack attack)
        {
            return GetAttackModifier(attack);
        }

        public IntegerValue GetDamageRollModifier(Attack attack)
        {
            return GetAttackModifier(attack);
        }

        public string rulesProviderName => "attack ability modifier";

        private IntegerValue GetAttackModifier(Attack attack)
        {
            // Bonus action attacks don't receive the ability modifier.
            if (attack.isBonusAction) return null;

            // The priority is the ability chosen by the attacker (for finesse weapons).
            if (attack.ability.HasValue)
            {
                return new IntegerValue(this, modifierValue: attack.attacker.abilityScores[attack.ability.Value].modifier);
            }

            // Find which ability was chosen for the modifier.
            DebugHelpers.StartLog("Determining attack ability … ");
            Ability attackAbility = Game.GetRuleValues((IAttackAbilityRule rule) => rule.GetAttackAbility(attack)).Resolve();
            DebugHelpers.EndLog();

            if (attackAbility == Ability.None) return null;

            return new IntegerValue(this, modifierValue: attack.attacker.abilityScores[attackAbility].modifier);
        }
    }
}
