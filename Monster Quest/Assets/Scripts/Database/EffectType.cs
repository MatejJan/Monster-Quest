using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public abstract class EffectType : ScriptableObject
    {
        public abstract Effect Create(object parent);

        public RuleDescription[] GetOwnRuleDescriptions(object context = null)
        {
            List<ArrayValue<RuleDescription>> values = new();

            if (this is IRuleDescriptionsProvider ruleDescriptionsProvider)
            {
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
