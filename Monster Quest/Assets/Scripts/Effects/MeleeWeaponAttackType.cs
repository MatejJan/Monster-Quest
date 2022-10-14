using System;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Melee Weapon Attack", menuName = "Effects/Melee Weapon Attack")]
    public class MeleeWeaponAttackType : MeleeAttackType
    {
        public override Effect Create(object parent)
        {
            return new MeleeWeaponAttack(this, parent);
        }
    }

    [Serializable]
    public class MeleeWeaponAttack : MeleeAttack
    {
        public MeleeWeaponAttack(EffectType type, object parent) : base(type, parent) { }
    }
}
