using MonsterQuest.Actions;

namespace MonsterQuest
{
    public interface IAttackRollMethodRule
    {
        MultipleValue<AttackRollMethod> GetAttackRollMethod(Attack attack);
    }
}
