using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonsterQuest
{
    public static class MonsterTypeImporter
    {
        private static MonsterIndexEntry[] _monsterIndexEntries;
        private static string[] _monsterNames;

        public static string[] monsterNames
        {
            get
            {
                if (_monsterNames is null) LoadMonsterNames();
                return _monsterNames;
            }
        }
        
        public static void ImportData(string name, MonsterType monsterType)
        {
            MonsterIndexEntry monsterIndexEntry = _monsterIndexEntries.First(entry => entry.name == name);

            HttpClient httpClient = new();
            string monsterJson = httpClient.GetStringAsync($"https://www.dnd5eapi.co/api/monsters/{monsterIndexEntry.index}").Result;
            JObject monsterData = JObject.Parse(monsterJson);

            monsterType.displayName = ((string)monsterData["name"]).ToLowerInvariant();
            
            monsterType.sizeCategory = Enum.Parse<SizeCategory>((string)monsterData["size"], true);
            
            monsterType.alignment = (string)monsterData["alignment"];
            
            monsterType.hitPointsRoll = (string)monsterData["hit_points_roll"];

            if (monsterData["armor_class"]?[0] is not null)
            {
                monsterType.armorClass = (int)monsterData["armor_class"][0]["value"];
            }
            
            foreach (Ability ability in Enum.GetValues(typeof(Ability)))
            {
                if (ability is Ability.None) continue;

                monsterType.abilityScores[ability].score = (int)monsterData[ability.ToString().ToLowerInvariant()];
            }
            
            monsterType.challengeRating = (float)monsterData["challenge_rating"];
        }

        private static void LoadMonsterNames()
        {
            HttpClient httpClient = new();

            Task<string> requestTask = httpClient.GetStringAsync("https://www.dnd5eapi.co/api/monsters");
            string responseJson = requestTask.Result;
            
            MonsterIndexResponse monsterIndexResponse = JsonConvert.DeserializeObject<MonsterIndexResponse>(responseJson);

            _monsterIndexEntries = monsterIndexResponse.results;
            _monsterNames = _monsterIndexEntries.Select(entry => entry.name).ToArray();
        }
        
        private class MonsterIndexResponse
        {
            public int count;
            public MonsterIndexEntry[] results;
        }

        private class MonsterIndexEntry
        {
            public string index;
            public string name;
        }
    }
}
