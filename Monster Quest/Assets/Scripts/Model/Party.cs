using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class Party : IRulesHandler
    {
        public Party(IEnumerable<Character> initialCharacters)
        {
            characters = new List<Character>(initialCharacters);
        }

        // State properties
        [field: SerializeReference] public List<Character> characters { get; private set; }

        // Derived properties
        public IEnumerable<object> rules => characters.SelectMany(character => character.rules);

        public void RemoveDeadCharacters()
        {
            characters.RemoveAll(character => character.lifeStatus == Creature.LifeStatus.Dead);
        }

        public override string ToString()
        {
            return StringHelper.JoinWithAnd(characters.Select(character => character.displayName));
        }
    }
}
