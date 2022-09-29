namespace MonsterQuest.Actions
{
    public class BeUnconscious : IAction
    {
        public BeUnconscious(Battle battle, Character character)
        {
            this.battle = battle;
            this.character = character;
        }

        public Battle battle { get; }
        public Character character { get; }

        public void Execute()
        {
            character.HandleUnconsciousState();
        }
    }
}
