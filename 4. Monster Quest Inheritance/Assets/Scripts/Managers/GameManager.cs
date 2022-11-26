using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] characterBodySprites;
        [SerializeField] private Sprite[] monsterBodySprites;
        
        private CombatManager _combatManager;
        private CombatPresenter _combatPresenter;
        private GameState _state;

        private void Awake()
        {
            Transform combatTransform = transform.Find("Combat");
            _combatManager = combatTransform.GetComponent<CombatManager>();
            _combatPresenter = combatTransform.GetComponent<CombatPresenter>();
        }
        
        private IEnumerator Start()
        {
            NewGame();
            
            yield return Simulate();
        }
        
        private void NewGame()
        {
            // Create a new party.
            Party party = new(new Character[]
            {
                new("Jazlyn", characterBodySprites[0], 10, SizeCategory.Medium), 
                new("Theron", characterBodySprites[1], 10, SizeCategory.Medium), 
                new("Dayana", characterBodySprites[2], 10, SizeCategory.Medium), 
                new("Rolando", characterBodySprites[3], 10, SizeCategory.Medium)
            });
            
            // Create a new game state.
            _state = new GameState(party);
        }

        private IEnumerator Simulate()
        {
            Console.Clear();
            Console.WriteLine($"Fighters {_state.party} descend into the dungeon.");
            
            _combatPresenter.InitializeParty(_state);

            Monster orc = new("orc", monsterBodySprites[0], DiceHelper.Roll("2d8+6"), SizeCategory.Medium, 10);
            _state.EnterCombatWithMonster(orc);
            _combatPresenter.InitializeMonster(_state);
            yield return _combatManager.Simulate(_state);

            if (_state.party.characters.Count > 0)
            {
                Monster azer = new("azer", monsterBodySprites[1], DiceHelper.Roll("6d8+12"), SizeCategory.Medium, 18);
                _state.EnterCombatWithMonster(azer);
                _combatPresenter.InitializeMonster(_state);
                yield return _combatManager.Simulate(_state);
            }

            if (_state.party.characters.Count > 0)
            {
                Monster troll = new("troll", monsterBodySprites[2], DiceHelper.Roll("8d10+40"), SizeCategory.Large, 16);
                _state.EnterCombatWithMonster(troll);
                _combatPresenter.InitializeMonster(_state);
                yield return _combatManager.Simulate(_state);
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
