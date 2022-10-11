using System.Collections;

namespace MonsterQuest.Actions
{
    public interface IAction
    {
        public IEnumerator Execute();
    }
}
