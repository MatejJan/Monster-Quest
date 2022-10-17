using System.Collections;
using UnityEngine;

namespace MonsterQuest
{
    public class CombatManager : MonoBehaviour
    {
        public IEnumerator Simulate(GameState gameState)
        {
            do
            {
                // Heroes' turn.
                foreach (Character character in GameManager.state.party.characters)
                {
                    IAction action = character.TakeTurn(gameState);

                    yield return action?.Execute();

                    // Stop attacking if the monster died.
                    if (gameState.combat.monster.hitPoints == 0) break;
                }

                // Remove any characters that died while unconscious.
                GameManager.state.party.RemoveDeadCharacters();

                if (gameState.combat.monster.lifeStatus != Creature.LifeStatus.Dead && GameManager.state.party.characters.Count > 0)
                {
                    // Monster's turn.
                    IAction action = gameState.combat.monster.TakeTurn(gameState);

                    yield return action?.Execute();

                    // Remove the characters that died from the attack.
                    GameManager.state.party.RemoveDeadCharacters();
                }

                // Save the game between turns.
                GameManager.SaveGame();
            } while (gameState.combat.monster.hitPoints > 0 && GameManager.state.party.characters.Count > 0);

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