using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    [Serializable]
    public class Combat
    {
        private readonly List<Creature> _creaturesInOrderOfInitiative;
        private int _currentTurnCreatureIndex;
        private List<Character> _participatingCharacters;

        public Combat(GameState gameState, Monster monster)
        {
            this.monster = monster;
            
            // All conscious characters starting combat count as participating.
            _participatingCharacters = new List<Character>(gameState.party.characters.Where(character => character.lifeStatus == LifeStatus.Conscious));
            
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

        public void AddParticipatingCharacter(Character character)
        {
            if (_participatingCharacters.Contains(character)) return;
            
            _participatingCharacters.Add(character);
        }

        public IEnumerator End()
        {
            if (monster.isAlive) yield break;
            
            // Distribute experience points.
            int experiencePointsPerCharacter = monster.type.experiencePoints / _participatingCharacters.Count;

            foreach (Character character in _participatingCharacters)
            {
                yield return character.GainExperiencePoints(experiencePointsPerCharacter);
            }
        }
    }
}
