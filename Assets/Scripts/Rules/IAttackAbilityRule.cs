using MonsterQuest.Actions;

namespace MonsterQuest
{
    public interface IAttackAbilityRule
    {
        SingleValue<Ability> GetAttackAbility(Attack attack);
    }
}
