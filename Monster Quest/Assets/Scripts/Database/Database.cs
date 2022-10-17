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

        public Race GetRace(string displayName)
        {
            return races.First(race => race.displayName == displayName);
        }

        public ClassType GetClass(string displayName)
        {
            return classes.First(classType => classType.displayName == displayName);
        }

        public MonsterType GetMonster(string displayName)
        {
            return monsters.First(monster => monster.displayName == displayName);
        }

        public ItemType GetItem(string displayName)
        {
            return items.First(item => item.displayName == displayName);
        }
    }
}
