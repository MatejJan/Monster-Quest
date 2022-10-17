using System.Collections;

namespace MonsterQuest
{
    public class BeUnconsciousAction : IAction
    {
        public BeUnconsciousAction(Character character)
        {
            this.character = character;
        }

        private Character character { get; }

        public IEnumerator Execute()
        {
            yield return character.HandleUnconsciousState();
        }
    }
}
