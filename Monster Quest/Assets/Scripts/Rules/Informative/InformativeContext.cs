using System;
using System.Collections.Generic;

namespace MonsterQuest
{
    public abstract class InformativeContext : IRulesHandler
    {
        public abstract IEnumerable<object> rules { get; }

        public IEnumerable<TValue> GetRuleValues<TRule, TValue>(Func<TRule, TValue> callback) where TRule : class
        {
            List<TValue> values = new();

            foreach (object rule in rules)
            {
                if (rule is not TRule t) continue;

                TValue value = callback(t);

                if (value is not null)
                {
                    values.Add(value);
                }
            }

            return values;
        }
    }
}
