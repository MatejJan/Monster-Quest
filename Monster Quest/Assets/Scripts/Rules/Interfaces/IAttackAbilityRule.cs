namespace MonsterQuest
{
    public interface IAttackAbilityRule
    {
        SingleValue<Ability> GetAttackAbility(AttackAction attackAction);
    }

    public interface IInformativeMonsterAttackAttackAbilityRule
    {
        SingleValue<Ability> GetAttackAbility(InformativeMonsterAttackAction attackAction);
    }
}
