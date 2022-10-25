using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class GameManager : MonoBehaviour
    {
        private CombatManager _combatManager;

        private void Awake()
        {
            Transform combatTransform = transform.Find("Combat");
            _combatManager = combatTransform.GetComponent<CombatManager>();
        }
        
        private void Start()
        {
            var characterNames = new List<string> { "Jazlyn", "Theron", "Dayana", "Rolando" };

            Console.Clear();
            Console.WriteLine($"Fighters {StringHelper.JoinWithAnd(characterNames)} descend into the dungeon.");

            _combatManager.SimulateCombat(characterNames, "orc", DiceHelper.Roll("2d8+6"), 10);
            if (characterNames.Count > 0) _combatManager.SimulateCombat(characterNames, "azer", DiceHelper.Roll("6d8+12"), 18);
            if (characterNames.Count > 0) _combatManager.SimulateCombat(characterNames, "troll", DiceHelper.Roll("8d10+40"), 16);

            if (characterNames.Count > 1)
            {
                Console.WriteLine($"After three grueling battles, the heroes {StringHelper.JoinWithAnd(characterNames)} return from the dungeons to live another day.");
            }
            else if (characterNames.Count == 1)
            {
                Console.WriteLine($"After three grueling battles, {characterNames[0]} returns from the dungeons. Unfortunately, none of the other party members survived.");
            }
        }
    }
}
