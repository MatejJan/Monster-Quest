using System;

namespace MonsterQuest.Effects
{
    public abstract class RangedAttackType : AttackType
    {
        public int range;

        protected override string GetDistanceDescription()
        {
            return $"range {range} ft.";
        }
    }

    [Serializable]
    public class RangedAttack : Attack
    {
        public RangedAttack(EffectType type, object parent) : base(type, parent) { }
    }
}
