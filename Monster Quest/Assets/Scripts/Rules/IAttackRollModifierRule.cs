using MonsterQuest.Actions;

namespace MonsterQuest
{
    public interface IAttackRollModifierRule
    {
        IntegerValue GetAttackRollModifier(Attack attack);
    }
}
