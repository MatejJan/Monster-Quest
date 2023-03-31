namespace MonsterQuest
{
    public interface IReactToDamageRule
    {
        public void ReactToDamage(Damage damage) { }

        public void ReactToDamageDealt(Damage damage) { }
    }
}
