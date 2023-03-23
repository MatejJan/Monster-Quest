using System;
using System.Linq;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Effects/Weapon")]
    public class WeaponType : EffectType, IRulesProvider, IInformativeMonsterAttackAttackAbilityRule, IInformativeMonsterAttackAttackRollModifierRule
    {
        public WeaponCategory[] categories;

        public SingleValue<Ability> GetAttackAbility(InformativeMonsterAttackAction attackAction)
        {
            // Only provide information for the current weapon.
            if (!attackAction.weapon.effects.Contains(this)) return null;

            // Melee weapons use Strength, ranged weapons use Dexterity.
            return new SingleValue<Ability>(this, HasCategory(WeaponCategory.Ranged) ? Ability.Dexterity : Ability.Strength);
        }

        public IntegerValue GetAttackRollModifier(InformativeMonsterAttackAction attackAction)
        {
            // Only provide information for the current weapon.
            if (!attackAction.weapon.effects.Contains(this)) return null;

            // Assume the monster is proficient with this weapon.
            return new IntegerValue(this, modifierValue: attackAction.attacker.proficiencyBonus);
        }

        public string rulesProviderName => name;

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

        public SingleValue<Ability> GetAttackAbility(AttackAction attackAction)
        {
            // Only provide information for the current weapon.
            if (attackAction.weapon != parent) return null;

            // Melee weapons use Strength, ranged weapons use Dexterity.
            return new SingleValue<Ability>(this, weaponType.HasCategory(WeaponCategory.Ranged) ? Ability.Dexterity : Ability.Strength);
        }

        public IntegerValue GetAttackRollModifier(AttackAction attackAction)
        {
            // Only provide information for the current weapon.
            if (attackAction.weapon != parent) return null;

            // Weapon provides the proficiency bonus modifier if its wielder is proficient with it.
            DebugHelper.StartLog("Determining weapon proficiencies … ");
            WeaponCategory[] weaponProficiencies = attackAction.gameState.GetRuleValues((IWeaponProficiencyRule rule) => rule.GetWeaponProficiency(attackAction.attacker)).Resolve();
            DebugHelper.EndLog();

            // The attacker must be proficient in at least one of the weapon's categories to get the proficiency bonus.
            if (weaponProficiencies.Intersect(weaponType.categories).Any())
            {
                return new IntegerValue(this, modifierValue: attackAction.attacker.proficiencyBonus);
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
