using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace MonsterQuest
{
    public class Console : MonoBehaviour
    {
        private static Console _instance;
        private readonly Stack<VisualElement> _textGroups = new();

        private ScrollView _consolePanelScrollView;
        private UIDocument _document;

        private VisualElement currentTextGroup => _textGroups.Peek();

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            _consolePanelScrollView = _document.rootVisualElement.Q<ScrollView>("console-panel");

            _textGroups.Push(_consolePanelScrollView.contentContainer);

            _instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _consolePanelScrollView.style.visibility = _consolePanelScrollView.resolvedStyle.visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public static void Write(string text)
        {
            _instance.WriteInternal(text);
        }

        public static void WriteLine(string text)
        {
            _instance.WriteLineInternal(text);
        }

        public static void Indent(bool verbose)
        {
            _instance.IndentInternal(verbose);
        }

        public static void Outdent()
        {
            _instance.OutdentInternal();
        }

        private void WriteInternal(string text)
        {
            string[] lines = text.Split("\n");

            AppendText(lines[0]);

            for (int i = 1; i < lines.Length; i++)
            {
                AddText(lines[i]);
            }

            StartCoroutine(ScrollOnNextFrame());
        }

        private void WriteLineInternal(string text)
        {
            WriteInternal($"{text}\n");
        }

        private IEnumerator ScrollOnNextFrame()
        {
            yield return new WaitForEndOfFrame();
            _consolePanelScrollView.verticalScroller.value = Math.Max(0, _consolePanelScrollView.verticalScroller.highValue);
        }

        private void AppendText(string text)
        {
            if (currentTextGroup.Children().LastOrDefault() is not Label lastLabel)
            {
                lastLabel = new Label();
                lastLabel.AddToClassList("text");

                currentTextGroup.Add(lastLabel);
            }

            lastLabel.text += text;
        }

        private void AddText(string text)
        {
            Label label = new()
            {
                text = text
            };

            label.AddToClassList("text");

            currentTextGroup.Add(label);
        }

        private void IndentInternal(bool verbose)
        {
            VisualElement lastElement = currentTextGroup.Children().LastOrDefault();

            Label lastLabel = lastElement as Label;

            if (lastLabel?.text == "")
            {
                currentTextGroup.Remove(lastLabel);
                lastElement = currentTextGroup.Children().LastOrDefault();
                lastLabel = lastElement as Label;
            }

            Foldout foldout = lastElement as Foldout;

            if (foldout is null)
            {
                foldout = new Foldout
                {
                    text = lastLabel?.text,
                    value = !verbose
                };

                foldout.AddToClassList("text-group");
                if (verbose) foldout.AddToClassList("verbose");

                currentTextGroup.Add(foldout);

                if (lastLabel is not null)
                {
                    currentTextGroup.Remove(lastLabel);
                }
            }

            _textGroups.Push(foldout.contentContainer);
        }

        private void OutdentInternal()
        {
            Label lastLabel = currentTextGroup.Children().LastOrDefault() as Label;

            if (lastLabel?.text == "")
            {
                currentTextGroup.Remove(lastLabel);
            }

            _textGroups.Pop();
        }
    }
}
