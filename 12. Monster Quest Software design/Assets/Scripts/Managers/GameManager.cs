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
            ClassType fighter = Database.GetClassType("fighter");

            string[] characterNames = { "Elana", "Jazlyn", "Theron", "Dayana", "Rolando" };
            List<Character> characters = new();

            for (int i = 0; i < 5; i++)
            {
                characters.Add(new Character(characterNames[i], characterBodySprites[i], SizeCategory.Medium, weaponTypes[Random.Range(0, weaponTypes.Length)], studdedLeather, fighter));
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
                yield return new WaitForSeconds(1);

                yield return _combatManager.Simulate(_state);

                if (_state.party.aliveCount == 0) break;
                
                yield return new WaitForSeconds(1);

                foreach (Character character in _state.party.aliveCharacters)
                {
                    yield return character.TakeShortRest();
                }

                yield return new WaitForSeconds(1);
            }

            if (_state.party.aliveCount > 1)
            {
                Console.WriteLine($"After {monsterTypes.Length} grueling battles, the heroes {StringHelper.JoinWithAnd(_state.party.aliveCharacters.Select(character => character.displayName))} return from the dungeons to live another day.");
            }
            else if (_state.party.aliveCount == 1)
            {
                Console.WriteLine($"After {monsterTypes.Length} grueling battles, {_state.party.aliveCharacters.First().displayName} returns from the dungeons. Unfortunately, none of the other party members survived.");
            }
            
            SaveGameHelper.Delete();
        }
    }
}
