using System;

namespace MonsterQuest
{
    public interface IStateEventProvider
    {
        event Action<object> stateEvent;

        void StartProvidingStateEvents() { }
    }
}
