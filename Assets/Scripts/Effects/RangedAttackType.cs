using System;

namespace MonsterQuest.Effects
{
    public abstract class RangedAttackType : AttackType
    {
        public int range;
    }

    [Serializable]
    public class RangedAttack : Attack
    {
        public RangedAttack(EffectType type, object parent) : base(type, parent) { }
    }
}
