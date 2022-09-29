namespace MonsterQuest
{
    public interface IDamageRule
    {
        public void ReactToDamage(Damage damage) { }

        public void ReactToDamageDealt(Damage damage) { }
    }
}
