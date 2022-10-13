using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class GameState : IRulesHandler
    {
        public GameState()
        {
            remainingMonsterTypes = new List<MonsterType>();
        }

        [field: SerializeField] public Party party { get; set; }
        [field: SerializeReference] public Battle battle { get; set; }
        [field: SerializeField] public List<MonsterType> remainingMonsterTypes { get; private set; }

        public IEnumerable<object> rules => party.rules.Concat(battle.rules);
    }
}
