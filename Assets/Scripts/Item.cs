using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class Item : IRulesHandler
    {
        public List<Effect> effects = new();

        public Item(ItemType type)
        {
            this.type = type;

            // Create all effects of this item.
            foreach (EffectType effectType in type.effects)
            {
                effects.Add(effectType.Create(this));
            }
        }

        public ItemType type { get; }

        public string displayName => type.displayName;

        public string definiteName => EnglishHelpers.GetDefiniteNounForm(displayName);
        public string indefiniteName => EnglishHelpers.GetIndefiniteNounForm(displayName);

        public IEnumerable<object> rules => effects;

        public T GetEffect<T>() where T : Effect
        {
            return effects.Find(effect => effect is T) as T;
        }

        public IEnumerable<T> GetEffects<T>() where T : Effect
        {
            return effects.OfType<T>();
        }
    }
}
