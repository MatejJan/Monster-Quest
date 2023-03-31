using System.Collections;
using UnityEngine;

namespace MonsterQuest.Presenters.Drawing
{
    public partial class CreaturePresenter
    {
        // Private methods

        private void FlyIfPossible(bool transition = true)
        {
            _bodyVerticalDisplacementAnimator.SetBool(_flyHash, _creature.flyHeight > 0);

            // Move to creature to flying height if needed.
            if (_creature.flyHeight == 0) return;

            StartCoroutine(FlyToHeight(_creature.flyHeight, transition ? 1 : 0));
        }

        private void StopFlying()
        {
            // Nothing to do if we weren't flying.
            if (_bodyTransform.localPosition.y == 0) return;

            // Stop flying.
            _bodyVerticalDisplacementAnimator.SetBool(_flyHash, false);

            // Return to ground.
            StartCoroutine(FlyToHeight(0, 0.5f));
        }

        private IEnumerator FlyToHeight(float height, float transitionDuration)
        {
            float startHeight = _bodyTransform.localPosition.y;
            float startTime = Time.time;

            float transitionProgress;

            do
            {
                transitionProgress = transitionDuration > 0 ? (Time.time - startTime) / transitionDuration : 1;

                // Smoothly interpolate to desired height.
                Vector3 position = _bodyTransform.localPosition;
                position.y = Mathf.SmoothStep(startHeight, height, transitionProgress);

                // Raise the creature to be sorted before lower creates.
                position.z = -position.y;

                _bodyTransform.localPosition = position;

                yield return null;
            } while (transitionProgress < 1);
        }
    }
}
