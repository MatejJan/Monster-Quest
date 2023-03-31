using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class GameState : IRulesHandler, IStateEventProvider
    {
        private bool _callingRules;
        private List<Action> _rulesMutationActions;

        public GameState(Party party)
        {
            this.party = party;
        }

        // State properties

        [field: SerializeReference] public Party party { get; private set; }
        [field: SerializeReference] public Combat combat { get; private set; }
        [field: SerializeField] public int combatsFoughtCount { get; private set; }

        // Derived properties

        public IEnumerable<object> rules => party.rules.Concat(combat.rules);

        // Events 

        [field: NonSerialized] public event Action<object> stateEvent;

        // Methods 

        public void StartProvidingStateEvents()
        {
            party.stateEvent += ReportStateEvent;
            party.StartProvidingStateEvents();

            if (combat is not null)
            {
                StartProvidingCombatStateEvents();
            }
        }

        private void StartProvidingCombatStateEvents()
        {
            combat.stateEvent += ReportStateEvent;
            combat.StartProvidingStateEvents();
        }

        private void ReportStateEvent(object eventData)
        {
            stateEvent?.Invoke(eventData);
        }

        public void EnterCombatWithMonsters(IEnumerable<Monster> monsters)
        {
            combat = new Combat(this, monsters);
            StartProvidingCombatStateEvents();

            combatsFoughtCount++;
        }

        public void ExitCombat()
        {
            combat = null;
        }

        public void CallRules<TRule>(Action<TRule> callback) where TRule : class
        {
            _callingRules = true;
            _rulesMutationActions ??= new List<Action>();

            // Give all rules a chance to react.
            foreach (object rule in rules)
            {
                if (rule is not TRule typedRule) continue;

                callback(typedRule);
            }

            // Apply any actions that mutate rules.
            foreach (Action action in _rulesMutationActions)
            {
                action();
            }

            _rulesMutationActions.Clear();
            _callingRules = false;
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

        public void MutateRules(Action action)
        {
            // If we're in the middle of calling rules, postpone execution of the action.
            if (_callingRules)
            {
                _rulesMutationActions.Add(action);
            }
            else
            {
                action();
            }
        }
    }
}
