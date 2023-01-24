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

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            layout.CloneTree(root);

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            return root;
        }
    }
}
