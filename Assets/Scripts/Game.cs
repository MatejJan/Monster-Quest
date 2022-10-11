using System.Collections;
using MonsterQuest.Controllers;
using MonsterQuest.Effects;
using UnityEngine;

namespace MonsterQuest
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Database databaseObject;
        private Transform _creaturesTransform;
        private Transform _worldTransform;

        public static Database database { get; private set; }

        public Battle battle { get; private set; }

        private void Awake()
        {
            database = databaseObject;

            _worldTransform = transform.Find("World");
            _creaturesTransform = _worldTransform.Find("Creatures");
        }

        private void Start()
        {
            StartCoroutine(Simulate());
        }

        private IEnumerator Simulate()
        {
            Race humanRace = database.GetRace("human");
            ClassType fighterClassType = database.GetClass("fighter");

            Party heroes = new()
            {
                new Character("Jazlyn", humanRace, fighterClassType, database.characterBodySprites[0]),
                new Character("Theron", humanRace, fighterClassType, database.characterBodySprites[1]),
                new Character("Dayana", humanRace, fighterClassType, database.characterBodySprites[2]),
                new Character("Rolando", humanRace, fighterClassType, database.characterBodySprites[3])
            };

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
                heroes[i].GiveItem(weaponItemTypes[i].Create());
                heroes[i].GiveItem(chainShirt.Create());
            }

            Console.WriteLine($"A party of warriors ({heroes}) descends into the dungeon.");

            battle = new Battle { heroes = heroes };

            for (int i = 0; i < heroes.Count; i++)
            {
                GameObject heroGameObject = Instantiate(database.creaturePrefab, _creaturesTransform);
                heroGameObject.transform.position = new Vector3(((heroes.Count - 1) * -0.5f + i) * 5, heroes[i].spaceTaken / 2, 0);
                heroGameObject.GetComponent<CreatureController>().Initialize(this, heroes[i]);
            }

            yield return new WaitForSeconds(1);

            MonsterType[] monsterTypes =
            {
                database.GetMonster("giant bat"),
                database.GetMonster("azer"),
                database.GetMonster("troll")
            };

            foreach (MonsterType monsterType in monsterTypes)
            {
                battle.monster = new Monster(monsterType);
                GameObject monsterGameObject = Instantiate(database.creaturePrefab, _creaturesTransform);
                monsterGameObject.transform.position = new Vector3(0, -battle.monster.spaceTaken / 2, 0);
                monsterGameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                monsterGameObject.GetComponent<CreatureController>().Initialize(this, battle.monster);

                yield return new WaitForSeconds(1);

                yield return battle.Simulate();

                if (heroes.Count == 0) break;
            }

            if (heroes.Count > 1)
            {
                Console.WriteLine($"After three grueling battles, the heroes {heroes} return from the dungeons to live another day.");
            }
            else if (heroes.Count == 1)
            {
                Console.WriteLine($"After three grueling battles, {heroes[0].name} returns from the dungeons. Unfortunately, none of the other party members survived.");
            }
        }
    }
}
