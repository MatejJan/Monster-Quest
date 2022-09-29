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

    public class MagicWeapon : Effect, IAttackRollModifierRule, IDamageRollModifierRule
    {
        public MagicWeapon(EffectType type, object parent) : base(type, parent) { }
        public MagicWeaponType magicWeaponType => (MagicWeaponType)type;

        public IntegerValue GetAttackRollModifier(Actions.Attack attack)
        {
            return GetRollModifier(attack);
        }

        public IntegerValue GetDamageRollModifier(Actions.Attack attack)
        {
            return GetRollModifier(attack);
        }

        private IntegerValue GetRollModifier(Actions.Attack attack)
        {
            // Only provide information for this weapon.
            if (attack.weapon != parent) return null;

            return new IntegerValue(this, modifierValue: magicWeaponType.bonus);
        }
    }
}
