using System.Collections;
using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public partial class CreaturePresenter
    {
        // Private methods

        private void SetHitPointRatio(float ratio)
        {
            _hitPointsSpriteRenderer.material.SetFloat(_angleRatio, ratio);
            _currentHitPointRatio = ratio;
        }

        private void UpdateHitPoints()
        {
            AnimateToHitPointRatio(Mathf.Max(0f, _creature.hitPoints) / _creature.hitPointsMaximum);
        }

        private void AnimateToHitPointRatio(float ratio)
        {
            if (_hitPointAnimationCoroutine != null)
            {
                StopCoroutine(_hitPointAnimationCoroutine);
            }

            _hitPointAnimationCoroutine = StartCoroutine(AnimateToHitPointRatioCoroutine(ratio, 0.5f));
        }

        private IEnumerator AnimateToHitPointRatioCoroutine(float endRatio, float transitionDuration)
        {
            float startRatio = _currentHitPointRatio;
            float startTime = Time.time;

            float transitionProgress;

            do
            {
                transitionProgress = Mathf.Clamp01(transitionDuration > 0 ? (Time.time - startTime) / transitionDuration : 1);

                // Ease out to desired ratio.
                float easedTransitionProgress = Mathf.Sin(transitionProgress * Mathf.PI / 2);
                float newRatio = Mathf.Lerp(startRatio, endRatio, easedTransitionProgress);
                SetHitPointRatio(newRatio);

                yield return null;
            } while (transitionProgress < 1);

            _hitPointAnimationCoroutine = null;
        }
    }
}
