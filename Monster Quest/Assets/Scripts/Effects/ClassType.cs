using System;
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
            return new Class(this, parent, 1, out int _);
        }

        public Effect Create(object parent, int level, out int hitPointsMaximumIncrease)
        {
            return new Class(this, parent, level, out hitPointsMaximumIncrease);
        }
    }

    [Serializable]
    public class Class : Effect, IWeaponProficiencyRule, IArmorProficiencyRule
    {
        public Class(ClassType type, object parent, int level, out int hitPointsMaximumIncrease) : base(type, parent)
        {
            this.level = 1;
            availableHitDice = level;

            hitPointsMaximumIncrease = 0;

            while (this.level < level)
            {
                LevelUp(out int singleHitPointsMaximumIncrease);
                hitPointsMaximumIncrease += singleHitPointsMaximumIncrease;
            }
        }

        public ClassType classType => (ClassType)type;

        [field: SerializeField] public int level { get; private set; }
        [field: SerializeField] public int availableHitDice { get; private set; }

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

        public void LevelUp(out int hitPointsMaximumIncrease)
        {
            Character character = parent as Character;

            int rollResult = DiceHelper.Roll(classType.hitDice);

            hitPointsMaximumIncrease = rollResult + character.abilityScores.constitution.modifier;

            level++;
            availableHitDice++;
        }

        public void SpendHitDice()
        {
            Character character = parent as Character;

            int rollResult = DiceHelper.Roll(classType.hitDice);

            int hitPointsRegained = rollResult + character.abilityScores.constitution.modifier;

            availableHitDice--;

            character.Heal(hitPointsRegained);
        }

        public void RegainHitDice(int amount)
        {
            availableHitDice = Math.Min(level, availableHitDice + amount);
        }
    }
}
