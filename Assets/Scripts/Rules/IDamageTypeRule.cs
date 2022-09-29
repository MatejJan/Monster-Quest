namespace MonsterQuest
{
    public interface IDamageTypeRule
    {
        ArrayValue<DamageType> GetDamageTypeVulnerabilities(Damage damage);
        ArrayValue<DamageType> GetDamageTypeResistances(Damage damage);
        ArrayValue<DamageType> GetDamageTypeImmunities(Damage damage);
    }
}
