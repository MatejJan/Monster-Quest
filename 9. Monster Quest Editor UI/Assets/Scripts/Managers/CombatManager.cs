using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    public class CombatManager : MonoBehaviour
    {
        public IEnumerator Simulate(GameState gameState)
        {
            Monster monster = gameState.combat.monster;
            Party party = gameState.party;

            Console.WriteLine($"Watch out, {monster.displayName} with {monster.hitPoints} HP appears!");

            List<Creature> creaturesInOrderOfInitiative = new(party.characters) { monster };

            creaturesInOrderOfInitiative.Shuffle();

            bool combatResolved = false;
            
            do
            {
                foreach (Creature creature in creaturesInOrderOfInitiative)
                {
                    if (creature.lifeStatus == LifeStatus.Dead) continue;

                    IAction action = creature.TakeTurn(gameState);
                    yield return action.Execute();

                    combatResolved = monster.lifeStatus == LifeStatus.Dead || party.aliveCount == 0;
                    if (combatResolved) break;
                }
                
            } while (!combatResolved);

            if (monster.lifeStatus == LifeStatus.Dead)
            {
                Console.WriteLine($"The {monster.displayName} collapses and the heroes celebrate their victory!");
            }
            else
            {
                Console.WriteLine($"The party has failed and the {monster.displayName} continues to attack unsuspecting adventurers.");
            }
        }
    }
}
