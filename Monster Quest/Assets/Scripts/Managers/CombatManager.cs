using System;
using System.Collections;
using UnityEngine;

namespace MonsterQuest
{
    public class CombatManager : MonoBehaviour
    {
        // Event for end of a combat round.
        public event Action onRoundEnd;

        public IEnumerator Simulate(GameState gameState)
        {
            do
            {
                // Heroes' turn.
                foreach (Character character in gameState.party.characters)
                {
                    IAction action = character.TakeTurn(gameState);

                    yield return action?.Execute();

                    // Stop attacking if the monster died.
                    if (gameState.combat.monster.hitPoints == 0) break;
                }

                // Remove any characters that died while unconscious.
                gameState.party.RemoveDeadCharacters();

                if (gameState.combat.monster.lifeStatus != LifeStatus.Dead && gameState.party.characters.Count > 0)
                {
                    // Monster's turn.
                    IAction action = gameState.combat.monster.TakeTurn(gameState);

                    yield return action?.Execute();

                    // Remove the characters that died from the attack.
                    gameState.party.RemoveDeadCharacters();
                }

                onRoundEnd?.Invoke();
            } while (gameState.combat.monster.hitPoints > 0 && gameState.party.characters.Count > 0);

            if (gameState.combat.monster.hitPoints == 0)
            {
                Console.WriteLine("The heroes celebrate their victory!");
            }
            else
            {
                Console.WriteLine($"The party has failed and {gameState.combat.monster.definiteName} continues to attack unsuspecting adventurers.");
            }
        }
    }
}
