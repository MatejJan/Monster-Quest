using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class Party : IRulesHandler, IStateEventProvider
    {
        [SerializeReference] private List<Character> _characters;

        public Party(IEnumerable<Character> initialCharacters)
        {
            _characters = new List<Character>(initialCharacters);
        }

        // Derived properties
        public IEnumerable<Character> characters => _characters;
        public IEnumerable<Character> aliveCharacters => _characters.Where(character => character.isAlive);
        public int aliveCount => _characters.Count(character => character.isAlive);
        public IEnumerable<object> rules => characters.SelectMany(character => character.rules);

        // Events 

        [field: NonSerialized] public event Action<string> stateEvent;

        // Methods

        public void StartProvidingStateEvents()
        {
            foreach (Character character in _characters)
            {
                character.stateEvent += ReportStateEvent;
            }
        }

        private void ReportStateEvent(string message)
        {
            stateEvent?.Invoke(message);
        }

        public IEnumerator TakeShortRest()
        {
            foreach (Character character in aliveCharacters)
            {
                yield return character.TakeShortRest();
            }
        }

        public override string ToString()
        {
            return EnglishHelper.JoinWithAnd(characters.Select(character => character.displayName));
        }
    }
}
