namespace MonsterQuest
{
    public interface ITargetRedirectionRule
    {
        public SingleValue<Creature> RedirectTarget(AttackAction attackAction);
    }
}
