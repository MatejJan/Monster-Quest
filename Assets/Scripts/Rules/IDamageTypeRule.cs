namespace MonsterQuest
{
    public interface IDamageTypeRule
    {
        ArrayValue<DamageType> GetDamageTypeVulnerabilities(DamageAmount damageAmount);
        ArrayValue<DamageType> GetDamageTypeResistances(DamageAmount damageAmount);
        ArrayValue<DamageType> GetDamageTypeImmunities(DamageAmount damageAmount);
    }
}
