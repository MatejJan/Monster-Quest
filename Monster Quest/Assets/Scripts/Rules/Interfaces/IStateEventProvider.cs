using System;

namespace MonsterQuest
{
    public interface IStateEventProvider
    {
        event Action<string> stateEvent;

        void StartProvidingStateEvents() { }
    }
}
