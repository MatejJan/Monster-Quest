namespace MonsterQuest
{
    public interface IDamageRollModifierRule
    {
        IntegerValue GetDamageRollModifier(AttackAction attackAction);
    }
}
