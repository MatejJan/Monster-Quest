using System;

namespace MonsterQuest.Effects
{
    public abstract class MeleeAttackType : AttackType
    {
        public int reach = 5;
    }

    [Serializable]
    public class MeleeAttack : Attack
    {
        public MeleeAttack(EffectType type, object parent) : base(type, parent) { }
    }
}
