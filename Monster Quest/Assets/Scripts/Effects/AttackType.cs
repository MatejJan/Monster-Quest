using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest.Effects
{
    public abstract class AttackType : EffectType
    {
        public TargetType targetType;
        public DamageRoll[] damageRolls;

        public Ability attackAbility;

        public bool customAttackModifiers;
        public int attackRollModifier;
        public int damageRollModifier;

        public string displayName;
        public string descriptionVerb;
        public string descriptionObject;

        public virtual string typeName => "attack";
        public string description => GetDescription();

        public string GetDescription(int? attackerAttackRollModifier = null, int? attackerDamageRollModifier = null)
        {
            List<string> descriptionParts = new();

            // Generate to hit description.
            if (customAttackModifiers || attackerAttackRollModifier.HasValue)
            {
                int attackModifier = customAttackModifiers ? attackRollModifier : attackerAttackRollModifier.Value;
                descriptionParts.Add($"{attackModifier:+#;-#;+0} to hit");
            }

            // Generate reach description.
            string distanceDescription = GetDistanceDescription();

            if (distanceDescription != null)
            {
                descriptionParts.Add(distanceDescription);
            }

            // Generate target description.
            descriptionParts.Add(GetTargetDescription());

            // Generate hit description.
            IEnumerable<DamageRoll> simpleDamageRolls = damageRolls.Where(damageRoll => damageRoll.savingThrowAbility == Ability.None);
            IEnumerable<DamageRoll> savingThrowDamageRolls = damageRolls.Where(damageRoll => damageRoll.savingThrowAbility != Ability.None);

            string[] damageRollDescriptions = simpleDamageRolls.Select(damageRoll => GetDamageRollDescription(damageRoll, attackerDamageRollModifier)).ToArray();

            if (damageRollDescriptions.Length == 0) return null;

            string hitDescription = $"{damageRollDescriptions[0]}.";

            if (damageRollDescriptions.Length > 1)
            {
                hitDescription += $", plus {EnglishHelper.JoinWithAnd(damageRollDescriptions[1..])}.";
            }

            foreach (DamageRoll damageRoll in savingThrowDamageRolls)
            {
                hitDescription += $" The target must make a DC {damageRoll.savingThrowDC} {damageRoll.savingThrowAbility} " + $"saving throw, taking {GetDamageRollDescription(damageRoll)} on a failed save, " + "or half as much damage on a successful one.";
            }

            // Return combined description.
            return $"{string.Join(", ", descriptionParts).ToUpperFirst()}. Hit: {hitDescription}";
        }

        protected virtual string GetDistanceDescription()
        {
            return null;
        }

        protected virtual string GetTargetDescription()
        {
            return $"one {targetType.GetDescription()}";
        }

        protected virtual string GetDamageRollDescription(DamageRoll damageRoll, int? damageModifier = null)
        {
            return $"{damageRoll.roll}{GetDamageModifierDescription(damageModifier, damageRoll.isExtraDamage)} {damageRoll.type.ToString().ToLower()} damage";
        }

        protected string GetDamageModifierDescription(int? damageModifier, bool isExtraDamage)
        {
            return damageModifier.HasValue && damageModifier != 0 && !isExtraDamage ? $"{damageModifier:+#;-#}" : "";
        }
    }

    [Serializable]
    public abstract class Attack : Effect, IAttackAbilityRule, IAttackRollModifierRule, IDamageRollModifierRule, IDamageRollRule
    {
        protected Attack(EffectType type, object parent) : base(type, parent) { }
        public AttackType attackType => (AttackType)type;

        public SingleValue<Ability> GetAttackAbility(AttackAction attackAction)
        {
            // Only provide information for the current attack.
            if (!IsOwnAttack(attackAction)) return null;

            // The attack type can specify its own attack ability.
            if (attackType.attackAbility != Ability.None) return new SingleValue<Ability>(this, attackType.attackAbility);

            return null;
        }

        public IntegerValue GetAttackRollModifier(AttackAction attackAction)
        {
            // Only provide information for the current attack.
            if (!IsOwnAttack(attackAction)) return null;

            // The attack type can override the attack roll modifier.
            if (attackType.customAttackModifiers) return new IntegerValue(this, overrideValue: attackType.attackRollModifier);

            return null;
        }

        public IntegerValue GetDamageRollModifier(AttackAction attackAction)
        {
            // Only provide information for the current attack.
            if (!IsOwnAttack(attackAction)) return null;

            // The attack type can override the damage roll modifier.
            if (attackType.customAttackModifiers) return new IntegerValue(this, overrideValue: attackType.damageRollModifier);

            return null;
        }

        public virtual ArrayValue<DamageRoll> GetDamageRolls(Hit hit)
        {
            // Only provide information to attacks with this weapon.
            if (!IsOwnAttack(hit.attackAction)) return null;

            return new ArrayValue<DamageRoll>(this, attackType.damageRolls);
        }

        protected bool IsOwnAttack(AttackAction attackAction)
        {
            return attackAction.effect == this && (attackAction.weapon == parent || attackAction.attacker == parent);
        }
    }
}
