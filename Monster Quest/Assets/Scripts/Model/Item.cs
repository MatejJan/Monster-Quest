using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class Item : IRulesHandler
    {
        public Item(ItemType type)
        {
            this.type = type;

            // Create all effects of this item.
            effects = new List<Effect>();

            foreach (EffectType effectType in type.effects)
            {
                effects.Add(effectType.Create(this));
            }
        }

        // State properties
        [field: SerializeReference] public List<Effect> effects { get; private set; }
        [field: SerializeField] public ItemType type { get; private set; }

        // Derived properties
        public string displayName => type.displayName;

        public string definiteName => EnglishHelper.GetDefiniteNounForm(displayName);
        public string indefiniteName => EnglishHelper.GetIndefiniteNounForm(displayName);

        public IEnumerable<object> rules => effects;

        public T GetEffect<T>() where T : Effect
        {
            return effects.Find(effect => effect is T) as T;
        }

        public IEnumerable<T> GetEffects<T>() where T : Effect
        {
            return effects.OfType<T>();
        }

        public bool HasEffect<T>()
        {
            return effects.Any(effect => effect is T);
        }
    }
}
