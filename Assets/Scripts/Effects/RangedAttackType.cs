namespace MonsterQuest.Effects
{
    public abstract class RangedAttackType : AttackType
    {
        public int range;
    }

    public class RangedAttack : Attack
    {
        public RangedAttack(EffectType type, object parent) : base(type, parent) { }
    }
}
