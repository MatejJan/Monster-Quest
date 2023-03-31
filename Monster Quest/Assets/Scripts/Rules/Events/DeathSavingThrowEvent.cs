namespace MonsterQuest.Events
{
    public class DeathSavingThrowEvent
    {
        public int amount;
        public Creature creature;
        public bool[] deathSavingThrows;
        public RollResult rollResult;
        public bool succeeded;
    }
}
