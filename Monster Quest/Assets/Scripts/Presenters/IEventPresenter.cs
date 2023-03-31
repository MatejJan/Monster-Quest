using System.Collections;

namespace MonsterQuest.Presenters
{
    public interface IEventPresenter<in TEvent>
    {
        IEnumerator Present(TEvent eventData);
    }
}
