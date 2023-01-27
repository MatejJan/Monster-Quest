namespace MonsterQuest
{
    public interface IDamageRollModifierRule
    {
        IntegerValue GetDamageRollModifier(AttackAction attackAction);
    }

    public interface IInformativeMonsterAttackDamageRollModifierRule
    {
        IntegerValue GetDamageRollModifier(InformativeMonsterAttackAction attackAction);
    }
}
