namespace MonsterQuest
{
    public interface IAttackRollModifierRule
    {
        IntegerValue GetAttackRollModifier(AttackAction attackAction);
    }

    public interface IInformativeMonsterAttackAttackRollModifierRule
    {
        IntegerValue GetAttackRollModifier(InformativeMonsterAttackAction attackAction);
    }
}
