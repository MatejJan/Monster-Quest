using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Class", menuName = "Effects/Class")]
    public class ClassType : EffectType
    {
        public string displayName;
        public string hitDice;
        public int hitPointsBase;

        public WeaponCategory[] weaponProficiencies;
        public ArmorCategory[] armorProficiencies;
        public Ability[] savingThrowProficiencies;

        public override Effect Create(object parent)
        {
            return new Class(this, parent);
        }
    }

    public class Class : Effect, IWeaponProficiencyRule, IArmorProficiencyRule
    {
        public Class(ClassType type, object parent) : base(type, parent) { }
        public ClassType classType => (ClassType)type;

        public ArrayValue<ArmorCategory> GetArmorProficiency(Creature creature)
        {
            // Only provide information for the current creature.
            if (creature != parent) return null;

            return new ArrayValue<ArmorCategory>(this, classType.armorProficiencies);
        }

        public ArrayValue<WeaponCategory> GetWeaponProficiency(Creature creature)
        {
            // Only provide information for the current creature.
            if (creature != parent) return null;

            return new ArrayValue<WeaponCategory>(this, classType.weaponProficiencies);
        }
    }
}
