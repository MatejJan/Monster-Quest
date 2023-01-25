using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomPropertyDrawer(typeof(MonsterType.SkillBonus))]
    public class MonsterTypeSkillBonusInspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.AddToClassList("property-list");

            EnumField skillField = new();
            skillField.bindingPath = "skill";
            skillField.AddToClassList("property-name");
            root.Add(skillField);

            IntegerField amountField = new();
            amountField.bindingPath = "amount";
            amountField.AddToClassList("property-value");
            root.Add(amountField);

            return root;
        }
    }
}
