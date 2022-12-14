using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class Combat : IRulesHandler
    {
        [SerializeReference] private List<object> _globalRules;

        public Combat(GameState gameState, Monster monster)
        {
            this.gameState = gameState;
            this.monster = monster;

            // Create global providers.
            _globalRules = new List<object> { new AttackAbilityModifier(), new CoverBonus(), new DamageAmountTypeDamageAmountAmountAlteration() };
        }

        // State properties
        [field: SerializeReference] public GameState gameState { get; private set; }
        [field: SerializeReference] public Monster monster { get; private set; }

        // Derived properties
        public IEnumerable<object> rules => _globalRules.Concat(monster.rules);

        public int GetDistance(Creature a, Creature b)
        {
            return 5;
        }

        public IEnumerable<Creature> GetCreatures()
        {
            List<Creature> creatures = new();

            creatures.AddRange(gameState.party.characters);
            creatures.Add(monster);

            return creatures;
        }

        public bool AreHostile(Creature a, Creature b)
        {
            // Creatures are hostile if one is a character and the other is a monster.
            return (a is Character && b is Monster) || (a is Monster && b is Character);
        }
    }
}
