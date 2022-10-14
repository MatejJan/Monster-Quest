using System;
using System.Collections;
using MonsterQuest.Effects;
using UnityEngine;
using Attack = MonsterQuest.Actions.Attack;

namespace MonsterQuest
{
    [Serializable]
    public class Monster : Creature, IArmorClassRule, IDamageTypeRule, IAttackAbilityRule, IAttackRollModifierRule
    {
        public Monster(MonsterType type)
        {
            this.type = type;
            displayName = type.displayName;

            DebugHelpers.StartLog($"Creating {indefiniteName}.");

            // Roll the monster's hit points.
            DebugHelpers.StartLog("Determining hit points.");
            hitPointsMaximum = Math.Max(1, Dice.Roll(type.hitPointsRoll));
            hitPoints = hitPointsMaximum;
            DebugHelpers.EndLog();

            // Copy the rest of the properties from the monster type.
            foreach (Ability ability in Enum.GetValues(typeof(Ability)))
            {
                if (ability == Ability.None) continue;

                abilityScores[ability].score = type.abilityScores[ability];
            }

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

            DebugHelpers.EndLog($"Created {indefiniteName} with {hitPoints} HP.");
        }

        // State properties
        [field: SerializeField] public MonsterType type { get; private set; }

        // Derived properties
        public override SizeCategory size => type.size;

        public override Sprite bodySprite => type.bodySprite;
        public override float flyHeight => type.flyHeight;

        protected override int proficiencyBonusBase => (int)type.challengeRating;

        public IntegerValue GetArmorClass(Creature creature)
        {
            // Only provide information for the current monster.
            if (creature != this) return null;

            // Monsters have the armor class directly specified in the type.
            return new IntegerValue(this, type.armorClass);
        }

        public SingleValue<Ability> GetAttackAbility(Attack attack)
        {
            // Only provide information for our own inherent (non-weapon) attacks.
            if (attack.effect.parent != this) return null;

            // Melee attacks use Strength, ranged attacks use Dexterity.
            return new SingleValue<Ability>(this, attack.effect is RangedAttack ? Ability.Dexterity : Ability.Strength);
        }

        public IntegerValue GetAttackRollModifier(Attack attack)
        {
            // Only provide information for our own attacks.
            if (attack.attacker != this) return null;

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

        protected override IEnumerator TakeDamageAtZeroHP(int remainingDamageAmount, Hit hit)
        {
            // Monsters immediately die.
            yield return Die();
        }
    }
}
