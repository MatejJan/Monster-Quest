using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Effects;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
#endif

namespace MonsterQuest
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameStateAsset loadGameState;
        [SerializeField] private Sprite[] characterBodySprites;
        [SerializeField] private int charactersStartingLevel;

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
            // Wait for the Manual to be initialized.
            yield return Database.Initialize();

            // Save game in between rounds.
            _combatManager.onTurnEnd += () => SaveGameHelper.Save(_state);

            // Load an existing or start a new game.
            if (loadGameState)
            {
                _state = loadGameState.gameState;
            }
            else if (SaveGameHelper.saveFileExists)
            {
                _state = SaveGameHelper.Load();
            }
            else
            {
                NewGame();
            }

            // Start the simulation.
            yield return Simulate();
        }

        private void NewGame()
        {
            // Create a new party.
            RaceType humanRaceType = Database.GetRaceType("human");
            ClassType fighterClassType = Database.GetClassType("fighter");

            Character[] characters =
            {
                new("Elana", humanRaceType, fighterClassType, characterBodySprites[0], charactersStartingLevel),
                new("Jazlyn", humanRaceType, fighterClassType, characterBodySprites[1], charactersStartingLevel),
                new("Theron", humanRaceType, fighterClassType, characterBodySprites[2], charactersStartingLevel),
                new("Dayana", humanRaceType, fighterClassType, characterBodySprites[3], charactersStartingLevel),
                new("Rolando", humanRaceType, fighterClassType, characterBodySprites[4], charactersStartingLevel)
            };

            Party party = new(characters);

            // Create a new game state.
            _state = new GameState(party);

            // Give characters equipment.
            ItemType[] weaponItemTypes =
            {
                Database.GetItemType("sling"),
                Database.GetItemType("greatsword"),
                Database.GetItemType("javelin"),
                Database.GetItemType("greatsword"),
                Database.GetItemType("greataxe")
            };

            ItemType chainShirt = Database.GetItemType("chain shirt");

            ItemType potionOfHealing = Database.GetItemType("potion of healing");

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].GiveItem(_state, weaponItemTypes[i].Create());
                characters[i].GiveItem(_state, chainShirt.Create());

                for (int j = 0; j < 3; j++)
                {
                    characters[i].GiveItem(_state, potionOfHealing.Create());
                }
            }

            // Output intro.
            Console.WriteLine($"Warriors {_state.party} descend into the dungeon.");
        }

        private IEnumerator Simulate()
        {
            // Present the characters.
            yield return _combatPresenter.InitializeParty(_state);

            while (true)
            {
                // See if we need to enter a new combat (it might already exist from a save file).
                if (_state.combat is null)
                {
                    // Wait a bit before starting new combat.
                    yield return new WaitForSeconds(1);

                    // Enter combat with a new set of monsters.
                    List<Monster> monsters = CreateMonsters();
                    _state.EnterCombatWithMonsters(monsters);

                    if (monsters.Count == 1)
                    {
                        Console.WriteLine($"Watch out, {monsters[0].indefiniteName} with {monsters[0].hitPoints} HP appears!");
                    }
                    else
                    {
                        Dictionary<MonsterType, IEnumerable<Monster>> monsterGroupsByType = _state.combat.GetMonsterGroupsByType();
                        List<string> monsterGroupDescriptions = new();
                        int totalHitPoints = 0;

                        foreach (KeyValuePair<MonsterType, IEnumerable<Monster>> monsterGroupEntry in monsterGroupsByType)
                        {
                            totalHitPoints += monsterGroupEntry.Value.Sum(monster => monster.hitPoints);

                            string displayName = monsterGroupEntry.Key.displayName;
                            int count = monsterGroupEntry.Value.Count();
                            string description;

                            if (count == 1)
                            {
                                description = EnglishHelper.GetIndefiniteNounForm(displayName);
                            }
                            else
                            {
                                description = $"{count} {EnglishHelper.GetPluralNounForm(displayName)}";
                            }

                            monsterGroupDescriptions.Add(description);
                        }

                        string monstersDescription = EnglishHelper.JoinWithAnd(monsterGroupDescriptions);

                        Console.WriteLine($"Watch out, {monstersDescription} with {totalHitPoints} total HP appears!");
                    }

                    // Save the start of the combat.
                    SaveGameHelper.Save(_state);
                }

                // Present the monsters.
                yield return _combatPresenter.InitializeMonsters(_state);

                yield return new WaitForSeconds(1);

                // Simulate the combat.
                yield return _combatManager.Simulate(_state);

                // If everyone died, stop simulation.
                if (_state.party.aliveCount == 0)
                {
                    SaveGameHelper.Delete();

                    break;
                }

                // End the combat.
                _state.ExitCombat();

                // Take a short rest between fights.
                yield return _state.party.TakeShortRest();

                // Gain a health potion as a reward.
                foreach (Character character in _state.party.aliveCharacters)
                {
                    character.GiveItem(_state, Database.GetItemType("potion of healing").Create());
                }

                // Save the game before a new fight.
                SaveGameHelper.Save(_state);

                yield return new WaitForSeconds(1);
            }

            Console.WriteLine($"RIP. The heroes {_state.party} entered {EnglishHelper.GetNounWithCount("battle", _state.combatsFoughtCount)}, but their last one proved to be fatal.");
        }

        private List<Monster> CreateMonsters()
        {
            // Create a group of monsters where the total challenge rating doesn't exceed characters total levels.
            float maxTotalChallengeRating = _state.party.characters.Where(character => character.isAlive).Sum(character => character.characterClass.level) / 4f;
            float totalChallengeRating = 0;

            List<MonsterType> remainingMonsterTypes = new(Database.monsterTypes);
            List<Monster> monsters = new();

            while (totalChallengeRating < maxTotalChallengeRating && monsters.Count < 5)
            {
                float remainingChallengeRating = maxTotalChallengeRating - totalChallengeRating;

                List<MonsterType> monsterTypes = remainingMonsterTypes.Where(monster => monster.challengeRating <= remainingChallengeRating).ToList();

                if (!monsterTypes.Any()) break;

                // Choose a random monster type.
                MonsterType monsterType = monsterTypes.Random();

                // Create a random amount of monsters of this type (max 3, without exceeding the challenge rating or 5 total monsters).
                int maxCount = Mathf.FloorToInt(remainingChallengeRating / Math.Max(0.5f, monsterType.challengeRating));
                maxCount = Math.Min(5 - monsters.Count, Math.Min(3, maxCount));

                int count = Random.Range(1, maxCount + 1);

                for (int i = 0; i < count; i++)
                {
                    monsters.Add(new Monster(monsterType));
                }

                totalChallengeRating += Math.Max(1, monsterType.challengeRating * count);
                remainingMonsterTypes.Remove(monsterType);
            }

            // Shuffle the monsters for a random display order.
            monsters.Shuffle();

            return monsters;
        }
    }
}
