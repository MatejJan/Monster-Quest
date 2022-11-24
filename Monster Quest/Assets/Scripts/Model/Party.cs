using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class Party : IRulesHandler
    {
        [SerializeReference] private List<Character> _characters;

        public Party(IEnumerable<Character> initialCharacters)
        {
            _characters = new List<Character>(initialCharacters);
        }

        // Derived properties
        public IEnumerable<Character> characters => _characters;
        public int count => _characters.Count;
        public IEnumerable<object> rules => characters.SelectMany(character => character.rules);

        public void RemoveDeadCharacters()
        {
            _characters.RemoveAll(character => character.lifeStatus == LifeStatus.Dead);
        }

        public override string ToString()
        {
            return StringHelper.JoinWithAnd(characters.Select(character => character.displayName));
        }
    }
}
