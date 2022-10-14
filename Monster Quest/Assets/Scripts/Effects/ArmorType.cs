using System;
using System.Linq;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Armor", menuName = "Effects/Armor")]
    public class ArmorType : EffectType
    {
        public int armorClass;
        public int minimumStrength;
        public ArmorCategory category;

        public override Effect Create(object parent)
        {
            return new Armor(this, parent);
        }
    }

    [Serializable]
    public class Armor : Effect, IArmorClassRule
    {
        public Armor(EffectType type, object parent) : base(type, parent) { }
        public ArmorType armorType => (ArmorType)type;

        public IntegerValue GetArmorClass(Creature creature)
        {
            // Only provide information for the creature wearing this armor.
            if (!creature.items.Contains(parent as Item)) return null;

            // Shields provide a bonus to armor.
            if (armorType.category == ArmorCategory.Shield)
            {
                return new IntegerValue(this, modifierValue: armorType.armorClass);
            }

            int? dexterityModifier = armorType.category switch
            {
                // Light armor has the full dexterity modifier applied.
                ArmorCategory.Light => creature.abilityScores.dexterity.modifier,

                // Medium armor has the dexterity modifier applied up to a maximum of 2.
                ArmorCategory.Medium => Math.Min(creature.abilityScores.dexterity.modifier, 2),

                _ => null
            };

            return new IntegerValue(this, armorType.armorClass, dexterityModifier);
        }
    }

    public enum ArmorCategory
    {
        None,
        Light,
        Medium,
        Heavy,
        Shield
    }
}
