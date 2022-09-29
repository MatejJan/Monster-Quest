using System.Linq;

namespace MonsterQuest
{
    public class DamageTypeDamageAlteration : IDamageAlterationRule, IRulesProvider
    {
        public DamageAlterationValue GetDamageAlteration(Damage damage)
        {
            Hit hit = damage.hit;
            Battle battle = hit.attack.battle;

            DebugHelper.StartLog("Determining vulnerabilities … ");
            DamageType[] vulnerabilities = battle.GetRuleValues((IDamageTypeRule rule) => rule.GetDamageTypeVulnerabilities(damage)).Resolve();
            DebugHelper.EndLog();

            DebugHelper.StartLog("Determining resistances … ");
            DamageType[] resistances = battle.GetRuleValues((IDamageTypeRule rule) => rule.GetDamageTypeResistances(damage)).Resolve();
            DebugHelper.EndLog();

            DebugHelper.StartLog("Determining immunities … ");
            DamageType[] immunities = battle.GetRuleValues((IDamageTypeRule rule) => rule.GetDamageTypeImmunities(damage)).Resolve();
            DebugHelper.EndLog();

            // See which vulnerabilities, resistances, and immunities are included in the damage type.
            bool isVulnerable = vulnerabilities.Any(vulnerability => (vulnerability & damage.type) == vulnerability);
            bool isResistant = resistances.Any(resistance => (resistance & damage.type) == resistance);
            bool isImmune = immunities.Any(immunity => (immunity & damage.type) == immunity);

            return new DamageAlterationValue(this, isVulnerable, isResistant, isImmune);
        }

        public string rulesProviderName => "damage type";
    }
}
