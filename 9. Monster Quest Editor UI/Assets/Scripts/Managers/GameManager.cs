using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
            ArmorType studdedLeather = Database.GetItemType<ArmorType>("studded leather");

            WeaponType[] weaponTypes = Database.itemTypes.Where(itemType => itemType is WeaponType { weight: > 0 }).Cast<WeaponType>().ToArray();

            string[] characterNames = { "Jazlyn", "Theron", "Dayana", "Rolando" };
            List<Character> characters = new();

            for (int i = 0; i < 4; i++)
            {
                characters.Add(new(characterNames[i], characterBodySprites[i], 10, SizeCategory.Medium, weaponTypes[Random.Range(0, weaponTypes.Length)], studdedLeather));
            }
            
            Party party = new(characters);
            
            // Create a new game state.
            _state = new GameState(party);
        }

        private IEnumerator Simulate()
        {
            Console.Clear();
            Console.WriteLine($"Fighters {_state.party} descend into the dungeon.");
            
            yield return _combatPresenter.InitializeParty(_state);
            
            // Fight all the monsters.
            foreach (MonsterType monsterType in monsterTypes)
            {
                // Create a new monster.
                Monster monster = new(monsterType);
                
                _state.EnterCombatWithMonster(monster);
                yield return _combatPresenter.InitializeMonster(_state);
                yield return _combatManager.Simulate(_state);

                if (_state.party.aliveCount == 0) break;
            }

            if (_state.party.aliveCount > 1)
            {
                Console.WriteLine($"After {monsterTypes.Length} grueling battles, the heroes {_state.party} return from the dungeons to live another day.");
            }
            else if (_state.party.aliveCount == 1)
            {
                Console.WriteLine($"After {monsterTypes.Length} grueling battles, {_state.party.characters[0].displayName} returns from the dungeons. Unfortunately, none of the other party members survived.");
            }
        }
    }
}
