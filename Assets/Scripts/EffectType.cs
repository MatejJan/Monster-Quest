using UnityEngine;

namespace MonsterQuest
{
    public abstract class EffectType : ScriptableObject
    {
        public abstract Effect Create(object parent);
    }
}
