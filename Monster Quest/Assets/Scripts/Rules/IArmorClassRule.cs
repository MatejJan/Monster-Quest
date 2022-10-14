using MonsterQuest.Actions;

namespace MonsterQuest
{
    public interface IArmorClassRule
    {
        IntegerValue GetArmorClass(Creature creature);

        IntegerValue GetArmorClass(Attack attack)
        {
            return GetArmorClass(attack.target);
        }
    }
}
