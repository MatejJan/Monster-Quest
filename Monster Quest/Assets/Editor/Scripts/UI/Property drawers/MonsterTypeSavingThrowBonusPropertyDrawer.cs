using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomPropertyDrawer(typeof(MonsterType.SavingThrowBonus))]
    public class MonsterTypeSavingThrowBonusPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.AddToClassList("property-list");

            EnumField abilityField = new();
            abilityField.bindingPath = "ability";
            abilityField.AddToClassList("property-name");
            root.Add(abilityField);

            IntegerField amountField = new();
            amountField.bindingPath = "amount";
            amountField.AddToClassList("property-value");
            root.Add(amountField);

            return root;
        }
    }
}
