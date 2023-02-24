using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
            ContractResolver = new CustomContractResolver(),
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

            AssetDatabase.SaveAssets();
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

        // We want to reuse Unity's SerializeField and SerializeReference attributes to know what to serialize and always use private default constructors.
        private class CustomContractResolver : DefaultContractResolver
        {
            public CustomContractResolver()
            {
                IgnoreSerializableAttribute = false;
                SerializeCompilerGeneratedMembers = true;
            }

            protected override JsonObjectContract CreateObjectContract(Type objectType)
            {
                JsonObjectContract jsonObjectContract = base.CreateObjectContract(objectType);

                jsonObjectContract.OverrideCreator = _ => FormatterServices.GetUninitializedObject(objectType);
                jsonObjectContract.CreatorParameters.Clear();

                return jsonObjectContract;
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                bool hasSerializeFieldAttribute = member.GetCustomAttributes(typeof(SerializeField)).Any();
                bool hasSerializeReferenceAttribute = member.GetCustomAttributes(typeof(SerializeReference)).Any();
                bool hasSerializeAttribute = hasSerializeFieldAttribute || hasSerializeReferenceAttribute;

                property.Writable = hasSerializeAttribute;
                property.ShouldSerialize = _ => hasSerializeAttribute;

                return property;
            }
        }
    }
}
