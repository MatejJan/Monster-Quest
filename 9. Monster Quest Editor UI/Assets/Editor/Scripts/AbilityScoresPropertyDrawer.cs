using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterQuest
{
    [CustomPropertyDrawer(typeof(AbilityScores))]
    public class AbilityScoresPropertyDrawer : PropertyDrawer
    {
        private Dictionary<Ability, Label> _modifierLabels = new();
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();

            foreach (Ability ability in Enum.GetValues(typeof(Ability)))
            {
                if (ability is Ability.None) continue;

                VisualElement abilityArea = new();
                abilityArea.style.flexDirection = FlexDirection.Row;
                root.Add(abilityArea);

                IntegerField abilityField = new();
                abilityField.style.flexGrow = 1;
                abilityField.label = ability.ToString();
                abilityField.bindingPath = $"<{ability.ToString().ToLowerInvariant()}>k__BackingField.<score>k__BackingField";
                abilityArea.Add(abilityField);

                Label modifierLabel = new();
                modifierLabel.style.marginLeft = 10;
                abilityArea.Add(modifierLabel);
                _modifierLabels[ability] = modifierLabel;
            }

            root.TrackSerializedObjectValue(property.serializedObject, OnSerializedObjectChanged);
            UpdateModifiers(property.serializedObject);

            return root;
        }

        private void OnSerializedObjectChanged(SerializedObject serializedObject)
        {
            UpdateModifiers(serializedObject);
        }
        
        private void UpdateModifiers(SerializedObject serializedObject) 
        {
            if (serializedObject.targetObject is not MonsterType monsterType) return;
            
            foreach (Ability ability in Enum.GetValues(typeof(Ability)))
            {
                if (ability is Ability.None) continue;

                _modifierLabels[ability].text = $"({monsterType.abilityScores[ability].modifier:+#;-#;+0})";
            }
        }
    }
}
