using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomPropertyDrawer(typeof(AbilityScore))]
    public class AbilityScoreInspector : PropertyDrawer
    {
        private readonly Label _modifier = new();

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.AddToClassList("value");

            IntegerField score = new();
            score.bindingPath = "<score>k__BackingField";
            score.AddToClassList("score");
            root.Add(score);

            root.Add(_modifier);
            _modifier.TrackPropertyValue(property, UpdateModifier);
            UpdateModifier(property);

            return root;
        }

        private void UpdateModifier(SerializedProperty property)
        {
            AbilityScore abilityScore = property.serializedObject as AbilityScore;
            _modifier.text = $"({abilityScore.modifier:+#;-#;+0})";
        }
    }
}
