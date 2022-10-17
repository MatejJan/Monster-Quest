using System.Collections;

namespace MonsterQuest
{
    public interface IAction
    {
        public IEnumerator Execute();
    }
}
