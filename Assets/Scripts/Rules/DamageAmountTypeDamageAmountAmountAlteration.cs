using System.Linq;

namespace MonsterQuest
{
    public class DamageAmountTypeDamageAmountAmountAlteration : IDamageAmountAlterationRule, IRulesProvider
    {
        public DamageAmountAlterationValue GetDamageAlteration(DamageAmount damageAmount)
        {
            Hit hit = damageAmount.hit;
            Battle battle = hit.attack.battle;

            DebugHelpers.StartLog("Determining vulnerabilities … ");
            DamageType[] vulnerabilities = battle.GetRuleValues((IDamageTypeRule rule) => rule.GetDamageTypeVulnerabilities(damageAmount)).Resolve();
            DebugHelpers.EndLog();

            DebugHelpers.StartLog("Determining resistances … ");
            DamageType[] resistances = battle.GetRuleValues((IDamageTypeRule rule) => rule.GetDamageTypeResistances(damageAmount)).Resolve();
            DebugHelpers.EndLog();

            DebugHelpers.StartLog("Determining immunities … ");
            DamageType[] immunities = battle.GetRuleValues((IDamageTypeRule rule) => rule.GetDamageTypeImmunities(damageAmount)).Resolve();
            DebugHelpers.EndLog();

            // See which vulnerabilities, resistances, and immunities are included in the damage type.
            bool isVulnerable = vulnerabilities.Any(vulnerability => (vulnerability & damageAmount.type) == vulnerability);
            bool isResistant = resistances.Any(resistance => (resistance & damageAmount.type) == resistance);
            bool isImmune = immunities.Any(immunity => (immunity & damageAmount.type) == immunity);

            return new DamageAmountAlterationValue(this, isVulnerable, isResistant, isImmune);
        }

        public string rulesProviderName => "damage type";
    }
}