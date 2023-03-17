using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    [Serializable]
    public class Combat
    {
        private readonly List<Creature> _creaturesInOrderOfInitiative;
        private int _currentTurnCreatureIndex;

        public Combat(GameState gameState, Monster monster)
        {
            this.monster = monster;
            
            List<Creature> creatures = new(gameState.party.characters) { monster };

            _creaturesInOrderOfInitiative = creatures.OrderBy(creature => creature.MakeAbilityRoll(Ability.Dexterity)).ToList();

            _currentTurnCreatureIndex = -1;
        }

        public Monster monster { get; }
        
        public Creature StartNextCreatureTurn()
        {
            // Start the turn of the next creature in the order.
            _currentTurnCreatureIndex++;

            if (_currentTurnCreatureIndex == _creaturesInOrderOfInitiative.Count)
            {
                // Start new round.
                _currentTurnCreatureIndex = 0;
            }

            return _creaturesInOrderOfInitiative[_currentTurnCreatureIndex];
        }
    }
}
