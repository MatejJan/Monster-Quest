using System;

namespace MonsterQuest
{
    [Serializable]
    public abstract class Effect : IRulesProvider
    {
        public EffectType type;
        public object parent;

        public Effect(EffectType type, object parent)
        {
            this.type = type;
            this.parent = parent;
        }

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
