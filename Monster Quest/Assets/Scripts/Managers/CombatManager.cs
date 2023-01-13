using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    public class CombatManager : MonoBehaviour
    {
        // Event for end of a combat round.
        public event Action onRoundEnd;

        public IEnumerator Simulate(GameState gameState)
        {
            bool hostileGroupsArePresent;

            void UpdateIfHostileGroupsArePresent()
            {
                hostileGroupsArePresent = gameState.combat.GetHostileGroups().Count() > 1;
            }

            UpdateIfHostileGroupsArePresent();

            // Keep simulating while we have groups that are hostile to each other.
            while (hostileGroupsArePresent)
            {
                // Simulate a round of combat.
                foreach (Creature creature in gameState.combat.creaturesInOrderOfInitiative)
                {
                    if (creature.lifeStatus == LifeStatus.Dead) continue;

                    IAction action = creature.TakeTurn(gameState);

                    yield return action?.Execute();

                    UpdateIfHostileGroupsArePresent();

                    if (!hostileGroupsArePresent) break;
                }

                onRoundEnd?.Invoke();
            }

            if (gameState.party.aliveCount > 0)
            {
                Console.WriteLine("The heroes celebrate their victory!");
            }
            else
            {
                Console.WriteLine("The party has failed and the monsters continue to attack unsuspecting adventurers.");
            }
        }
    }
}
