using System;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Magic Weapon", menuName = "Effects/Magical/Magic Weapon")]
    public class MagicWeaponType : EffectType
    {
        public int bonus;

        public override Effect Create(object parent)
        {
            return new MagicWeapon(this, parent);
        }
    }

    [Serializable]
    public class MagicWeapon : Effect, IAttackRollModifierRule, IDamageRollModifierRule
    {
        public MagicWeapon(EffectType type, object parent) : base(type, parent) { }
        public MagicWeaponType magicWeaponType => (MagicWeaponType)type;

        public IntegerValue GetAttackRollModifier(AttackAction attackAction)
        {
            return GetRollModifier(attackAction);
        }

        public IntegerValue GetDamageRollModifier(AttackAction attackAction)
        {
            return GetRollModifier(attackAction);
        }

        private IntegerValue GetRollModifier(AttackAction attackAction)
        {
            // Only provide information for this weapon.
            if (attackAction.weapon != parent) return null;

            return new IntegerValue(this, modifierValue: magicWeaponType.bonus);
        }
    }
}
