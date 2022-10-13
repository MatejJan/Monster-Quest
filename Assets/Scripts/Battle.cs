using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Actions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonsterQuest
{
    [Serializable]
    public class Battle : IRulesHandler
    {
        [SerializeReference] private List<object> _globalRules;

        public Battle(Monster monster)
        {
            this.monster = monster;

            // Create global providers.
            _globalRules = new List<object>
            {
                new AttackAbilityModifier(),
                new CoverBonus(),
                new DamageAmountTypeDamageAmountAmountAlteration()
            };
        }

        // State properties
        [field: SerializeReference] public Monster monster { get; private set; }

        // Derived properties
        public IEnumerable<object> rules => _globalRules.Concat(monster.rules);

        public IEnumerator Simulate()
        {
            do
            {
                // Heroes' turn.
                foreach (Character character in Game.state.party.characters)
                {
                    IAction action = character.TakeTurn(this, monster);
                    yield return action?.Execute();

                    // Stop attacking if the monster died.
                    if (monster.hitPoints == 0) break;
                }

                // Remove any characters that died while unconscious.
                Game.state.party.RemoveDeadCharacters();

                if (monster.lifeStatus != Creature.LifeStatus.Dead && Game.state.party.characters.Count > 0)
                {
                    // Monster's turn.
                    int randomHeroIndex = Random.Range(0, Game.state.party.characters.Count);
                    Character attackedHero = Game.state.party.characters[randomHeroIndex];

                    IAction action = monster.TakeTurn(this, attackedHero);
                    yield return action?.Execute();

                    // Remove the characters that died from the attack.
                    Game.state.party.RemoveDeadCharacters();
                }

                // Save the game between turns.
                Game.SaveGame();
            } while (monster.hitPoints > 0 && Game.state.party.characters.Count > 0);

            if (monster.hitPoints == 0)
            {
                Console.WriteLine("The heroes celebrate their victory!");
            }
            else
            {
                Console.WriteLine($"The party has failed and {monster.definiteName} continues to attack unsuspecting adventurers.");
            }
        }

        public int GetDistance(Creature a, Creature b)
        {
            return 5;
        }

        public IEnumerable<Creature> GetCreatures()
        {
            List<Creature> creatures = new();

            creatures.AddRange(Game.state.party.characters);
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
