using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    public class CombatManager : MonoBehaviour, IStateEventProvider
    {
        // Events 

        public event Action<string> stateEvent;

        // Methods 

        private void ReportStateEvent(string message)
        {
            stateEvent?.Invoke(message);
        }

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
                // Simulate a combat turn.
                Creature creature = gameState.combat.StartNextCreatureTurn();

                if (creature.lifeStatus == LifeStatus.Dead) continue;

                // In case a character became conscious after the start of the combat, mark them as participating.
                if (creature.lifeStatus == LifeStatus.Conscious && creature is Character character)
                {
                    gameState.combat.AddParticipatingCharacter(character);
                }

                IAction action = creature.TakeTurn(gameState);

                if (action is IStateEventProvider stateEventProvider)
                {
                    stateEventProvider.stateEvent += ReportStateEvent;
                    stateEventProvider.StartProvidingStateEvents();
                }

                yield return action?.Execute();

                UpdateIfHostileGroupsArePresent();

                if (!hostileGroupsArePresent) break;

                SaveGameHelper.Save(gameState);
            }

            if (gameState.party.aliveCount > 0)
            {
                ReportStateEvent("The heroes celebrate their victory!");
            }
            else
            {
                ReportStateEvent("The party has failed and the monsters continue to attack unsuspecting adventurers.");
            }

            yield return gameState.combat.End();
        }
    }
}
