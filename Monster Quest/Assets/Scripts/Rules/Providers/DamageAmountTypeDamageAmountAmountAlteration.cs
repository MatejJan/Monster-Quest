using System.Linq;

namespace MonsterQuest
{
    public class DamageAmountTypeDamageAmountAmountAlteration : IDamageAmountAlterationRule, IRulesProvider
    {
        public DamageAmountAlterationValue GetDamageAlteration(DamageAmount damageAmount)
        {
            DebugHelper.StartLog("Determining vulnerabilities … ");
            DamageType[] vulnerabilities = damageAmount.hit.attackAction.gameState.GetRuleValues((IDamageTypeRule rule) => rule.GetDamageTypeVulnerabilities(damageAmount)).Resolve();
            DebugHelper.EndLog();

            DebugHelper.StartLog("Determining resistances … ");
            DamageType[] resistances = damageAmount.hit.attackAction.gameState.GetRuleValues((IDamageTypeRule rule) => rule.GetDamageTypeResistances(damageAmount)).Resolve();
            DebugHelper.EndLog();

            DebugHelper.StartLog("Determining immunities … ");
            DamageType[] immunities = damageAmount.hit.attackAction.gameState.GetRuleValues((IDamageTypeRule rule) => rule.GetDamageTypeImmunities(damageAmount)).Resolve();
            DebugHelper.EndLog();

            // See which vulnerabilities, resistances, and immunities are included in the damage type.
            bool isVulnerable = vulnerabilities.Any(vulnerability => (vulnerability & damageAmount.type) == vulnerability);
            bool isResistant = resistances.Any(resistance => (resistance & damageAmount.type) == resistance);
            bool isImmune = immunities.Any(immunity => (immunity & damageAmount.type) == immunity);

            return new DamageAmountAlterationValue(this, isVulnerable, isResistant, isImmune);
        }

        public string rulesProviderName => "damage type";
    }
}
