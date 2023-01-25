using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomPropertyDrawer(typeof(MonsterType.LanguageAbility))]
    public class MonsterTypeLanguageAbilityInspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.AddToClassList("property-list");

            EnumField languageField = new();
            languageField.bindingPath = "language";
            languageField.AddToClassList("property-name");
            root.Add(languageField);

            Toggle canSpeakToggle = new();
            canSpeakToggle.label = "can speak";
            canSpeakToggle.bindingPath = "canSpeak";
            canSpeakToggle.AddToClassList("property-value");
            root.Add(canSpeakToggle);

            return root;
        }
    }
}
