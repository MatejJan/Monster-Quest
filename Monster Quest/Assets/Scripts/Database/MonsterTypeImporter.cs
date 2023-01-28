using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace MonsterQuest
{
    public static class MonsterTypeImporter
    {
        private static Task _initializationTask;
        private static bool _initialized;

        private static MonsterIndexEntry[] _monsterIndexEntries;

        public static void Initialize()
        {
            if (_initializationTask is not null || _initialized) return;

            HttpClient httpClient = new();

            _initializationTask = httpClient.GetStringAsync("https://www.dnd5eapi.co/api/monsters").ContinueWith(requestTask =>
            {
                _initializationTask = null;

                if (requestTask.Exception is not null)
                {
                    Debug.Log(requestTask.Exception.Message);

                    return;
                }

                MonsterIndexResponse monsterIndexResponse;

                try
                {
                    monsterIndexResponse = JsonConvert.DeserializeObject<MonsterIndexResponse>(requestTask.Result);
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);

                    return;
                }

                if (monsterIndexResponse == null)
                {
                    Debug.Log("Monster index did not return a correct response. Returned json was:");
                    Debug.Log(requestTask.Result);

                    return;
                }

                _monsterIndexEntries = monsterIndexResponse.results;

                _initialized = true;
                Debug.Log("Initialized MonsterType importer.");
            });

            Debug.Log("Initializing MonsterType importer ...");
        }

        public static IEnumerable<string> GetMatchingMonsterNames(string pattern)
        {
            _initializationTask?.Wait();

            Regex nameRegex = new(pattern, RegexOptions.IgnoreCase);

            return _monsterIndexEntries.Where(monsterIndexEntry => nameRegex.IsMatch(monsterIndexEntry.name)).Select(monsterIndexEntry => monsterIndexEntry.name);
        }

        public static void ImportData(MonsterType monsterType) { }

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
