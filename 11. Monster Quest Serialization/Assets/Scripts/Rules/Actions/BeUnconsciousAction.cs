using System.Collections;

namespace MonsterQuest
{
    public class BeUnconsciousAction : IAction
    {
        private Character _character;
        
        public BeUnconsciousAction(Character character)
        {
            _character = character;
        }

        public IEnumerator Execute()
        {
            yield return _character.HandleUnconsciousState();
        }
    }
}
