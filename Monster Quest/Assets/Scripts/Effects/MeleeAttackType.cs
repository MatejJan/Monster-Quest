using System;

namespace MonsterQuest.Effects
{
    public abstract class MeleeAttackType : AttackType
    {
        public int reach = 5;

        protected override string GetDistanceDescription()
        {
            return $"reach {reach} ft.";
        }
    }

    [Serializable]
    public class MeleeAttack : Attack
    {
        public MeleeAttack(EffectType type, object parent) : base(type, parent) { }
    }
}
