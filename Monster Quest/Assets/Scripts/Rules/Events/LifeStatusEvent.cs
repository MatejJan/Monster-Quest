namespace MonsterQuest.Events
{
    public class LifeStatusEvent
    {
        public Creature creature;
        public LifeStatus newLifeStatus;
        public LifeStatus previousLifeStatus;
    }
}
