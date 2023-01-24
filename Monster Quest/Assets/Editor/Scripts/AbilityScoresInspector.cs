using UnityEditor;
using UnityEngine.UIElements;

namespace MonsterQuest.Editor
{
    [CustomPropertyDrawer(typeof(AbilityScores))]
    public class AbilityScoresInspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualTreeAsset layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UXML/AbilityScores.uxml");

            return layout.Instantiate();
        }
    }
}
