using MonsterQuest.Effects;

namespace MonsterQuest
{
    public interface IArmorProficiencyRule
    {
        ArrayValue<ArmorCategory> GetArmorProficiency(Creature creature);
    }
}
