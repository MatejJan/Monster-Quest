using System;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Swarm Melee Weapon Attack", menuName = "Effects/Swarm Melee Weapon Attack")]
    public class SwarmMeleeWeaponAttackType : MeleeWeaponAttackType
    {
        public override Effect Create(object parent)
        {
            return new SwarmMeleeWeaponAttack(this, parent);
        }

        protected override string GetDamageRollDescription(DamageRoll damageRoll, int? damageModifier = null)
        {
            string mainDescription = base.GetDamageRollDescription(damageRoll, damageModifier);

            if (damageRoll != damageRolls[0]) return mainDescription;

            DamageRoll halvedDamageRoll = new(DiceHelper.GetRollWithHalfTheDice(damageRoll.roll), damageRoll.type, damageRoll.isExtraDamage, damageRoll.savingThrowAbility, damageRoll.savingThrowDC);

            return $"{mainDescription}, or {GetDamageRollDescription(halvedDamageRoll, damageModifier)} if the swarm has half of its hit points or fewer";
        }

        protected override string GetTargetDescription()
        {
            return $"one {targetType.GetDescription()} in the swarm’s space";
        }
    }

    [Serializable]
    public class SwarmMeleeWeaponAttack : MeleeWeaponAttack
    {
        public SwarmMeleeWeaponAttack(EffectType type, object parent) : base(type, parent) { }
        public SwarmMeleeWeaponAttackType swarmMeleeWeaponAttackType => (SwarmMeleeWeaponAttackType)type;

        public override ArrayValue<DamageRoll> GetDamageRolls(Hit hit)
        {
            // Only provide information to attacks with this weapon.
            if (!IsOwnAttack(hit.attackAction)) return null;

            // Deal half the damage if the swarm has half of its hit points or fewer.
            Creature attacker = hit.attackAction.attacker;

            if (attacker.hitPoints > attacker.hitPointsMaximum / 2) return base.GetDamageRolls(hit);

            DamageRoll normalDamageRoll = swarmMeleeWeaponAttackType.damageRolls[0];
            DamageRoll halvedDamageRoll = new(DiceHelper.GetRollWithHalfTheDice(normalDamageRoll.roll), normalDamageRoll.type, normalDamageRoll.isExtraDamage, normalDamageRoll.savingThrowAbility, normalDamageRoll.savingThrowDC);

            DamageRoll[] damageRolls = new DamageRoll[swarmMeleeWeaponAttackType.damageRolls.Length];
            damageRolls[0] = halvedDamageRoll;

            for (int i = 1; i < damageRolls.Length; i++)
            {
                damageRolls[i] = swarmMeleeWeaponAttackType.damageRolls[i];
            }

            return new ArrayValue<DamageRoll>(this, damageRolls);
        }
    }
}
