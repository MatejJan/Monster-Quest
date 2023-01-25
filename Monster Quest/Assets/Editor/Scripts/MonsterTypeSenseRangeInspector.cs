using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomPropertyDrawer(typeof(MonsterType.SenseRange))]
    public class MonsterTypeSenseRangeInspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.AddToClassList("property-list");

            EnumField senseField = new();
            senseField.bindingPath = "sense";
            senseField.AddToClassList("property-name");
            root.Add(senseField);

            IntegerField rangeField = new();
            rangeField.bindingPath = "range";
            rangeField.AddToClassList("property-value");
            root.Add(rangeField);

            return root;
        }
    }
}
