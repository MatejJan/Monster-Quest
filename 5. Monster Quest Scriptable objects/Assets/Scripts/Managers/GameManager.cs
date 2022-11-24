using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] characterBodySprites;
        [SerializeField] private MonsterType[] monsterTypes;
        
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
            yield return Database.Initialize();
            
            NewGame();
            
            yield return Simulate();
        }
        
        private void NewGame()
        {
            // Create a new party.
            WeaponType greatsword = (WeaponType)Database.GetItemType("greatsword");
            ArmorType studdedLeather = (ArmorType)Database.GetItemType("studded leather");
            
            Party party = new(new Character[]
            {
                new("Jazlyn", characterBodySprites[0], 10, SizeCategory.Medium, greatsword, studdedLeather), 
                new("Theron", characterBodySprites[1], 10, SizeCategory.Medium, greatsword, studdedLeather), 
                new("Dayana", characterBodySprites[2], 10, SizeCategory.Medium, greatsword, studdedLeather), 
                new("Rolando", characterBodySprites[3], 10, SizeCategory.Medium, greatsword, studdedLeather)
            });
            
            // Create a new game state.
            _state = new GameState(party);
        }

        private IEnumerator Simulate()
        {
            Console.Clear();
            Console.WriteLine($"Fighters {_state.party} descend into the dungeon.");
            
            _combatPresenter.InitializeParty(_state);
            
            // Fight all the monsters.
            foreach (MonsterType monsterType in monsterTypes)
            {
                // Create a new monster.
                Monster monster = new(monsterType);
                
                _state.EnterCombatWithMonster(monster);
                _combatPresenter.InitializeMonster(_state);
                yield return _combatManager.Simulate(_state);

                if (_state.party.characters.Count == 0) break;
            }

            if (_state.party.characters.Count > 1)
            {
                Console.WriteLine($"After {monsterTypes.Length} grueling battles, the heroes {_state.party} return from the dungeons to live another day.");
            }
            else if (_state.party.characters.Count == 1)
            {
                Console.WriteLine($"After {monsterTypes.Length} grueling battles, {_state.party.characters[0].displayName} returns from the dungeons. Unfortunately, none of the other party members survived.");
            }
        }
    }
}
