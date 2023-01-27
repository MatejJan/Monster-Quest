namespace MonsterQuest
{
    public interface IRuleDescriptionsProvider
    {
        public ArrayValue<RuleDescription> GetRuleDescriptions(object context = null);
    }
}
