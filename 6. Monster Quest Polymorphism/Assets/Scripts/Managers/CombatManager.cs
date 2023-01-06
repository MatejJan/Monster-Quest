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
            var random = new System.Random();

            Monster monster = gameState.combat.monster;

            Console.WriteLine($"Watch out, {monster.displayName} with {monster.hitPoints} HP appears!");

            do
            {
                // Heroes' turn.
                foreach (Character character in gameState.party.characters)
                {
                    if (character.lifeStatus != LifeStatus.Alive) continue;
                    
                    yield return character.presenter.Attack();
                    
                    int damageAmount = DiceHelper.Roll(character.weaponType.damageRoll);
                    
                    Console.WriteLine($"{character.displayName} hits the {monster.displayName} with {character.weaponType.displayName} for {damageAmount} damage.");
                    yield return monster.ReactToDamage(damageAmount);

                    Console.WriteLine($"The {monster.displayName} has {monster.hitPoints} HP left.");

                    if (monster.hitPoints == 0) break;
                }

                if (monster.lifeStatus == LifeStatus.Alive)
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
                    
                    if (attackedHero.lifeStatus == LifeStatus.Dead)
                    {
                        gameState.party.characters.Remove(attackedHero);
                    }
                }
                
            } while (monster.lifeStatus == LifeStatus.Alive && gameState.party.characters.Count > 0);

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
