using MonsterQuest.Effects;

namespace MonsterQuest
{
    public interface IWeaponProficiencyRule
    {
        ArrayValue<WeaponCategory> GetWeaponProficiency(Creature creature);
    }
}
