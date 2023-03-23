using System;
using System.Linq;
using MonsterQuest.Effects;
using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
    public class MonsterType : ScriptableObject, IRulesProvider, IInformativeMonsterAttackAttackAbilityRule, IInformativeMonsterAttackAttackRollModifierRule
    {
        // Main stat block
        public string displayName;
        public SizeCategory sizeCategory;
        public MonsterTypeCategory typeCategory;
        public string subtype;
        public string alignment;
        public int armorClass;
        public string hitPointsRoll;
        public Speed speed;
        public AbilityScores abilityScores;
        public SavingThrowBonus[] savingThrowBonuses;
        public SkillBonus[] skillBonuses;
        public DamageType[] damageVulnerabilities;
        public DamageType[] damageResistances;
        public DamageType[] damageImmunities;
        public Condition[] conditionImmunities;
        public SenseRange[] senseRanges;
        public bool blind;
        public LanguageAbility[] languageAbilities;
        public int telepathyRange;
        public float challengeRating;

        // Traits, actions, reactions, equipment
        public EffectType[] effects;
        public ItemType[] items;

        // Visuals
        public Sprite bodySprite;
        public float flyHeight;

        public int passivePerception
        {
            get
            {
                SkillBonus perceptionBonus = skillBonuses.FirstOrDefault(bonus => bonus.skill == Skill.Perception);

                return 10 + (perceptionBonus?.amount ?? abilityScores.wisdom.modifier);
            }
        }

        public int experiencePoints
        {
            get
            {
                return challengeRating switch
                {
                    0 => hasEffectiveAttacks ? 10 : 0,
                    0.125f => 25,
                    0.25f => 50,
                    0.5f => 100,
                    1 => 200,
                    2 => 450,
                    3 => 700,
                    4 => 1100,
                    5 => 1800,
                    6 => 2300,
                    7 => 2900,
                    8 => 3900,
                    9 => 5000,
                    10 => 5900,
                    11 => 7200,
                    12 => 8400,
                    13 => 10000,
                    14 => 11500,
                    15 => 13000,
                    16 => 15000,
                    17 => 18000,
                    18 => 20000,
                    19 => 22000,
                    20 => 25000,
                    21 => 33000,
                    22 => 41000,
                    23 => 50000,
                    24 => 62000,
                    25 => 75000,
                    26 => 90000,
                    27 => 105000,
                    28 => 120000,
                    29 => 135000,
                    30 => 155000,
                    _ => 0
                };
            }
        }

        private bool hasEffectiveAttacks
        {
            get
            {
                // Possessing an item with an attack effect counts as an effective attack.
                if (items.Any(itemType => itemType.GetEffect<AttackType>())) return true;

                // Having an attack effect directly counts as an effective attack.
                if (effects.Any(effectType => effectType is AttackType)) return true;

                return false;
            }
        }

        public int proficiencyBonus => CreatureRules.GetProficiencyBonus((int)challengeRating);

        public SingleValue<Ability> GetAttackAbility(InformativeMonsterAttackAction attackAction)
        {
            // Only provide information for our own inherent (non-weapon) attacks.
            if (!effects.Contains(attackAction.effect)) return null;

            // Melee attacks use Strength, ranged attacks use Dexterity.
            return new SingleValue<Ability>(this, attackAction.effect is RangedAttackType ? Ability.Dexterity : Ability.Strength);
        }

        public IntegerValue GetAttackRollModifier(InformativeMonsterAttackAction attackAction)
        {
            // Only provide information for our own inherent (non-weapon) attacks.
            if (!effects.Contains(attackAction.effect)) return null;

            // Return the proficiency bonus modifier.
            return new IntegerValue(this, 0, proficiencyBonus);
        }

        // Derived properties

        public string rulesProviderName => displayName;

        // Classes

        [Serializable]
        public class SavingThrowBonus
        {
            public Ability ability;
            public int amount;
        }

        [Serializable]
        public class SkillBonus
        {
            public Skill skill;
            public int amount;
        }

        [Serializable]
        public class SenseRange
        {
            public Sense sense;
            public int range;
        }

        [Serializable]
        public class LanguageAbility
        {
            public Language language;
            public bool canSpeak;
        }
    }
}
