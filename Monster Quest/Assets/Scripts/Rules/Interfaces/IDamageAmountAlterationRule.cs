namespace MonsterQuest
{
    public interface IDamageAmountAlterationRule
    {
        public DamageAmountAlterationValue GetDamageAlteration(DamageAmount damageAmount);
    }
}
