using System.Collections;
using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public partial class CreaturePresenter
    {
        // Public methods

        public void UpdateStableStatus()
        {
            //_standAnimator.SetBool(_stableHash, _creature.lifeStatus != LifeStatus.UnconsciousUnstable);
        }

        public IEnumerator Heal()
        {
            yield return WaitForResetMiniature();

            // Trigger the use animation.
            _bodyVerticalDisplacementAnimator.SetTrigger(_useHash);

            // Update hit points indicator.
            UpdateHitPoints();

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator RegainConsciousness()
        {
            // The creature should stand.
            //_bodySpriteAnimator.SetTrigger(_standHash);

            // Start flying again.
            //FlyIfPossible();

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator Die()
        {
            //_animator.SetTrigger(_dieHash);

            if (_resetMiniatureCoroutine is not null) StopCoroutine(_resetMiniatureCoroutine);
            EnablePhysics();

            yield return new WaitForSeconds(5);

            Destroy(gameObject);
            _destroyed = true;

            destroyed?.Invoke();
        }
    }
}
