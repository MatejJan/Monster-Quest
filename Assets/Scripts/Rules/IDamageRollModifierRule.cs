using MonsterQuest.Actions;

namespace MonsterQuest
{
    public interface IDamageRollModifierRule
    {
        IntegerValue GetDamageRollModifier(Attack attack);
    }
}
