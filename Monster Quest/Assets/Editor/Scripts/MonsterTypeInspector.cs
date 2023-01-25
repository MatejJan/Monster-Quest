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
        }

        private void OnSerializedObjectUpdated(SerializedObject _)
        {
            if (serializedObject.targetObject is not MonsterType monsterType) return;

            UpdateSpeed(monsterType);
            UpdateSavingThrowBonuses(monsterType);
            UpdateSkillBonuses(monsterType);
            UpdateDamageVulnerabilities(monsterType);
            UpdateDamageResistances(monsterType);
            UpdateDamageImmunities(monsterType);
            UpdateConditionImmunities(monsterType);
            UpdateSenseRanges(monsterType);
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

        private void UpdateFoldoutLabel(string propertyFieldName, string extraText)
        {
            PropertyField propertyField = _root.Q<PropertyField>(propertyFieldName);
            Label label = propertyField.Q<Label>(className: "unity-foldout__text");
            label.text = $"<b>{propertyField.label}</b> {extraText}";
        }
    }
}
