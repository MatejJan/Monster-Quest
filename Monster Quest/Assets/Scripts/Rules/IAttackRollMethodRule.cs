namespace MonsterQuest
{
    public interface IAttackRollMethodRule
    {
        MultipleValue<AttackRollMethod> GetAttackRollMethod(AttackAction attackAction);
    }
}
