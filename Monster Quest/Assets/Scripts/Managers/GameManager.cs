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

        [SerializeField] private Database databaseObject;
        [SerializeField] private GameStateAsset loadGameState;

        private CombatManager _combatManager;
        private CombatPresenter _combatPresenter;
        private GameState _state;

        private static string saveFilePath => Path.Combine(Application.persistentDataPath, _saveFileName);
        private static bool saveFileExists => File.Exists(saveFilePath);

        public static Database database { get; private set; }

        private void Awake()
        {
            database = databaseObject;

            Transform combatTransform = transform.Find("Combat");
            _combatManager = combatTransform.GetComponent<CombatManager>();
            _combatPresenter = combatTransform.GetComponent<CombatPresenter>();
        }

        private void Start()
        {
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
            StartCoroutine(Simulate());
        }

        private void NewGame()
        {
            // Create a new party.
            Race humanRace = database.GetRace("human");
            ClassType fighterClassType = database.GetClass("fighter");

            Party party = new(new Character[] { new("Jazlyn", humanRace, fighterClassType, database.characterBodySprites[0]), new("Theron", humanRace, fighterClassType, database.characterBodySprites[1]), new("Dayana", humanRace, fighterClassType, database.characterBodySprites[2]), new("Rolando", humanRace, fighterClassType, database.characterBodySprites[3]) });

            ItemType[] weaponItemTypes = { database.GetItem("greatsword"), database.GetItem("javelin"), database.GetItem("greatsword"), database.GetItem("greataxe") };

            ItemType chainShirt = database.GetItem("chain shirt");

            for (int i = 0; i < 4; i++)
            {
                party.characters[i].GiveItem(weaponItemTypes[i].Create());
                party.characters[i].GiveItem(chainShirt.Create());
            }

            // Prepare the monster types to be fought.
            IEnumerable<MonsterType> monsterTypes = database.monsters.OrderBy(monsterType => monsterType.challengeRating);

            // Create a new game state.
            _state = new GameState(party, monsterTypes);

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
            _combatPresenter.InitializeParty(_state);

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
                _combatPresenter.InitializeMonster(_state);

                yield return new WaitForSeconds(1);

                // Simulate the combat.
                yield return _combatManager.Simulate(_state);

                // If everyone died, stop simulation.
                if (_state.party.characters.Count == 0)
                {
                    DeleteSavedGame();

                    break;
                }

                // End the combat and save the game before continuing.
                _state.ExitCombat();
                SaveGame();
            }

            switch (_state.party.characters.Count)
            {
                case > 1:
                    Console.WriteLine($"After many grueling fights, the heroes {_state.party.characters} return from the dungeons to live another day.");

                    break;

                case 1:
                    Console.WriteLine($"After many grueling fights, {_state.party.characters[0].displayName} returns from the dungeons. Unfortunately, none of the other party members survived.");

                    break;
            }
        }
    }
}
