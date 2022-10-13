using System;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public abstract class Effect : IRulesProvider
    {
        protected Effect(EffectType type, object parent)
        {
            this.type = type;
            this.parent = parent;
        }

        [field: SerializeField] public EffectType type { get; private set; }
        [field: SerializeReference] public object parent { get; private set; }

        public string rulesProviderName
        {
            get
            {
                // Effects on an item use the item name.
                if (parent is Item item)
                {
                    return item.type.displayName;
                }

                // Otherwise use the scriptable object name.
                return type.name;
            }
        }
    }
}
