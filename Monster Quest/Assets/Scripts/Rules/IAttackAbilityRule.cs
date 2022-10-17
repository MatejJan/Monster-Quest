namespace MonsterQuest
{
    public interface IAttackAbilityRule
    {
        SingleValue<Ability> GetAttackAbility(AttackAction attackAction);
    }
}
