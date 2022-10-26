using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class GameState : IRulesHandler
    {
        public GameState(Party party, IEnumerable<MonsterType> monsterTypes)
        {
            this.party = party;

            remainingMonsterTypes = new List<MonsterType>(monsterTypes);
        }

        [field: SerializeField] public Party party { get; private set; }
        [field: SerializeReference] public Combat combat { get; private set; }
        [field: SerializeField] public List<MonsterType> remainingMonsterTypes { get; private set; }

        public IEnumerable<object> rules => party.rules.Concat(combat.rules);

        public void EnterCombatWithMonster(Monster monster)
        {
            combat = new Combat(this, monster);
        }

        public void ExitCombat()
        {
            combat = null;
        }

        public IEnumerator CallRules<TRule>(Func<TRule, IEnumerator> callback) where TRule : class
        {
            foreach (object rule in rules)
            {
                if (rule is not TRule typedRule) continue;

                IEnumerator result = callback(typedRule);

                if (result is not null)
                {
                    yield return result;
                }
            }
        }

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
