namespace MonsterQuest.Events
{
    public class HealEvent
    {
        public int amount;
        public Creature creature;
        public int hitPointsEnd;
        public int hitPointsMaximum;
        public int hitPointsStart;
    }
}
