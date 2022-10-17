using System;

namespace MonsterQuest.Effects
{
    public abstract class AttackType : EffectType
    {
        public DamageRoll[] damageRolls;

        public Ability attackAbility;

        public bool customAttackModifiers;
        public int attackRollModifier;
        public int damageRollModifier;

        public string descriptionVerb;
        public string descriptionObject;
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
