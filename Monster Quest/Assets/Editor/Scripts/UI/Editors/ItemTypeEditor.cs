using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomEditor(typeof(ItemType))]
    public class ItemTypeEditor : UnityEditor.Editor
    {
        private Label _descriptionLabel;
        private VisualElement _descriptionsArea;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            _descriptionsArea = new VisualElement();

            root.Add(_descriptionsArea);

            root.TrackSerializedObjectValue(serializedObject, OnSerializedObjectUpdated);
            OnSerializedObjectUpdated(null);

            return root;
        }

        private void OnSerializedObjectUpdated(SerializedObject _)
        {
            if (serializedObject.targetObject is not ItemType item) return;

            _descriptionsArea.Clear();

            foreach (RuleDescription ruleDescription in item.GetOwnRuleDescriptions())
            {
                Label descriptionLabel = new()
                {
                    text = $"<i><b>{ruleDescription.name.ToStartCase()}.</b> {ruleDescription.type.ToStartCase()}.</i> {ruleDescription.description}",
                    style =
                    {
                        marginTop = 10,
                        whiteSpace = WhiteSpace.Normal
                    }
                };

                _descriptionsArea.Add(descriptionLabel);
            }
        }
    }
}
