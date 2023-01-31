using MonsterQuest.Effects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomEditor(typeof(AttackType), true)]
    public class AttackTypeEditor : UnityEditor.Editor
    {
        private Label _descriptionLabel;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            _descriptionLabel = new Label
            {
                style =
                {
                    marginTop = 10,
                    whiteSpace = WhiteSpace.Normal
                }
            };

            root.Add(_descriptionLabel);

            root.TrackSerializedObjectValue(serializedObject, OnSerializedObjectUpdated);
            OnSerializedObjectUpdated(null);

            return root;
        }

        private void OnSerializedObjectUpdated(SerializedObject _)
        {
            if (serializedObject.targetObject is not AttackType attack) return;

            RuleDescription ruleDescription = attack.GetRuleDescription();

            _descriptionLabel.text = $"<i><b>{ruleDescription.name.ToStartCase()}.</b> {ruleDescription.type.ToStartCase()}.</i> {ruleDescription.description}";
        }
    }
}
