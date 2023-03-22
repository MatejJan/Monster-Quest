using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterQuest
{
    [CustomEditor(typeof(MonsterType))]
    public class MonsterTypeEditor : UnityEditor.Editor
    {
        private DropdownField _monsterNameField;
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            VisualElement importArea = new();
            importArea.style.flexDirection = FlexDirection.Row;
            root.Add(importArea);
            
            importArea.Add(new Label("Import monster data:"));

            _monsterNameField = new DropdownField();
            _monsterNameField.choices.AddRange(MonsterTypeImporter.monsterNames);
            _monsterNameField.RegisterValueChangedCallback(OnMonsterNameValueChanged);
            importArea.Add(_monsterNameField);
            
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            return root;
        }

        private void OnMonsterNameValueChanged(ChangeEvent<string> changeEvent)
        {
            MonsterTypeImporter.ImportData(changeEvent.newValue, serializedObject.targetObject as MonsterType);
        }
    }
}
