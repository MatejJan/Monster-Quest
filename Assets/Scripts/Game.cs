using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonsterQuest.Effects;
using MonsterQuest.Presenters;
using UnityEngine;

namespace MonsterQuest
{
    public class Game : MonoBehaviour
    {
        private const string _saveFileName = "save.json";

        private static Game _instance;

        [SerializeField] private Database databaseObject;

        private BattlePresenter _battlePresenter;
        private static string saveFilePath => Path.Combine(Application.persistentDataPath, _saveFileName);
        private static bool saveFileExists => File.Exists(saveFilePath);

        public static GameState state { get; private set; }
        public static Database database { get; private set; }

        private void Awake()
        {
            _instance = this;

            database = databaseObject;

            _battlePresenter = transform.Find("Battle").GetComponent<BattlePresenter>();
        }

        private void Start()
        {
            if (saveFileExists)
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

            state.party = new Party(new Character[]
            {
                new("Jazlyn", humanRace, fighterClassType, database.characterBodySprites[0]),
                new("Theron", humanRace, fighterClassType, database.characterBodySprites[1]),
                new("Dayana", humanRace, fighterClassType, database.characterBodySprites[2]),
                new("Rolando", humanRace, fighterClassType, database.characterBodySprites[3])
            });

            ItemType[] weaponItemTypes =
            {
                database.GetItem("greatsword"),
                database.GetItem("javelin"),
                database.GetItem("greatsword"),
                database.GetItem("greataxe")
            };

            ItemType chainShirt = database.GetItem("chain shirt");

            for (int i = 0; i < 4; i++)
            {
                state.party.characters[i].GiveItem(weaponItemTypes[i].Create());
                state.party.characters[i].GiveItem(chainShirt.Create());
            }

            Console.WriteLine($"{state.party} descend into the dungeon.");

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
        }

        public static void DeleteSavedGame()
        {
            File.Delete(saveFilePath);
        }

        public static IEnumerator CallRules<T>(Func<T, IEnumerator> callback) where T : class
        {
            foreach (object rule in state.rules)
            {
                if (rule is T)
                {
                    IEnumerator result = callback(rule as T);

                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }

        public static IEnumerable<V> GetRuleValues<T, V>(Func<T, V> callback) where T : class
        {
            List<V> values = new();

            foreach (object rule in state.rules)
            {
                if (rule is T t)
                {
                    V value = callback(t);

                    if (value != null)
                    {
                        values.Add(value);
                    }
                }
            }

            return values;
        }

        private IEnumerator Simulate()
        {
            // Present the characters.
            _battlePresenter.InitializeParty();

            while (state.battle != null || state.remainingMonsterTypes.Count > 0)
            {
                // Create a new battle (it might already exist from a save file).
                if (state.battle == null)
                {
                    // Wait a bit before starting a new battle.
                    yield return new WaitForSeconds(1);

                    // Create the next monster.
                    MonsterType monsterType = state.remainingMonsterTypes[0];
                    state.remainingMonsterTypes.RemoveAt(0);
                    Monster monster = new(monsterType);
                    Console.WriteLine($"Watch out, {monster.indefiniteName} with {monster.hitPoints} HP appears!");

                    // Create a battle with the monster.
                    state.battle = new Battle(monster);
                }

                // Present the monster.
                _battlePresenter.InitializeMonster();
                yield return new WaitForSeconds(1);

                // Simulate the battle.
                yield return state.battle.Simulate();

                // If everyone died, stop simulation.
                if (state.party.characters.Count == 0)
                {
                    DeleteSavedGame();
                    break;
                }

                // End the battle and save the game before continuing.
                state.battle = null;
                SaveGame();
            }

            if (state.party.characters.Count > 1)
            {
                Console.WriteLine($"After many grueling battles, the heroes {state.party.characters} return from the dungeons to live another day.");
            }
            else if (state.party.characters.Count == 1)
            {
                Console.WriteLine($"After many grueling battles, {state.party.characters[0].displayName} returns from the dungeons. Unfortunately, none of the other party members survived.");
            }
        }
    }
}
