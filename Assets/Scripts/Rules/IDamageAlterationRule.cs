namespace MonsterQuest
{
    public interface IDamageAlterationRule
    {
        public DamageAlterationValue GetDamageAlteration(Damage damage);
    }
}
