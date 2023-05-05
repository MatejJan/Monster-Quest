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
            // Update hit points indicator.
            //UpdateHitPoints();

            yield break;
        }

        public IEnumerator RegainConsciousness()
        {
            // The creature should stand.
            //_bodySpriteAnimator.SetTrigger(_standHash);
            _standing = true;

            // Start flying again.
            //FlyIfPossible();

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator Die()
        {
            //_animator.SetTrigger(_dieHash);

            EnablePhysics();

            yield return new WaitForSeconds(5);

            Destroy(gameObject);
            _destroyed = true;

            destroyed?.Invoke();
        }
    }
}
