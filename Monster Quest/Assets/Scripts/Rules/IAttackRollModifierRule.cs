namespace MonsterQuest
{
    public interface IAttackRollModifierRule
    {
        IntegerValue GetAttackRollModifier(AttackAction attackAction);
    }
}
