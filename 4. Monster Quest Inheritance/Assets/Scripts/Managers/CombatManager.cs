using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class CombatManager : MonoBehaviour
    {
        public IEnumerator Simulate(GameState gameState)
        {
            var random = new System.Random();

            Monster monster = gameState.combat.monster;

            Console.WriteLine($"Watch out, {monster.displayName} with {monster.hitPoints} HP appears!");

            do
            {
                // Heroes' turn.
                foreach (Character character in gameState.party.characters)
                {
                    yield return character.presenter.Attack();
                    
                    int greatswordDamageAmount = DiceHelper.Roll("2d6");
                    
                    Console.WriteLine($"{character.displayName} hits the {monster.displayName} for {greatswordDamageAmount} damage.");
                    yield return monster.ReactToDamage(greatswordDamageAmount);

                    Console.WriteLine($"The {monster.displayName} has {monster.hitPoints} HP left.");

                    if (monster.hitPoints == 0) break;
                }

                if (monster.hitPoints > 0)
                {
                    // Monster's turn.
                    yield return monster.presenter.Attack();
                    
                    int randomHeroIndex = random.Next(0, gameState.party.characters.Count);
                    Character attackedHero = gameState.party.characters[randomHeroIndex];
                    Console.WriteLine($"The {monster.displayName} attacks {attackedHero.displayName}!");

                    // Do the saving throw.
                    int d20Roll = DiceHelper.Roll("d20");
                    int savingThrow = 3 + d20Roll;

                    if (savingThrow >= monster.savingThrowDC)
                    {
                        Console.WriteLine($"{attackedHero.displayName} rolls a {d20Roll} and is saved from the attack.");
                    }
                    else
                    {
                        Console.WriteLine($"{attackedHero.displayName} rolls a {d20Roll} and fails to be saved. {attackedHero.displayName} is killed.");
                        gameState.party.characters.Remove(attackedHero);
                        yield return attackedHero.ReactToDamage(10);
                    }
                }

            } while (monster.hitPoints > 0 && gameState.party.characters.Count > 0);

            if (monster.hitPoints == 0)
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
