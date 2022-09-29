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

    public class Heavy : Effect, IAttackRollMethodRule
    {
        public Heavy(EffectType type, object parent) : base(type, parent) { }

        public MultipleValue<AttackRollMethod> GetAttackRollMethod(Actions.Attack attack)
        {
            // Only provide information for this weapon.
            if (attack.weapon != parent) return null;

            // Small creatures have disadvantage on attack rolls with heavy weapons.
            if (attack.attacker.size > Creature.SizeCategory.Small) return null;

            return new MultipleValue<AttackRollMethod>(this, AttackRollMethod.Disadvantage);
        }
    }
}
