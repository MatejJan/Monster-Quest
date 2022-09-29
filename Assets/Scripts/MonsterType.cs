using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
    public class MonsterType : ScriptableObject
    {
        public enum TypeCategory
        {
            None,
            Aberration,
            Beast,
            Celestial,
            Construct,
            Dragon,
            Elemental,
            Fey,
            Fiend,
            Giant,
            Humanoid,
            Monstrosity,
            Ooze,
            Plant,
            Undead
        }

        public string displayName;
        public Creature.SizeCategory size;
        public TypeCategory type;
        public string alignment;
        public int armorClass;
        public string hitPointsRoll;
        public AbilityScores abilityScores = new();
        public DamageType[] damageVulnerabilities;
        public DamageType[] damageResistances;
        public DamageType[] damageImmunities;
        public float challengeRating;
        public EffectType[] effects;
        public ItemType[] items;
    }
}
