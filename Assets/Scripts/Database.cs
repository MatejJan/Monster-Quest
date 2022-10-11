using System.Linq;
using MonsterQuest.Effects;
using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Database", menuName = "Database")]
    public class Database : ScriptableObject
    {
        public Race[] races;
        public ClassType[] classes;
        public MonsterType[] monsters;
        public ItemType[] items;

        public Sprite[] standSprites;
        public Sprite[] characterBodySprites;

        public GameObject creaturePrefab;

        public Race GetRace(string name)
        {
            return races.First(race => race.displayName == name);
        }

        public ClassType GetClass(string name)
        {
            return classes.First(classType => classType.displayName == name);
        }

        public MonsterType GetMonster(string name)
        {
            return monsters.First(monster => monster.displayName == name);
        }

        public ItemType GetItem(string name)
        {
            return items.First(item => item.displayName == name);
        }
    }
}
