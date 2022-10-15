using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonsterQuest
{
    public class Game : MonoBehaviour
    {
        private void Start()
        {
            var characterNames = new List<string> { "Jazlyn", "Theron", "Dayana", "Rolando" };

            Console.Clear();
            Console.WriteLine($"Fighters {String.Join(", ", characterNames)} descend into the dungeon.");

            SimulateCombat(characterNames, "orc", DiceRoll(2, 8, 6), 10);
            if (characterNames.Count > 0) SimulateCombat(characterNames, "azer", DiceRoll(6, 8, 12), 18);
            if (characterNames.Count > 0) SimulateCombat(characterNames, "troll", DiceRoll(8, 10, 40), 16);

            if (characterNames.Count > 1)
            {
                Console.WriteLine($"After three grueling battles, the heroes {String.Join(", ", characterNames)} return from the dungeons to live another day.");
            }
            else if (characterNames.Count == 1)
            {
                Console.WriteLine($"After three grueling battles, {characterNames[0]} returns from the dungeons. Unfortunately, none of the other party members survived.");
            }
        }

        static int DiceRoll(int numberOfRolls, int diceSides, int fixedBonus = 0)
        {
            var random = new System.Random();

            int result = fixedBonus;

            for (var i = 0; i < numberOfRolls; i++)
            {
                result += random.Next(1, diceSides + 1);
            }

            return result;
        }

        static void SimulateCombat(List<string> characterNames, string monsterName, int monsterHP, int savingThrowDC)
        {
            var random = new System.Random();

            Console.WriteLine($"Watch out, {monsterName} with {monsterHP} HP appears!");

            do
            {
                // Heroes' turn.
                foreach (string hero in characterNames)
                {
                    var greatswordHit = DiceRoll(2, 6);
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
                    int d20roll = DiceRoll(1, 20);
                    int savingThrow = 3 + d20roll;

                    if (savingThrow >= savingThrowDC)
                    {
                        Console.WriteLine($"{attackedHero} rolls a {d20roll} and is saved from the attack.");
                    }
                    else
                    {
                        Console.WriteLine($"{attackedHero} rolls a {d20roll} and fails to be saved. {attackedHero} is killed.");
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
