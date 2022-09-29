using System.Linq;
using MonsterQuest.Effects;
using UnityEngine;

namespace MonsterQuest
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Race[] races;
        [SerializeField] private ClassType[] classes;
        [SerializeField] private MonsterType[] monsterTypes;
        [SerializeField] private ItemType[] weaponTypes;

        private void Start()
        {
            Race humanRace = races.First(race => race.displayName == "human");
            ClassType fighterClassType = classes.First(characterClass => characterClass.displayName == "fighter");

            Party heroes = new() { new Character("Jazlyn", humanRace, fighterClassType), new Character("Theron", humanRace, fighterClassType), new Character("Dayana", humanRace, fighterClassType), new Character("Rolando", humanRace, fighterClassType) };

            for (int i = 0; i < 4; i++)
            {
                heroes[i].GiveItem(weaponTypes[i].Create());
            }

            Console.WriteLine($"A party of warriors ({heroes}) descends into the dungeon.");

            Battle battle = new() { heroes = heroes };

            foreach (MonsterType monsterType in monsterTypes)
            {
                battle.monster = new Monster(monsterType);
                battle.Simulate();

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
