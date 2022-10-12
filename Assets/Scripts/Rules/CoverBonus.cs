using System.Collections.Generic;
using MonsterQuest.Actions;

namespace MonsterQuest
{
    public class CoverBonus : IArmorClassRule, IRulesProvider
    {
        public IntegerValue GetArmorClass(Creature creature)
        {
            return null;
        }

        public IntegerValue GetArmorClass(Attack attack)
        {
            // See what cover the target has.
            List<CoverValue> values = new();

            DebugHelpers.StartLog("Determining cover … ");
            Cover cover = attack.battle.GetRuleValues((ICoverRule rule) => rule.GetCover(attack)).Resolve();
            DebugHelpers.EndLog();

            // Half and three-quarters cover give you a bonus.
            int armorClassBonus = cover switch
            {
                Cover.Half => 2,
                Cover.ThreeQuarters => 5,
                _ => 0
            };

            return armorClassBonus == 0 ? null : new IntegerValue(this, modifierValue: armorClassBonus);
        }

        public string rulesProviderName => "cover";
    }
}