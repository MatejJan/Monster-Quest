using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomPropertyDrawer(typeof(AbilityScore))]
    public class AbilityScorePropertyDrawer : PropertyDrawer
    {
        private Label _modifier;
        private SerializedProperty _property;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.AddToClassList("value");

            IntegerField score = new();
            score.bindingPath = "<score>k__BackingField";
            score.AddToClassList("score");
            root.Add(score);

            _modifier = new Label();
            root.Add(_modifier);

            _property = property;
            _modifier.TrackSerializedObjectValue(property.serializedObject, UpdateModifier);
            UpdateModifier(null);

            return root;
        }

        private void UpdateModifier(SerializedObject _)
        {
            AbilityScore abilityScore = _property.GetUnderlyingValue() as AbilityScore;
            _modifier.text = $"({abilityScore.modifier:+#;-#;+0})";
        }
    }
}
