using System.Collections;

namespace MonsterQuest.Actions
{
    public class BeUnconscious : IAction
    {
        public BeUnconscious(Character character)
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
