using System;
using MonsterQuest.Effects;
using Attack = MonsterQuest.Actions.Attack;

namespace MonsterQuest
{
    public class Monster : Creature, IArmorClassRule, IDamageTypeRule, IAttackAbilityRule
    {
        public Monster(MonsterType type)
        {
            this.type = type;
            name = type.displayName;

            DebugHelper.StartLog($"Creating {indefiniteName}.");

            // Roll the monster's hit points.
            DebugHelper.StartLog("Determining hit points.");
            hitPointsMaximum = Dice.Roll(type.hitPointsRoll);
            hitPoints = hitPointsMaximum;
            DebugHelper.EndLog();

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

            DebugHelper.EndLog($"Created {indefiniteName} with {hitPoints} HP.");
        }

        public MonsterType type { get; }

        public override SizeCategory size => type.size;

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
            // Only provide information for our own attacks.
            if (attack.effect.parent != this) return null;

            // Melee attacks use Strength, ranged attacks use Dexterity.
            return new SingleValue<Ability>(this, attack.effect is RangedAttack ? Ability.Dexterity : Ability.Strength);
        }

        public ArrayValue<DamageType> GetDamageTypeResistances(Damage damage)
        {
            // Only provide information for the current monster.
            if (damage.hit.target != this) return null;

            return new ArrayValue<DamageType>(this, type.damageResistances);
        }

        public ArrayValue<DamageType> GetDamageTypeImmunities(Damage damage)
        {
            // Only provide information for the current monster.
            if (damage.hit.target != this) return null;

            return new ArrayValue<DamageType>(this, type.damageImmunities);
        }

        public ArrayValue<DamageType> GetDamageTypeVulnerabilities(Damage damage)
        {
            // Only provide information for the current monster.
            if (damage.hit.target != this) return null;

            return new ArrayValue<DamageType>(this, type.damageVulnerabilities);
        }

        protected override void HandleZeroHP(int remainingDamageAmount, Hit hit)
        {
            // Monsters immediately die.
            Die();
        }
    }
}
