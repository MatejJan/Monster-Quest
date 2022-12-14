using System;
using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
    public class MonsterType : ScriptableObject
    {
        // Main stat block
        public string displayName;
        public SizeCategory sizeCategory;
        public MonsterTypeCategory typeCategory;
        public string[] typeTags;
        public string alignment;
        public int armorClass;
        public string hitPointsRoll;
        public Speed speed;
        public AbilityScores abilityScores = new();
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

        [Serializable]
        public class Speed
        {
            public float walk;
            public float burrow;
            public float climb;
            public float fly;
            public float swim;
            public bool hover;
        }

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
