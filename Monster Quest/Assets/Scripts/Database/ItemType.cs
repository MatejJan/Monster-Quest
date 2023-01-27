using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Item")]
    public class ItemType : ScriptableObject
    {
        public string displayName;
        public CoinValue cost;
        public float weight;

        public EffectType[] effects;

        public string definiteName => EnglishHelper.GetDefiniteNounForm(displayName);
        public string indefiniteName => EnglishHelper.GetIndefiniteNounForm(displayName);

        public Item Create()
        {
            return new Item(this);
        }

        public T GetEffect<T>() where T : EffectType
        {
            return GetEffects<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetEffects<T>() where T : EffectType
        {
            return effects.OfType<T>();
        }

        public bool HasEffect<T>()
        {
            return effects.Any(effect => effect is T);
        }

        public RuleDescription[] GetOwnRuleDescriptions(object context = null)
        {
            List<ArrayValue<RuleDescription>> values = new();

            foreach (EffectType effect in effects)
            {
                if (effect is not IRuleDescriptionsProvider ruleDescriptionsProvider) continue;

                ArrayValue<RuleDescription> value = ruleDescriptionsProvider.GetRuleDescriptions(context);

                if (value is not null)
                {
                    values.Add(value);
                }
            }

            return values.Resolve();
        }
    }
}
