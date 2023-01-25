using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomEditor(typeof(MonsterType))]
    public class MonsterTypeInspector : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset layout;
        private IntegerField _telepathyRangeField;
        private Toggle _blindToggle;

        private Toggle _languageAbilitiesToggle;

        private Toggle _senseRangesToggle;

        private VisualElement _root;

        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement();
            layout.CloneTree(_root);

            //InspectorElement.FillDefaultInspector(root, serializedObject, this);

            _root.TrackSerializedObjectValue(serializedObject, OnSerializedObjectUpdated);
            _root.RegisterCallback<GeometryChangedEvent>(InitialRefresh);

            return _root;
        }

        private void InitialRefresh(GeometryChangedEvent _)
        {
            _root.UnregisterCallback<GeometryChangedEvent>(InitialRefresh);
            OnSerializedObjectUpdated(null);

            _blindToggle = _root.Q<Toggle>("blind");
            _senseRangesToggle = _root.Q("sense-ranges").Q<Toggle>();
            _senseRangesToggle.RegisterValueChangedCallback(UpdateBlindVisibility);
            UpdateBlindVisibility(null);

            _telepathyRangeField = _root.Q<IntegerField>("telepathy-range");
            _languageAbilitiesToggle = _root.Q("language-abilities").Q<Toggle>();
            _languageAbilitiesToggle.RegisterValueChangedCallback(UpdateTelepathyRangeVisibility);
            UpdateTelepathyRangeVisibility(null);
        }

        private void UpdateBlindVisibility(ChangeEvent<bool> _)
        {
            _blindToggle.style.display = _senseRangesToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void UpdateTelepathyRangeVisibility(ChangeEvent<bool> _)
        {
            _telepathyRangeField.style.display = _languageAbilitiesToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnSerializedObjectUpdated(SerializedObject _)
        {
            if (serializedObject.targetObject is not MonsterType monsterType) return;

            UpdateTags(monsterType);
            UpdateSpeed(monsterType);
            UpdateSavingThrowBonuses(monsterType);
            UpdateSkillBonuses(monsterType);
            UpdateDamageVulnerabilities(monsterType);
            UpdateDamageResistances(monsterType);
            UpdateDamageImmunities(monsterType);
            UpdateConditionImmunities(monsterType);
            UpdateSenseRanges(monsterType);
            UpdateLanguageAbilities(monsterType);
            UpdateXP(monsterType);
        }

        private void UpdateTags(MonsterType monsterType)
        {
            string description = "";

            if (monsterType.typeTags.Length > 0)
            {
                description = $"({string.Join(", ", monsterType.typeTags)})";
            }

            PropertyField propertyField = _root.Q<PropertyField>("type-tags");
            Label label = propertyField.Q<Label>(className: "unity-foldout__text");
            label.text = description;
        }

        private void UpdateSpeed(MonsterType monsterType)
        {
            string description = $"{monsterType.speed.walk} ft.";

            if (monsterType.speed.burrow > 0)
            {
                description += $", burrow {monsterType.speed.burrow} ft.";
            }

            if (monsterType.speed.climb > 0)
            {
                description += $", climb {monsterType.speed.climb} ft.";
            }

            if (monsterType.speed.fly > 0)
            {
                description += $", fly {monsterType.speed.fly} ft.";
            }

            if (monsterType.speed.hover)
            {
                description += " (hover)";
            }

            if (monsterType.speed.swim > 0)
            {
                description += $", swim {monsterType.speed.swim} ft.";
            }

            UpdateFoldoutLabel("speed", string.Join(", ", description));
        }

        private void UpdateSavingThrowBonuses(MonsterType monsterType)
        {
            IEnumerable<string> descriptions = monsterType.savingThrowBonuses.Select(bonus => $"{bonus.ability.ToString()[..3]} {bonus.amount:+#;-#;+0}");
            UpdateFoldoutLabel("saving-throw-bonuses", string.Join(", ", descriptions));
        }

        private void UpdateSkillBonuses(MonsterType monsterType)
        {
            IEnumerable<string> descriptions = monsterType.skillBonuses.Select(bonus => $"{bonus.skill} {bonus.amount:+#;-#;+0}");
            UpdateFoldoutLabel("skill-bonuses", string.Join(", ", descriptions));
        }

        private void UpdateDamageVulnerabilities(MonsterType monsterType)
        {
            IEnumerable<string> descriptions = monsterType.damageVulnerabilities.Select(damageType => damageType.ToString().ToLower());
            UpdateFoldoutLabel("damage-vulnerabilities", string.Join(", ", descriptions));
        }

        private void UpdateDamageResistances(MonsterType monsterType)
        {
            IEnumerable<string> descriptions = monsterType.damageResistances.Select(damageType => damageType.ToString().ToLower());
            UpdateFoldoutLabel("damage-resistances", string.Join(", ", descriptions));
        }

        private void UpdateDamageImmunities(MonsterType monsterType)
        {
            IEnumerable<string> descriptions = monsterType.damageImmunities.Select(damageType => damageType.ToString().ToLower());
            UpdateFoldoutLabel("damage-immunities", string.Join(", ", descriptions));
        }

        private void UpdateConditionImmunities(MonsterType monsterType)
        {
            IEnumerable<string> descriptions = monsterType.conditionImmunities.Select(damageType => damageType.ToString().ToLower());
            UpdateFoldoutLabel("condition-immunities", string.Join(", ", descriptions));
        }

        private void UpdateSenseRanges(MonsterType monsterType)
        {
            List<string> descriptions = monsterType.senseRanges.Select(senseRange => $"{senseRange.sense.ToString().ToLower()} {senseRange.range} ft.").ToList();

            if (monsterType.blind)
            {
                int blindsightIndex = descriptions.FindIndex(description => description.Contains("blindsight"));

                if (blindsightIndex > -1)
                {
                    descriptions[blindsightIndex] = $"{descriptions[blindsightIndex]} (blind beyond this radius)";
                }
            }

            descriptions.Add($"passive Perception {monsterType.passivePerception}");
            UpdateFoldoutLabel("sense-ranges", string.Join(", ", descriptions));
        }

        private void UpdateLanguageAbilities(MonsterType monsterType)
        {
            List<string> descriptions = monsterType.languageAbilities.Where(ability => ability.canSpeak).Select(ability => ability.language.ToString()).ToList();
            Language[] understoodLanguages = monsterType.languageAbilities.Where(ability => !ability.canSpeak).Select(ability => ability.language).ToArray();

            if (understoodLanguages.Length > 0)
            {
                string understoodDescription = $"understands {EnglishHelper.JoinWithAnd(understoodLanguages)} but can't speak";

                if (descriptions.Count > 0)
                {
                    understoodDescription += $" {(understoodLanguages.Length == 1 ? "it" : "them")}";
                }

                descriptions.Add(understoodDescription);
            }

            if (monsterType.telepathyRange > 0)
            {
                descriptions.Add($"telepathy {monsterType.telepathyRange} ft.");
            }

            if (descriptions.Count == 0) descriptions.Add("—");

            UpdateFoldoutLabel("language-abilities", string.Join(", ", descriptions));
        }

        private void UpdateXP(MonsterType monsterType)
        {
            Label xpLabel = _root.Q<Label>("experience-points");
            xpLabel.text = $" ({monsterType.experiencePoints} XP)";
        }

        private void UpdateFoldoutLabel(string propertyFieldName, string extraText)
        {
            PropertyField propertyField = _root.Q<PropertyField>(propertyFieldName);
            Label label = propertyField.Q<Label>(className: "unity-foldout__text");
            label.text = $"<b>{propertyField.label}</b> {extraText}";
        }
    }
}
