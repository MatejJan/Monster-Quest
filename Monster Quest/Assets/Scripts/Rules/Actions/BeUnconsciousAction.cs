namespace MonsterQuest
{
    public class BeUnconsciousAction : IAction
    {
        public BeUnconsciousAction(Character character)
        {
            this.character = character;
        }

        private Character character { get; }

        public void Execute()
        {
            character.HandleUnconsciousState();
        }
    }
}
