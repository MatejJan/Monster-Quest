using System;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Heavy", menuName = "Effects/Heavy")]
    public class HeavyType : EffectType
    {
        public override Effect Create(object parent)
        {
            return new Heavy(this, parent);
        }
    }

    [Serializable]
    public class Heavy : Effect, IAttackRollMethodRule
    {
        public Heavy(EffectType type, object parent) : base(type, parent) { }

        public MultipleValue<AttackRollMethod> GetAttackRollMethod(AttackAction attackAction)
        {
            // Only provide information for this weapon.
            if (attackAction.weapon != parent) return null;

            // Small creatures have disadvantage on attack rolls with heavy weapons.
            if (attackAction.attacker.sizeCategory > SizeCategory.Small) return null;

            return new MultipleValue<AttackRollMethod>(this, AttackRollMethod.Disadvantage);
        }
    }
}
