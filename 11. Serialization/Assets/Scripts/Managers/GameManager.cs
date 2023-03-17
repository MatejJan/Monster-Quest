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

            if (SaveGameHelper.saveFileExists)
            {
                _state = SaveGameHelper.Load();
            }
            else
            {
                NewGame();
            }
            
            yield return Simulate();
        }
        
        private void NewGame()
        {
            // Create a new party.
            ArmorType studdedLeather = Database.GetItemType<ArmorType>("studded leather");

            WeaponType[] weaponTypes = Database.itemTypes.Where(itemType => itemType is WeaponType { weight: > 0 }).Cast<WeaponType>().ToArray();

            string[] characterNames = { "Elana", "Jazlyn", "Theron", "Dayana", "Rolando" };
            List<Character> characters = new();

            for (int i = 0; i < 5; i++)
            {
                characters.Add(new(characterNames[i], characterBodySprites[i], 10, SizeCategory.Medium, weaponTypes[Random.Range(0, weaponTypes.Length)], studdedLeather));
            }
            
            Party party = new(characters);
            
            // Create a new game state.
            _state = new GameState(party, monsterTypes);
            
            Console.WriteLine($"Fighters {_state.party} descend into the dungeon.");
        }

        private IEnumerator Simulate()
        {
            yield return _combatPresenter.InitializeParty(_state);
            
            while (true)
            {
                // Start a new combat if we're between rounds.
                if (_state.combat == null || !_state.combat.monster.isAlive)
                {
                    // Make sure we have some monsters left to fight.
                    bool canEnterCombat = _state.EnterCombatWithNextMonster();
                    if (!canEnterCombat) break;
                }
                
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
            
            SaveGameHelper.Delete();
        }
    }
}
