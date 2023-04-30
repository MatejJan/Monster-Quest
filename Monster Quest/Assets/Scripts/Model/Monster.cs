using System;
using System.Collections.Generic;
using MonsterQuest.Effects;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class Monster : Creature, IArmorClassRule, IDamageTypeRule, IAttackAbilityRule, IAttackRollModifierRule
    {
        private static readonly bool[] _deathSavingThrows = Array.Empty<bool>();

        public Monster(MonsterType type)
        {
            this.type = type;
            displayName = type.displayName;

            DebugHelper.StartLog($"Creating {indefiniteName}.");

            // Roll the monster's hit points.
            DebugHelper.StartLog("Determining hit points.");
            hitPointsMaximum = Math.Max(1, DiceHelper.Roll(type.hitPointsRoll));
            DebugHelper.EndLog();

            // Give items to the monster.
            foreach (ItemType itemType in type.items)
            {
                Item item = itemType.Create();
                itemsList.Add(item);
            }

            // Create all effects of the monster.
            foreach (EffectType effectType in type.effects)
            {
                effectsList.Add(effectType.Create(this));
            }

            Initialize();

            DebugHelper.EndLog($"Created {indefiniteName} with {hitPoints} HP.");
        }

        // State properties
        [field: SerializeField] public MonsterType type { get; private set; }

        // Derived properties
        public override AbilityScores abilityScores => type.abilityScores;
        public override SizeCategory sizeCategory => type.sizeCategory;

        public override string bodyAssetName => type.name;

        protected override int proficiencyBonusBase => (int)type.challengeRating;

        public override IEnumerable<bool> deathSavingThrows => _deathSavingThrows;

        public IntegerValue GetArmorClass(Creature creature)
        {
            // Only provide information for the current monster.
            if (creature != this) return null;

            // Monsters have the armor class directly specified in the type.
            return new IntegerValue(this, type.armorClass);
        }

        public SingleValue<Ability> GetAttackAbility(AttackAction attackAction)
        {
            // Only provide information for our own inherent (non-weapon) attacks.
            if (attackAction.effect.parent != this) return null;

            // Melee attacks use Strength, ranged attacks use Dexterity.
            return new SingleValue<Ability>(this, attackAction.effect is RangedAttack ? Ability.Dexterity : Ability.Strength);
        }

        public IntegerValue GetAttackRollModifier(AttackAction attackAction)
        {
            // Only provide information for our own attacks.
            if (attackAction.attacker != this) return null;

            // Return the proficiency bonus modifier.
            return new IntegerValue(this, 0, proficiencyBonus);
        }

        public ArrayValue<DamageType> GetDamageTypeResistances(DamageAmount damageAmount)
        {
            // Only provide information for the current monster.
            if (damageAmount.hit.target != this) return null;

            return new ArrayValue<DamageType>(this, type.damageResistances);
        }

        public ArrayValue<DamageType> GetDamageTypeImmunities(DamageAmount damageAmount)
        {
            // Only provide information for the current monster.
            if (damageAmount.hit.target != this) return null;

            return new ArrayValue<DamageType>(this, type.damageImmunities);
        }

        public ArrayValue<DamageType> GetDamageTypeVulnerabilities(DamageAmount damageAmount)
        {
            // Only provide information for the current monster.
            if (damageAmount.hit.target != this) return null;

            return new ArrayValue<DamageType>(this, type.damageVulnerabilities);
        }

        protected override void TakeDamageAtZeroHitPoints(int remainingDamageAmount, Hit hit)
        {
            // Monsters immediately die.
            Die();
        }
    }
}
