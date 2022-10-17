using System;
using System.Linq;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Ranged Weapon Attack", menuName = "Effects/Ranged Weapon Attack")]
    public class RangedWeaponAttackType : RangedAttackType
    {
        public int longRange;

        public override Effect Create(object parent)
        {
            return new RangedWeaponAttack(this, parent);
        }
    }

    [Serializable]
    public class RangedWeaponAttack : RangedAttack, IAttackRollMethodRule
    {
        public RangedWeaponAttack(EffectType type, object parent) : base(type, parent) { }
        public RangedWeaponAttackType rangedWeaponAttackType => (RangedWeaponAttackType)attackType;

        public MultipleValue<AttackRollMethod> GetAttackRollMethod(AttackAction attackAction)
        {
            // Only provide information for the current attack.
            if (!IsOwnAttack(attackAction)) return null;

            // Shooting beyond the normal range results in a disadvantage.
            if (GameManager.state.combat.GetDistance(attackAction.attacker, attackAction.target) > rangedWeaponAttackType.range)
            {
                return new MultipleValue<AttackRollMethod>(this, AttackRollMethod.Disadvantage);
            }

            // Shooting next to a hostile creature results in a disadvantage.
            Creature nearestHostileCreature = GameManager.state.combat.GetCreatures().Where(creature => GameManager.state.combat.AreHostile(attackAction.attacker, creature)).OrderBy(hostile => GameManager.state.combat.GetDistance(attackAction.attacker, hostile)).First();

            if (nearestHostileCreature is not null && GameManager.state.combat.GetDistance(attackAction.attacker, nearestHostileCreature) <= 5)
            {
                return new MultipleValue<AttackRollMethod>(this, AttackRollMethod.Disadvantage);
            }

            return null;
        }
    }
}
