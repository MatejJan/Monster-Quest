using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Effects;
using UnityEngine.AddressableAssets;

namespace MonsterQuest
{
    public static class Database
    {
        private static readonly List<RaceType> _raceTypes = new();
        private static readonly List<ClassType> _classTypes = new();
        private static readonly List<MonsterType> _monsterTypes = new();
        private static readonly List<ItemType> _itemTypes = new();

        public static IEnumerable<RaceType> raceTypes => _raceTypes;
        public static IEnumerable<ClassType> classTypes => _classTypes;
        public static IEnumerable<MonsterType> monsterTypes => _monsterTypes;
        public static IEnumerable<ItemType> itemTypes => _itemTypes;

        public static IEnumerator Initialize()
        {
            yield return Addressables.InitializeAsync();

            // Load all assets.
            yield return LoadAssets(_raceTypes);
            yield return LoadAssets(_classTypes);
            yield return LoadAssets(_monsterTypes);
            yield return LoadAssets(_itemTypes);
        }

        public static RaceType GetRaceType(string displayName)
        {
            return _raceTypes.First(raceType => raceType.displayName == displayName);
        }

        public static ClassType GetClassType(string displayName)
        {
            return _classTypes.First(classType => classType.displayName == displayName);
        }

        public static MonsterType GetMonsterType(string displayName)
        {
            return _monsterTypes.First(monster => monster.displayName == displayName);
        }

        public static ItemType GetItemType(string displayName)
        {
            return _itemTypes.First(item => item.displayName == displayName);
        }

        private static IEnumerator LoadAssets<TObject>(List<TObject> list)
        {
            yield return Addressables.LoadAssetsAsync<TObject>("database", list.Add);
        }
    }
}
