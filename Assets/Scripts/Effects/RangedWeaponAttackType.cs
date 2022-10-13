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

        public MultipleValue<AttackRollMethod> GetAttackRollMethod(Actions.Attack attack)
        {
            // Only provide information for the current attack.
            if (!IsOwnAttack(attack)) return null;

            // Shooting beyond the normal range results in a disadvantage.
            if (Game.state.battle.GetDistance(attack.attacker, attack.target) > rangedWeaponAttackType.range)
            {
                return new MultipleValue<AttackRollMethod>(this, AttackRollMethod.Disadvantage);
            }

            // Shooting next to a hostile creature results in a disadvantage.
            Creature nearestHostileCreature = Game.state.battle.GetCreatures().Where(creature => Game.state.battle.AreHostile(attack.attacker, creature)).OrderBy(hostile => Game.state.battle.GetDistance(attack.attacker, hostile)).First();

            if (nearestHostileCreature != null && Game.state.battle.GetDistance(attack.attacker, nearestHostileCreature) <= 5)
            {
                return new MultipleValue<AttackRollMethod>(this, AttackRollMethod.Disadvantage);
            }

            return null;
        }
    }
}
