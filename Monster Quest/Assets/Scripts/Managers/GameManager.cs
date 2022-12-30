using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonsterQuest.Effects;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MonsterQuest
{
    public class GameManager : MonoBehaviour
    {
        private const string _saveFileName = "save.json";
        private const string _gameStatesAssetFolderName = "Game States";
        private const string _gameStatesAssetFolderPath = "Assets/Game States";
        private const string _lastGameStateAssetPath = "Assets/Game States/Last.asset";

        [SerializeField] private GameStateAsset loadGameState;

        [SerializeField] private Sprite[] characterBodySprites;

        private CombatManager _combatManager;
        private CombatPresenter _combatPresenter;
        private GameState _state;

        private static string saveFilePath => Path.Combine(Application.persistentDataPath, _saveFileName);
        private static bool saveFileExists => File.Exists(saveFilePath);

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
            _combatManager.onRoundEnd += SaveGame;

            // Load an existing or start a new game.
            if (loadGameState)
            {
                _state = loadGameState.gameState;
            }
            else if (saveFileExists)
            {
                LoadGame();
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
            int challengeRating = 1;

            // Create a new party.
            RaceType humanRaceType = Database.GetRaceType("human");
            ClassType fighterClassType = Database.GetClassType("fighter");

            Character[] characters =
            {
                new("Jazlyn", humanRaceType, fighterClassType, characterBodySprites[0]),
                new("Theron", humanRaceType, fighterClassType, characterBodySprites[1]),
                new("Dayana", humanRaceType, fighterClassType, characterBodySprites[2]),
                new("Rolando", humanRaceType, fighterClassType, characterBodySprites[3])
            };

            Party party = new(characters);

            // Prepare the monster types to be fought.
            IList<MonsterType> monsterTypes = Database.monsterTypes.Where(monster => monster.challengeRating >= challengeRating).OrderBy(monster => monster.challengeRating).ToList();

            // Create a new game state.
            _state = new GameState(party, monsterTypes);

            // Level up the characters to the challenge rating.
            foreach (Character character in characters)
            {
                while (character.characterClass.level < challengeRating)
                {
                    character.LevelUp();
                }
            }

            // Give characters equipment.
            ItemType[] weaponItemTypes =
            {
                Database.GetItemType("greatsword"),
                Database.GetItemType("javelin"),
                Database.GetItemType("greatsword"),
                Database.GetItemType("greataxe")
            };

            ItemType chainShirt = Database.GetItemType("chain shirt");

            ItemType potionOfHealing = Database.GetItemType("potion of healing");

            for (int i = 0; i < 4; i++)
            {
                characters[i].GiveItem(_state, weaponItemTypes[i].Create());
                characters[i].GiveItem(_state, chainShirt.Create());

                for (int j = 0; j < challengeRating; j++)
                {
                    characters[i].GiveItem(_state, potionOfHealing.Create());
                }
            }

            // Output intro.
            Console.WriteLine($"Warriors {_state.party} descend into the dungeon.");
        }

        private void LoadGame()
        {
            string json = File.ReadAllText(saveFilePath);
            _state = JsonUtility.FromJson<GameState>(json);
        }

        private void SaveGame()
        {
            string json = JsonUtility.ToJson(_state);
            File.WriteAllText(saveFilePath, json);

#if UNITY_EDITOR
            if (!AssetDatabase.IsValidFolder(_gameStatesAssetFolderPath))
            {
                AssetDatabase.CreateFolder("Assets", _gameStatesAssetFolderName);
            }

            AssetDatabase.DeleteAsset(_lastGameStateAssetPath);

            GameStateAsset gameStateAsset = ScriptableObject.CreateInstance<GameStateAsset>();
            gameStateAsset.gameState = _state;
            AssetDatabase.CreateAsset(gameStateAsset, _lastGameStateAssetPath);

            AssetDatabase.SaveAssets();
#endif
        }

        private void DeleteSavedGame()
        {
            File.Delete(saveFilePath);
        }

        private IEnumerator Simulate()
        {
            // Present the characters.
            yield return _combatPresenter.InitializeParty(_state);

            while (_state.combat is not null || _state.remainingMonsterTypes.Count > 0)
            {
                // See if we need to enter a new combat (it might already exist from a save file).
                if (_state.combat is null)
                {
                    // Wait a bit before starting new combat.
                    yield return new WaitForSeconds(1);

                    // Create the next monster.
                    MonsterType monsterType = _state.remainingMonsterTypes[0];
                    _state.remainingMonsterTypes.RemoveAt(0);
                    Monster monster = new(monsterType);
                    Console.WriteLine($"Watch out, {monster.indefiniteName} with {monster.hitPoints} HP appears!");

                    // Enter a combat with the monster.
                    _state.EnterCombatWithMonster(monster);
                }

                // Present the monster.
                yield return _combatPresenter.InitializeMonster(_state);

                yield return new WaitForSeconds(1);

                // Simulate the combat.
                yield return _combatManager.Simulate(_state);

                // If everyone died, stop simulation.
                if (_state.party.count == 0)
                {
                    DeleteSavedGame();

                    break;
                }

                // End the combat.
                _state.ExitCombat();

                // Take a short rest between fights.
                yield return _state.party.TakeShortRest();

                // save the game before a new fight.
                SaveGame();
            }

            switch (_state.party.count)
            {
                case > 1:
                    Console.WriteLine($"After many grueling fights, the heroes {_state.party} return from the dungeons to live another day.");

                    break;

                case 1:
                    Console.WriteLine($"After many grueling fights, {_state.party.characters.First().displayName} returns from the dungeons. Unfortunately, none of the other party members survived.");

                    break;
            }
        }
    }
}
