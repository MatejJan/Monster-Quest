using MonsterQuest.Presenters.Narrative;
using TMPro;
using UnityEngine;

namespace MonsterQuest.Presenters.Drawing
{
    public class NarrativePresenter : MonoBehaviour, IOutput
    {
        private TextMeshProUGUI _textMeshProUGui;

        private void Awake()
        {
            _textMeshProUGui = GetComponent<TextMeshProUGUI>();
        }

        public void WriteLine(string text)
        {
            _textMeshProUGui.text += $"{text}\n";

            Console.WriteLine(text);
        }
    }
}
