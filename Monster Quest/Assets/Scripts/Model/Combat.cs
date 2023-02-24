using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class Combat : IRulesHandler
    {
        [SerializeReference] private List<Monster> _monsters;
        [SerializeReference] private List<Creature> _creaturesInOrderOfInitiative;
        [SerializeField] private int _roundNumber;
        [SerializeField] private int _currentTurnCreatureIndex;
        [SerializeReference] private List<object> _globalRules;

        public Combat(GameState gameState, IEnumerable<Monster> monsters)
        {
            this.gameState = gameState;
            _monsters = new List<Monster>(monsters);
            RollInitiative();

            // Create global providers.
            _globalRules = new List<object>
            {
                new AttackAbilityModifier(),
                new CoverBonus(),
                new DamageAmountTypeDamageAmountAmountAlteration()
            };

            // Start the first round.
            _roundNumber = 1;
            _currentTurnCreatureIndex = -1;
        }

        // State properties

        [field: SerializeReference] public GameState gameState { get; private set; }

        // Derived properties

        public IEnumerable<Monster> monsters => _monsters;
        public IEnumerable<Creature> creatures => _monsters.Concat<Creature>(gameState.party.characters);
        public IEnumerable<object> rules => _globalRules.Concat(monsters.SelectMany(monster => monster.rules));

        // Methods

        public Creature StartNextCreatureTurn()
        {
            // Start the turn of the next creature in the order.
            _currentTurnCreatureIndex++;

            if (_currentTurnCreatureIndex == _creaturesInOrderOfInitiative.Count)
            {
                // Start new round.
                _roundNumber++;
                _currentTurnCreatureIndex = 0;
            }

            return _creaturesInOrderOfInitiative[_currentTurnCreatureIndex];
        }

        public int GetDistance(Creature a, Creature b)
        {
            return 5;
        }

        public Dictionary<MonsterType, IEnumerable<Monster>> GetMonsterGroupsByType()
        {
            return monsters.GroupBy(monster => monster.type).ToDictionary(group => group.Key, group => group.AsEnumerable());
        }

        public IEnumerable<IEnumerable<Creature>> GetHostileGroups()
        {
            List<IEnumerable<Creature>> hostileGroups = new();

            if (gameState.party.aliveCount > 0) hostileGroups.Add(gameState.party.aliveCharacters);

            Monster[] aliveMonsters = _monsters.Where(monster => monster.isAlive).ToArray();
            if (aliveMonsters.Length > 0) hostileGroups.Add(aliveMonsters);

            return hostileGroups;
        }

        public bool AreHostile(Creature a, Creature b)
        {
            // Creatures are hostile if one is a character and the other is a monster.
            return (a is Character && b is Monster) || (a is Monster && b is Character);
        }

        private void RollInitiative()
        {
            Dictionary<object, int> initiativesByPerformer = new();

            // Roll initiative for characters.
            foreach (Character character in gameState.party.aliveCharacters)
            {
                initiativesByPerformer[character] = character.MakeAbilityRoll(Ability.Dexterity);
            }

            // Roll initiative for monster types.
            Dictionary<MonsterType, IEnumerable<Monster>> monsterGroupsByType = GetMonsterGroupsByType();

            foreach (KeyValuePair<MonsterType, IEnumerable<Monster>> monsterGroupEntry in monsterGroupsByType)
            {
                // Have the first monster roll initiative for the entire type.
                initiativesByPerformer[monsterGroupEntry.Key] = monsterGroupEntry.Value.First().MakeAbilityRoll(Ability.Dexterity);
            }

            // Add creatures from highest initiative to lowest, breaking ties in the process.
            _creaturesInOrderOfInitiative = new List<Creature>();

            IEnumerable<IGrouping<int, KeyValuePair<object, int>>> initiativeGroups = initiativesByPerformer.GroupBy(entry => entry.Value).OrderByDescending(group => group.Key);

            foreach (IGrouping<int, KeyValuePair<object, int>> initiativeGroup in initiativeGroups)
            {
                List<object> performers = initiativeGroup.Select(performerEntry => performerEntry.Key).ToList();

                // Shuffle the performers for a random order within the same initiative roll.
                performers.Shuffle();

                foreach (object performer in performers)
                {
                    if (performer is MonsterType monsterType)
                    {
                        _creaturesInOrderOfInitiative.AddRange(monsterGroupsByType[monsterType]);
                    }
                    else if (performer is Character character)
                    {
                        _creaturesInOrderOfInitiative.Add(character);
                    }
                }
            }
        }
    }
}
