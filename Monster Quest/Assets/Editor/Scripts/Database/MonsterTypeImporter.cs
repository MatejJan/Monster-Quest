using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MonsterQuest
{
    public static class MonsterTypeImporter
    {
        private static Task _initializationTask;
        private static bool _initialized;

        private static MonsterIndexEntry[] _monsterIndexEntries;

        private static readonly Dictionary<string, Ability> _abilitiesByShorthand = new();

        static MonsterTypeImporter()
        {
            foreach (Ability ability in Enum.GetValues(typeof(Ability)))
            {
                if (ability is Ability.None) continue;

                string shorthand = ability.ToString().ToLowerInvariant().Substring(0, 3);
                _abilitiesByShorthand[shorthand] = ability;
            }
        }

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
            });
        }

        public static IEnumerable<string> GetMatchingMonsterNames(string pattern)
        {
            _initializationTask?.Wait();

            Regex nameRegex = new(pattern, RegexOptions.IgnoreCase);

            return _monsterIndexEntries.Where(entry => nameRegex.IsMatch(entry.name)).Select(entry => entry.name);
        }

        public static void ImportData(MonsterType monsterType)
        {
            Regex nameRegex = new(monsterType.displayName, RegexOptions.IgnoreCase);
            MonsterIndexEntry monsterIndexEntry = _monsterIndexEntries.FirstOrDefault(entry => nameRegex.IsMatch(entry.name));

            if (monsterIndexEntry is null)
            {
                Debug.Log($"Could not find monster with name {monsterType.name}.");

                return;
            }

            try
            {
                HttpClient httpClient = new();
                string monsterJson = httpClient.GetStringAsync($"https://www.dnd5eapi.co/api/monsters/{monsterIndexEntry.index}").Result;
                JObject monsterData = JObject.Parse(monsterJson);

                monsterType.displayName = ((string)monsterData["name"])?.ToLowerInvariant();

                monsterType.sizeCategory = Enum.Parse<SizeCategory>((string)monsterData["size"], true);

                string monsterTypeText = (string)monsterData["type"];

                if (monsterTypeText is not null)
                {
                    if (monsterTypeText.ToLowerInvariant().Contains("swarm"))
                    {
                        int typeStartIndex = monsterTypeText.LastIndexOf(" ", StringComparison.Ordinal) + 1;
                        monsterTypeText = monsterTypeText[typeStartIndex..^1];
                    }

                    monsterType.typeCategory = Enum.Parse<MonsterTypeCategory>(monsterTypeText, true);
                }

                monsterType.subtype = (string)monsterData["subtype"];
                monsterType.alignment = (string)monsterData["alignment"];

                if (monsterData["armor_class"]?[0] is not null)
                {
                    monsterType.armorClass = (int)monsterData["armor_class"][0]["value"];
                }

                monsterType.hitPointsRoll = (string)monsterData["hit_points_roll"];

                if (monsterData["speed"] is not null)
                {
                    foreach (JProperty property in monsterData["speed"].Children<JProperty>())
                    {
                        if (property.Name is "hover")
                        {
                            monsterType.speed.hover = (bool)property;

                            continue;
                        }

                        MovementType movementType = Enum.Parse<MovementType>(property.Name, true);
                        monsterType.speed[movementType] = ExtractInteger((string)property.Value);
                    }
                }

                foreach (Ability ability in Enum.GetValues(typeof(Ability)))
                {
                    if (ability is Ability.None) continue;

                    monsterType.abilityScores[ability].score = (int)monsterData[ability.ToString().ToLowerInvariant()];
                }

                if (monsterData["proficiencies"] is not null)
                {
                    List<MonsterType.SavingThrowBonus> savingThrowBonuses = new();
                    List<MonsterType.SkillBonus> skillBonuses = new();

                    foreach (JToken proficiency in monsterData["proficiencies"])
                    {
                        if (proficiency["proficiency"] is null) continue;

                        string proficiencyIndex = (string)proficiency["proficiency"]["index"];

                        if (proficiencyIndex is null) continue;

                        int lastDashIndex = proficiencyIndex.LastIndexOf("-", StringComparison.Ordinal);
                        string proficiencyCategory = proficiencyIndex[..lastDashIndex];
                        string proficiencyType = proficiencyIndex[(lastDashIndex + 1)..];

                        int amount = (int)proficiency["value"];

                        switch (proficiencyCategory)
                        {
                            case "saving-throw":
                                Ability ability = _abilitiesByShorthand[proficiencyType];

                                savingThrowBonuses.Add(new MonsterType.SavingThrowBonus
                                {
                                    ability = ability,
                                    amount = amount
                                });

                                break;

                            case "skill":
                                Skill skill = Enum.Parse<Skill>(proficiencyType, true);

                                skillBonuses.Add(new MonsterType.SkillBonus
                                {
                                    skill = skill,
                                    amount = amount
                                });

                                break;
                        }
                    }

                    monsterType.savingThrowBonuses = savingThrowBonuses.ToArray();
                    monsterType.skillBonuses = skillBonuses.ToArray();
                }

                monsterType.damageVulnerabilities = ExtractDamageTypes(monsterData["damage_vulnerabilities"]);
                monsterType.damageResistances = ExtractDamageTypes(monsterData["damage_resistances"]);
                monsterType.damageImmunities = ExtractDamageTypes(monsterData["damage_immunities"]);
                monsterType.conditionImmunities = ExtractConditionImmunities(monsterData["condition_immunities"]);

                if (monsterData["senses"] is not null)
                {
                    List<MonsterType.SenseRange> senseRanges = new();

                    foreach (JProperty property in monsterData["senses"].Children<JProperty>())
                    {
                        Sense sense;

                        if (property.Name is "passive_perception")
                        {
                            int passivePerception = (int)property.Value;

                            if (monsterType.passivePerception != passivePerception)
                            {
                                Debug.LogWarning($"Calculated passive perception ({monsterType.passivePerception}) does not match the provided one ({passivePerception})");
                            }
                        }
                        else if (Enum.TryParse(property.Name, true, out sense))
                        {
                            senseRanges.Add(new MonsterType.SenseRange
                            {
                                sense = sense,
                                range = ExtractInteger((string)property.Value)
                            });
                        }
                    }

                    monsterType.senseRanges = senseRanges.ToArray();

                    string blindsight = (string)monsterData["senses"]["blindsight"];

                    if (blindsight is not null)
                    {
                        monsterType.blind = blindsight.Contains("blind");
                    }
                }

                string languages = (string)monsterData["languages"];

                if (languages is not null)
                {
                    List<MonsterType.LanguageAbility> languageAbilities = new();

                    languages = languages.Replace("Giant Eagle", "GiantEagle");
                    languages = languages.Replace("Deep Speech", "DeepSpeech");

                    string[] parts = languages.Split(",");

                    foreach (string part in parts)
                    {
                        if (part.Contains("telepathy"))
                        {
                            monsterType.telepathyRange = ExtractInteger(part);
                        }
                        else if (Enum.TryParse(part, true, out Language language))
                        {
                            languageAbilities.Add(new MonsterType.LanguageAbility
                            {
                                language = language,
                                canSpeak = true
                            });
                        }
                        else
                        {
                            string[] words = part.Split(' ', ',');

                            foreach (string word in words)
                            {
                                if (Enum.TryParse(word, true, out language))
                                {
                                    languageAbilities.Add(new MonsterType.LanguageAbility
                                    {
                                        language = language,
                                        canSpeak = false
                                    });
                                }
                            }
                        }
                    }

                    monsterType.languageAbilities = languageAbilities.ToArray();
                }

                monsterType.challengeRating = (float)monsterData["challenge_rating"];

                int experiencePoints = (int)monsterData["xp"];

                if (monsterType.experiencePoints != experiencePoints)
                {
                    Debug.LogWarning($"Calculated experience points ({monsterType.experiencePoints}) do not match provided ones ({experiencePoints})");
                }

                /*
"challenge_rating": 2,
"xp": 450,
"special_abilities": [
{
  "name": "Heated Body",
  "desc": "A creature that touches the azer or hits it with a melee attack while within 5 ft. of it takes 5 (1d10) fire damage.",
  "damage": [
    {
      "damage_type": {
        "index": "fire",
        "name": "Fire",
        "url": "/api/damage-types/fire"
      },
      "damage_dice": "1d10"
    }
  ]
},
{
  "name": "Heated Weapons",
  "desc": "When the azer hits with a metal melee weapon, it deals an extra 3 (1d6) fire damage (included in the attack).",
  "damage": [
    {
      "damage_type": {
        "index": "fire",
        "name": "Fire",
        "url": "/api/damage-types/fire"
      },
      "damage_dice": "1d6"
    }
  ]
},
{
  "name": "Illumination",
  "desc": "The azer sheds bright light in a 10-foot radius and dim light for an additional 10 ft.."
}
],
"actions": [
{
  "name": "Warhammer",
  "desc": "Melee Weapon Attack: +5 to hit, reach 5 ft., one target. Hit: 7 (1d8 + 3) bludgeoning damage, or 8 (1d10 + 3) bludgeoning damage if used with two hands to make a melee attack, plus 3 (1d6) fire damage.",
  "attack_bonus": 5,
  "damage": [
    {
      "damage_type": {
        "index": "bludgeoning",
        "name": "Bludgeoning",
        "url": "/api/damage-types/bludgeoning"
      },
      "damage_dice": "1d8+3"
    },
    {
      "damage_type": {
        "index": "fire",
        "name": "Fire",
        "url": "/api/damage-types/fire"
      },
      "damage_dice": "1d6"
    }
  ],
  "actions": []
}
],
"url": "/api/monsters/azer",
"legendary_actions": []
}
                 */
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error parsing data for monster. {exception.Message}");
            }
        }

        private static int ExtractInteger(string text)
        {
            if (text is null) throw new ArgumentException();

            Match match = Regex.Match(text, @"(\d+)");

            return int.Parse(match.Groups[1].Value);
        }

        private static DamageType[] ExtractDamageTypes(JToken damageTypesToken)
        {
            List<DamageType> damageTypes = new();

            foreach (JToken token in damageTypesToken)
            {
                string description = (string)token;

                if (description is null) continue;

                if (Enum.TryParse(description, true, out DamageType damageType))
                {
                    damageTypes.Add(damageType);
                }
                else
                {
                    Debug.LogWarning($"Parsing advanced damage type: {description}");
                    DamageType baseDamageType = DamageType.None;

                    if (description.Contains("nonmagical"))
                    {
                        baseDamageType = DamageType.Nonmagical;
                    }
                    else if (description.Contains("magical"))
                    {
                        baseDamageType = DamageType.Magical;
                    }

                    string[] words = description.Split(' ', ',');

                    foreach (string word in words)
                    {
                        if (Enum.TryParse(word, true, out damageType))
                        {
                            if (baseDamageType != damageType)
                            {
                                damageTypes.Add(baseDamageType | damageType);
                            }
                        }
                    }
                }
            }

            return damageTypes.ToArray();
        }

        private static Condition[] ExtractConditionImmunities(JToken conditionImmunitiesToken)
        {
            List<Condition> conditionImmunities = new();

            foreach (JToken token in conditionImmunitiesToken)
            {
                string conditionName = (string)token["name"];

                if (conditionName is null) continue;

                if (Enum.TryParse(conditionName, true, out Condition condition))
                {
                    conditionImmunities.Add(condition);
                }
                else
                {
                    Debug.LogError($"Unknown condition: {conditionName}");
                }
            }

            return conditionImmunities.ToArray();
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
