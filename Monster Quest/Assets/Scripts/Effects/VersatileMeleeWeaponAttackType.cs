using System;
using System.Linq;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Versatile Melee Weapon Attack", menuName = "Effects/Versatile Melee Weapon Attack")]
    public class VersatileMeleeWeaponAttackType : MeleeWeaponAttackType
    {
        public string twoHandedDamageRoll;

        public override Effect Create(object parent)
        {
            return new VersatileMeleeWeaponAttack(this, parent);
        }

        protected override string GetDamageRollDescription(DamageRoll damageRoll, int? damageModifier = null)
        {
            string mainDescription = base.GetDamageRollDescription(damageRoll, damageModifier);

            if (damageRoll != damageRolls[0]) return mainDescription;

            return $"{mainDescription}, or {twoHandedDamageRoll}{GetDamageModifierDescription(damageModifier, false)} {damageRoll.type.ToString().ToLower()} damage if used with two hands to make a melee attack";
        }
    }

    [Serializable]
    public class VersatileMeleeWeaponAttack : MeleeWeaponAttack
    {
        public VersatileMeleeWeaponAttack(EffectType type, object parent) : base(type, parent) { }
        public VersatileMeleeWeaponAttackType versatileMeleeWeaponAttackType => (VersatileMeleeWeaponAttackType)type;

        public override ArrayValue<DamageRoll> GetDamageRolls(Hit hit)
        {
            // Only provide information to attacks with this weapon.
            if (!IsOwnAttack(hit.attackAction)) return null;

            // If the attack is made with two hands, use the two handed damage roll.
            // For now we assume two handed attacks when the attacker has only one weapon.
            if (hit.attackAction.attacker.items.Count(item => item.GetEffect<Weapon>() is not null) > 1) return base.GetDamageRolls(hit);

            DamageRoll oneHandedDamageRoll = versatileMeleeWeaponAttackType.damageRolls[0];
            DamageRoll twoHandedDamageRoll = new(versatileMeleeWeaponAttackType.twoHandedDamageRoll, oneHandedDamageRoll.type, oneHandedDamageRoll.isExtraDamage, oneHandedDamageRoll.savingThrowAbility, oneHandedDamageRoll.savingThrowDC);

            DamageRoll[] damageRolls = new DamageRoll[versatileMeleeWeaponAttackType.damageRolls.Length];
            damageRolls[0] = twoHandedDamageRoll;

            for (int i = 1; i < damageRolls.Length; i++)
            {
                damageRolls[i] = versatileMeleeWeaponAttackType.damageRolls[i];
            }

            return new ArrayValue<DamageRoll>(this, damageRolls);
        }
    }
}
