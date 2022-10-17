namespace MonsterQuest
{
    public class CoverBonus : IArmorClassRule, IRulesProvider
    {
        public IntegerValue GetArmorClass(Creature creature)
        {
            return null;
        }

        public IntegerValue GetArmorClass(AttackAction attackAction)
        {
            // See what cover the target has.
            DebugHelper.StartLog("Determining cover … ");
            Cover cover = attackAction.gameState.GetRuleValues((ICoverRule rule) => rule.GetCover(attackAction)).Resolve();
            DebugHelper.EndLog();

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
