namespace MonsterQuest
{
    public class AttackAbilityModifier : IAttackRollModifierRule, IDamageRollModifierRule, IRulesProvider
    {
        public IntegerValue GetAttackRollModifier(AttackAction attackAction)
        {
            return GetAttackModifier(attackAction);
        }

        public IntegerValue GetDamageRollModifier(AttackAction attackAction)
        {
            return GetAttackModifier(attackAction);
        }

        public string rulesProviderName => "attack ability modifier";

        private IntegerValue GetAttackModifier(AttackAction attackAction)
        {
            // Bonus action attacks don't receive the ability modifier.
            if (attackAction.isBonusAction) return null;

            // The priority is the ability chosen by the attacker (for finesse weapons).
            if (attackAction.ability.HasValue)
            {
                return new IntegerValue(this, modifierValue: attackAction.attacker.abilityScores[attackAction.ability.Value].modifier);
            }

            // Find which ability was chosen for the modifier.
            DebugHelper.StartLog("Determining attack ability … ");
            Ability attackAbility = attackAction.gameState.GetRuleValues((IAttackAbilityRule rule) => rule.GetAttackAbility(attackAction)).Resolve();
            DebugHelper.EndLog();

            if (attackAbility == Ability.None) return null;

            return new IntegerValue(this, modifierValue: attackAction.attacker.abilityScores[attackAbility].modifier);
        }
    }
}
