using System.Collections;
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

            StartCoroutine(Simulate());
        }

        public void NewGame()
        {
            // Create a new game state.
            _state = new GameState();

            // Create a new party.
            Race humanRace = database.GetRace("human");
            ClassType fighterClassType = database.GetClass("fighter");

            _state.party = new Party(new Character[] { new("Jazlyn", humanRace, fighterClassType, database.characterBodySprites[0]), new("Theron", humanRace, fighterClassType, database.characterBodySprites[1]), new("Dayana", humanRace, fighterClassType, database.characterBodySprites[2]), new("Rolando", humanRace, fighterClassType, database.characterBodySprites[3]) });

            ItemType[] weaponItemTypes = { database.GetItem("greatsword"), database.GetItem("javelin"), database.GetItem("greatsword"), database.GetItem("greataxe") };

            ItemType chainShirt = database.GetItem("chain shirt");

            for (int i = 0; i < 4; i++)
            {
                _state.party.characters[i].GiveItem(weaponItemTypes[i].Create());
                _state.party.characters[i].GiveItem(chainShirt.Create());
            }

            Console.WriteLine($"Warriors {_state.party} descend into the dungeon.");

            _state.remainingMonsterTypes.AddRange(database.monsters.OrderBy(monsterType => monsterType.challengeRating));
        }

        public void LoadGame()
        {
            string json = File.ReadAllText(saveFilePath);
            _state = JsonUtility.FromJson<GameState>(json);
        }

        public void SaveGame()
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

        public void DeleteSavedGame()
        {
            File.Delete(saveFilePath);
        }

        private IEnumerator Simulate()
        {
            // Present the characters.
            _combatPresenter.InitializeParty(_state);

            while (_state.combat is not null || _state.remainingMonsterTypes.Count > 0)
            {
                // See if we need to create a new combat (it might already exist from a save file).
                if (_state.combat is null)
                {
                    // Wait a bit before starting new combat.
                    yield return new WaitForSeconds(1);

                    // Create the next monster.
                    MonsterType monsterType = _state.remainingMonsterTypes[0];
                    _state.remainingMonsterTypes.RemoveAt(0);
                    Monster monster = new(monsterType);
                    Console.WriteLine($"Watch out, {monster.indefiniteName} with {monster.hitPoints} HP appears!");

                    // Create a combat with the monster.
                    _state.combat = new Combat(_state, monster);
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
                _state.combat = null;
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
