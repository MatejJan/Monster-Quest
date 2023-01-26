using System;

namespace MonsterQuest
{
    [Flags]
    public enum TargetType
    {
        None = 0,
        Creature = 1,
        Object = 1 << 1,
        Location = 1 << 2,
        All = Creature | Object | Location
    }
}
