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

        private static string saveFilePath => Path.Combine(Application.persistentDataPath, _saveFileName);
        private static bool saveFileExists => File.Exists(saveFilePath);

        public static GameState state { get; private set; }
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
                state = loadGameState.gameState;
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

        public static void NewGame()
        {
            // Create a new game state.
            state = new GameState();

            // Create a new party.
            Race humanRace = database.GetRace("human");
            ClassType fighterClassType = database.GetClass("fighter");

            state.party = new Party(new Character[] { new("Jazlyn", humanRace, fighterClassType, database.characterBodySprites[0]), new("Theron", humanRace, fighterClassType, database.characterBodySprites[1]), new("Dayana", humanRace, fighterClassType, database.characterBodySprites[2]), new("Rolando", humanRace, fighterClassType, database.characterBodySprites[3]) });

            ItemType[] weaponItemTypes = { database.GetItem("greatsword"), database.GetItem("javelin"), database.GetItem("greatsword"), database.GetItem("greataxe") };

            ItemType chainShirt = database.GetItem("chain shirt");

            for (int i = 0; i < 4; i++)
            {
                state.party.characters[i].GiveItem(weaponItemTypes[i].Create());
                state.party.characters[i].GiveItem(chainShirt.Create());
            }

            Console.WriteLine($"Warriors {state.party} descend into the dungeon.");

            state.remainingMonsterTypes.AddRange(database.monsters.OrderBy(monsterType => monsterType.challengeRating));
        }

        public static void LoadGame()
        {
            string json = File.ReadAllText(saveFilePath);
            state = JsonUtility.FromJson<GameState>(json);
        }

        public static void SaveGame()
        {
            string json = JsonUtility.ToJson(state);
            File.WriteAllText(saveFilePath, json);

#if UNITY_EDITOR
            if (!AssetDatabase.IsValidFolder(_gameStatesAssetFolderPath))
            {
                AssetDatabase.CreateFolder("Assets", _gameStatesAssetFolderName);
            }

            AssetDatabase.DeleteAsset(_lastGameStateAssetPath);

            GameStateAsset gameStateAsset = ScriptableObject.CreateInstance<GameStateAsset>();
            gameStateAsset.gameState = state;
            AssetDatabase.CreateAsset(gameStateAsset, _lastGameStateAssetPath);

            AssetDatabase.SaveAssets();
#endif
        }

        public static void DeleteSavedGame()
        {
            File.Delete(saveFilePath);
        }

        private IEnumerator Simulate()
        {
            // Present the characters.
            _combatPresenter.InitializeParty();

            while (state.combat is not null || state.remainingMonsterTypes.Count > 0)
            {
                // See if we need to create a new combat (it might already exist from a save file).
                if (state.combat is null)
                {
                    // Wait a bit before starting new combat.
                    yield return new WaitForSeconds(1);

                    // Create the next monster.
                    MonsterType monsterType = state.remainingMonsterTypes[0];
                    state.remainingMonsterTypes.RemoveAt(0);
                    Monster monster = new(monsterType);
                    Console.WriteLine($"Watch out, {monster.indefiniteName} with {monster.hitPoints} HP appears!");

                    // Create a combat with the monster.
                    state.combat = new Combat(state, monster);
                }

                // Present the monster.
                _combatPresenter.InitializeMonster();

                yield return new WaitForSeconds(1);

                // Simulate the combat.
                yield return _combatManager.Simulate(state);

                // If everyone died, stop simulation.
                if (state.party.characters.Count == 0)
                {
                    DeleteSavedGame();

                    break;
                }

                // End the combat and save the game before continuing.
                state.combat = null;
                SaveGame();
            }

            switch (state.party.characters.Count)
            {
                case > 1:
                    Console.WriteLine($"After many grueling fights, the heroes {state.party.characters} return from the dungeons to live another day.");

                    break;

                case 1:
                    Console.WriteLine($"After many grueling fights, {state.party.characters[0].displayName} returns from the dungeons. Unfortunately, none of the other party members survived.");

                    break;
            }
        }
    }
}