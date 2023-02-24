using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Effects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace MonsterQuest
{
    public static class Database
    {
        private static readonly List<RaceType> _raceTypes = new();
        private static readonly List<ClassType> _classTypes = new();
        private static readonly List<MonsterType> _monsterTypes = new();
        private static readonly List<ItemType> _itemTypes = new();
        private static readonly List<Sprite> _sprites = new();
        private static readonly List<Object> _allObjects = new();

        private static readonly Dictionary<Object, string> _primaryKeysByAssets = new();
        private static readonly Dictionary<string, Object> _assetsByPrimaryKey = new();

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

            // We also load all Unity objects so they get their instanceIDs indexed. We need to load the
            // sprites first so they get registered before their textures (which have the same primary key).
            yield return LoadAssets(_sprites);
            yield return LoadAssets(_allObjects);
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

        public static string GetPrimaryKeyForAsset(Object asset)
        {
            if (!_primaryKeysByAssets.ContainsKey(asset))
            {
                Debug.LogError("Referenced Unity Object is not part of the database.");

                return null;
            }

            return _primaryKeysByAssets[asset];
        }

        public static T GetAssetForPrimaryKey<T>(string primaryKey) where T : Object
        {
            if (!_assetsByPrimaryKey.ContainsKey(primaryKey))
            {
                Debug.LogError("Referenced addressable is not part of the database.");

                return null;
            }

            return _assetsByPrimaryKey[primaryKey] as T;
        }

        private static IEnumerator LoadAssets<TObject>(List<TObject> list) where TObject : Object
        {
            AsyncOperationHandle<IList<IResourceLocation>> loadResourceLocationsHandle = Addressables.LoadResourceLocationsAsync("database", typeof(TObject));

            if (!loadResourceLocationsHandle.IsDone) yield return loadResourceLocationsHandle;

            // Load referenced resources.
            List<AsyncOperationHandle> loadAssetHandles = new();

            foreach (IResourceLocation location in loadResourceLocationsHandle.Result)
            {
                AsyncOperationHandle<TObject> loadAssetHandle = Addressables.LoadAssetAsync<TObject>(location);

                loadAssetHandle.Completed += loadedAssetHandle =>
                {
                    TObject asset = loadedAssetHandle.Result;
                    list.Add(asset);

                    int instanceId = asset.GetInstanceID();

                    if (_primaryKeysByAssets.ContainsKey(asset))
                    {
                        if (_primaryKeysByAssets[asset] != location.PrimaryKey)
                        {
                            Debug.LogError($"Multiple assets with the same instance ID. {location.PrimaryKey} - {instanceId}");
                        }
                    }
                    else
                    {
                        _primaryKeysByAssets[asset] = location.PrimaryKey;
                    }

                    if (_assetsByPrimaryKey.ContainsKey(location.PrimaryKey))
                    {
                        if (_assetsByPrimaryKey[location.PrimaryKey] != asset)
                        {
                            // If we're loading a texture asset, the collision is probably with the associated sprite, so we don't raise an error.
                            if (asset is not Texture)
                            {
                                Debug.LogError($"Multiple assets with the same primary key. {location.PrimaryKey} - {instanceId}");
                            }
                        }
                    }
                    else
                    {
                        _assetsByPrimaryKey[location.PrimaryKey] = asset;
                    }
                };

                loadAssetHandles.Add(loadAssetHandle);
            }

            //create a GroupOperation to wait on all the above loads at once. 
            AsyncOperationHandle<IList<AsyncOperationHandle>> loadAssetsOperationHandle = Addressables.ResourceManager.CreateGenericGroupOperation(loadAssetHandles);

            if (!loadAssetsOperationHandle.IsDone) yield return loadAssetsOperationHandle;

            Addressables.Release(loadResourceLocationsHandle);
        }
    }
}
