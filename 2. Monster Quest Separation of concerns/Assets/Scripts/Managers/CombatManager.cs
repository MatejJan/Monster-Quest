using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class CombatManager : MonoBehaviour
    {
        public void SimulateCombat(List<string> characterNames, string monsterName, int monsterHP, int savingThrowDC)
        {
            var random = new System.Random();

            Console.WriteLine($"Watch out, {monsterName} with {monsterHP} HP appears!");

            do
            {
                // Heroes' turn.
                foreach (string hero in characterNames)
                {
                    var greatswordHit = DiceHelper.Roll("2d6");
                    monsterHP -= greatswordHit;
                    if (monsterHP < 0) monsterHP = 0;

                    Console.WriteLine($"{hero} hits the {monsterName} for {greatswordHit} damage. The {monsterName} has {monsterHP} HP left.");

                    if (monsterHP == 0) break;
                }

                if (monsterHP > 0)
                {
                    // Monster's turn.
                    int randomHeroIndex = random.Next(0, characterNames.Count);
                    string attackedHero = characterNames[randomHeroIndex];
                    Console.WriteLine($"The {monsterName} attacks {attackedHero}!");

                    // Do the saving throw.
                    int d20Roll = DiceHelper.Roll("d20");
                    int savingThrow = 3 + d20Roll;

                    if (savingThrow >= savingThrowDC)
                    {
                        Console.WriteLine($"{attackedHero} rolls a {d20Roll} and is saved from the attack.");
                    }
                    else
                    {
                        Console.WriteLine($"{attackedHero} rolls a {d20Roll} and fails to be saved. {attackedHero} is killed.");
                        characterNames.Remove(attackedHero);
                    }
                }

            } while (monsterHP > 0 && characterNames.Count > 0);

            if (monsterHP == 0)
            {
                Console.WriteLine($"The {monsterName} collapses and the heroes celebrate their victory!");
            }
            else
            {
                Console.WriteLine($"The party has failed and the {monsterName} continues to attack unsuspecting adventurers.");
            }
        }
    }
}
