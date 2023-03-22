using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonsterQuest
{
    public static class SaveGameHelper
    {
        private static readonly string _saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
        
        private static readonly JsonSerializerSettings _settings = new()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new DefaultContractResolver()
            {
              IgnoreSerializableAttribute = false
            },
            Converters = new List<JsonConverter>
            {
                new UnityObjectConverter()
            }
        };

        public static bool saveFileExists => File.Exists(_saveFilePath);
        
        public static GameState Load()
        {
            string json = File.ReadAllText(_saveFilePath);
            return JsonConvert.DeserializeObject<GameState>(json, _settings);
        }

        public static void Save(GameState state)
        {
            string json = JsonConvert.SerializeObject(state, _settings);
            File.WriteAllText(_saveFilePath, json);
        }

        public static void Delete()
        {
            File.Delete(_saveFilePath);
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
