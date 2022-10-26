using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class GameManager : MonoBehaviour
    {
        private CombatManager _combatManager;
        private GameState _state;

        private void Awake()
        {
            Transform combatTransform = transform.Find("Combat");
            _combatManager = combatTransform.GetComponent<CombatManager>();
        }
        
        private void Start()
        {
            NewGame();
            Simulate();
        }
        
        private void NewGame()
        {
            // Create a new party.
            Party party = new(new Character[]
            {
                new("Jazlyn"), 
                new("Theron"), 
                new("Dayana"), 
                new("Rolando")
            });
            
            // Create a new game state.
            _state = new GameState(party);
        }

        private void Simulate()
        {
            Console.Clear();
            Console.WriteLine($"Fighters {_state.party} descend into the dungeon.");

            Monster orc = new("orc", DiceHelper.Roll("2d8+6"), 10);
            _state.EnterCombatWithMonster(orc);
            _combatManager.SimulateCombat(_state);

            if (_state.party.characters.Count > 0)
            {
                Monster azer = new("azer", DiceHelper.Roll("6d8+12"), 18);
                _state.EnterCombatWithMonster(azer);
                _combatManager.SimulateCombat(_state);
            }

            if (_state.party.characters.Count > 0)
            {
                Monster troll = new("troll", DiceHelper.Roll("8d10+40"), 16);
                _state.EnterCombatWithMonster(troll);
                _combatManager.SimulateCombat(_state);
            }

            if (_state.party.characters.Count > 1)
            {
                Console.WriteLine($"After three grueling battles, the heroes {_state.party} return from the dungeons to live another day.");
            }
            else if (_state.party.characters.Count == 1)
            {
                Console.WriteLine($"After three grueling battles, {_state.party.characters[0].displayName} returns from the dungeons. Unfortunately, none of the other party members survived.");
            }
        }
    }
}
