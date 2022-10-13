using System;
using System.Linq;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Effects/Weapon")]
    public class WeaponType : EffectType
    {
        public WeaponCategory[] categories;

        public bool HasCategory(WeaponCategory category)
        {
            return Array.IndexOf(categories, category) > -1;
        }

        public override Effect Create(object parent)
        {
            return new Weapon(this, parent);
        }
    }

    [Serializable]
    public class Weapon : Effect, IAttackAbilityRule, IAttackRollModifierRule
    {
        public Weapon(WeaponType type, object parent) : base(type, parent) { }
        public WeaponType weaponType => (WeaponType)type;

        public SingleValue<Ability> GetAttackAbility(Actions.Attack attack)
        {
            // Only provide information for the current weapon.
            if (attack.weapon != parent) return null;

            // Melee weapons use Strength, ranged weapons use Dexterity.
            return new SingleValue<Ability>(this, weaponType.HasCategory(WeaponCategory.Ranged) ? Ability.Dexterity : Ability.Strength);
        }

        public IntegerValue GetAttackRollModifier(Actions.Attack attack)
        {
            // Only provide information for the current weapon.
            if (attack.weapon != parent) return null;

            // Weapon provides the proficiency bonus modifier if its wielder is proficient with it.
            DebugHelpers.StartLog("Determining weapon proficiencies … ");
            WeaponCategory[] proficientWeaponCategories = Game.GetRuleValues((IWeaponProficiencyRule rule) => rule.GetWeaponProficiency(attack.attacker)).Resolve();
            DebugHelpers.EndLog();

            // The attacker must be proficient in at least one of the weapon's categories to get the proficiency bonus.
            if (proficientWeaponCategories.Intersect(weaponType.categories).Any())
            {
                return new IntegerValue(this, modifierValue: attack.attacker.proficiencyBonus);
            }

            return null;
        }
    }

    public enum WeaponCategory
    {
        None,
        Simple,
        Martial,
        Melee,
        Ranged,
        HandCrossbow,
        Longsword,
        Rapier,
        Shortsword,
        Club,
        Dagger,
        Dart,
        Javelin,
        Mace,
        Quarterstaff,
        Scimitar,
        Sickle,
        Sling,
        Spear,
        LightCrossbow
    }
}
