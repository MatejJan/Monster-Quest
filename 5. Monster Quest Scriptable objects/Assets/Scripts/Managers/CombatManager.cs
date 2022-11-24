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
                    
                    int damageAmount = DiceHelper.Roll(character.weaponType.damageRoll);
                    
                    Console.WriteLine($"{character.displayName} hits the {monster.displayName} with {character.weaponType.displayName} for {damageAmount} damage.");
                    yield return monster.ReactToDamage(damageAmount);

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

                    WeaponType selectedWeaponType = monster.type.weaponTypes[random.Next(monster.type.weaponTypes.Length)];
                    int damageAmount = DiceHelper.Roll(selectedWeaponType.damageRoll);
                    
                    Console.WriteLine($"The {monster.displayName} hits {attackedHero.displayName} with {selectedWeaponType.displayName} for {damageAmount} damage.");
                    yield return attackedHero.ReactToDamage(damageAmount);
                    
                    Console.WriteLine($"{attackedHero.displayName} has {attackedHero.hitPoints} HP left.");

                    if (attackedHero.hitPoints == 0)
                    {
                        Console.WriteLine($"{attackedHero.displayName} meets an untimely end.");
                        gameState.party.characters.Remove(attackedHero);
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
