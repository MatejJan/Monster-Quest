using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MonsterQuest
{
    public class DiceRollPresenter : MonoBehaviour
    {
        private const float _rollDuration = 1.5f;
        private const int _rollRotations = 3;
        private static readonly Color _clearColor = new(1, 1, 1, 0);

        [SerializeField] private int diceSides;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public IEnumerator Roll(int result)
        {
            // Set numbers on dice sides.
            List<int> displayedNumbers = new();

            for (int diceSideIndex = 0; diceSideIndex < transform.childCount; diceSideIndex++)
            {
                int number;

                if (diceSideIndex == 0)
                {
                    number = result;
                }
                else
                {
                    do
                    {
                        number = Random.Range(1, diceSides + 1);
                    } while (displayedNumbers.Contains(number));
                }

                transform.GetChild(diceSideIndex).GetComponent<TextMeshPro>().text = number.ToString();

                displayedNumbers.Add(number);
            }

            return Rotate();
        }

        private IEnumerator Rotate()
        {
            float startTime = Time.time;
            float transitionProgress;

            do
            {
                transitionProgress = (Time.time - startTime) / _rollDuration;

                // Ease out to desired ratio.
                float easedTransitionProgress = Mathf.Sin(transitionProgress * Mathf.PI / 2);
                float angleDegrees = Mathf.Lerp(0, _rollRotations * 360, easedTransitionProgress);

                transform.localRotation = Quaternion.Euler(0, 0, angleDegrees);
                _spriteRenderer.color = Color.Lerp(_clearColor, Color.white, transitionProgress);

                yield return null;
            } while (transitionProgress < 1);
        }
    }
}
