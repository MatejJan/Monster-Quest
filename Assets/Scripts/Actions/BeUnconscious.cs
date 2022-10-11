using System.Collections;

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

        public IEnumerator Execute()
        {
            yield return character.HandleUnconsciousState();
        }
    }
}
