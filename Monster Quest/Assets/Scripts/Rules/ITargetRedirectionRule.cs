using MonsterQuest.Actions;

namespace MonsterQuest
{
    public interface ITargetRedirectionRule
    {
        public SingleValue<Creature> RedirectTarget(Attack attack);
    }
}
