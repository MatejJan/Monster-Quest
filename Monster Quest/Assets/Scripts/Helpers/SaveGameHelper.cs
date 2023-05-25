using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonsterQuest
{
    public static class SaveGameHelper
    {
        private const string _saveFileName = "save.json";
        private const string _gameStatesAssetFolderName = "Game States";
        private const string _gameStatesAssetFolderPath = "Assets/Game States";
        private const string _lastGameStateAssetPath = "Assets/Game States/Last.asset";

        private static readonly JsonSerializerSettings _settings = new()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableAttribute = false
            },
            Converters = new List<JsonConverter>
            {
                new UnityObjectConverter()
            }
        };

        public static bool saveFileExists => File.Exists(saveFilePath);

        private static string saveFilePath => Path.Combine(Application.persistentDataPath, _saveFileName);

        public static GameState Load()
        {
            string json = File.ReadAllText(saveFilePath);

            return JsonConvert.DeserializeObject<GameState>(json, _settings);
        }

        public static void Save(GameState state)
        {
            string json = JsonConvert.SerializeObject(state, _settings);
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

            AssetDatabase.SaveAssetIfDirty(gameStateAsset);
#endif
        }

        public static void Delete()
        {
            File.Delete(saveFilePath);
        }

        // We need to reference Unity Objects with their addressable primary key. 
        private class UnityObjectConverter : JsonConverter<Object>
        {
            public override void WriteJson(JsonWriter writer, Object asset, JsonSerializer serializer)
            {
                writer.WriteValue(Database.GetPrimaryKeyForAsset(asset));
            }

            public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                string primaryKey = (string)reader.Value;

                return Database.GetAssetForPrimaryKey<Object>(primaryKey);
            }
        }
    }
}
