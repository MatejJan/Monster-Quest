using System.Collections.Generic;

namespace MonsterQuest
{
    public interface IRulesHandler
    {
        IEnumerable<object> rules { get; }
    }
}
