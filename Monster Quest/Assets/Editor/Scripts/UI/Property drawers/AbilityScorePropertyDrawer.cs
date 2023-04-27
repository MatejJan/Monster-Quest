using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomPropertyDrawer(typeof(AbilityScore))]
    public class AbilityScorePropertyDrawer : PropertyDrawer
    {
        private AbilityScore _abilityScore;
        private Label _modifier;

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

            // Note: We have to track the score property itself because tracking classes in general is not supported.
            // We do have to save the ability to be able to get to the modifier though.
            _abilityScore = property.managedReferenceValue as AbilityScore;
            _modifier.TrackPropertyValue(property.FindPropertyRelative(score.bindingPath), UpdateModifier);
            UpdateModifier(null);

            return root;
        }

        private void UpdateModifier(SerializedProperty _)
        {
            _modifier.text = $"({_abilityScore.modifier:+#;-#;+0})";
        }
    }
}
