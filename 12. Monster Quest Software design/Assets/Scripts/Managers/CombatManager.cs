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

            do
            {
                SaveGameHelper.Save(gameState);
                Creature creature = gameState.combat.StartNextCreatureTurn();
                if (creature.lifeStatus == LifeStatus.Dead) continue;

                IAction action = creature.TakeTurn(gameState);
                yield return action.Execute();
            } while (monster.isAlive && party.aliveCount > 0);

            if (!monster.isAlive)
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
