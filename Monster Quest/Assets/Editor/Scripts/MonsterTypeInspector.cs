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

            PropertyField savingThrowBonusesField = _root.Q<PropertyField>("saving-throw-bonuses");
            Label savingThrowBonusesLabel = savingThrowBonusesField.Q<Label>(className: "unity-foldout__text");

            IEnumerable<string> bonusDescriptions = monsterType.savingThrowBonuses.Select(bonus => $"{bonus.ability.ToString()[..3]} {bonus.amount:+#;-#;+0}");
            savingThrowBonusesLabel.text = $"{savingThrowBonusesField.label} {string.Join(", ", bonusDescriptions)}";
        }
    }
}
