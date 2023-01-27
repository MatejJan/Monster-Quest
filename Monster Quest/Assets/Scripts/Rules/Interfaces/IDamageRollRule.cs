namespace MonsterQuest
{
    public interface IDamageRollRule
    {
        public ArrayValue<DamageRoll> GetDamageRolls(Hit hit);
    }
}
