using System.Collections;
using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public partial class CreaturePresenter
    {
        // Public methods

        public IEnumerator Heal(float hitPointsEnd, bool consciousAction)
        {
            yield return WaitForResetStandingMiniature();

            // Update hit points indicator.
            UpdateHitPoints(hitPointsEnd);

            if (consciousAction)
            {
                // Trigger the use animation.
                _bodyVerticalDisplacementAnimator.SetTrigger(_useHash);

                yield return new WaitForSeconds(1f);
            }
        }

        public IEnumerator FallUnconscious()
        {
            // Give 2 extra seconds for the physics engine to animate falling.
            yield return new WaitForSeconds(2);
        }

        public IEnumerator RegainConsciousness()
        {
            yield return StandUp();
        }

        public IEnumerator Die()
        {
            // Give 3 extra seconds for the physics engine to animate falling without the distance constraint.
            if (_configurableJoint is not null) Destroy(_configurableJoint);

            yield return new WaitForSeconds(3);

            Destroy(gameObject);
            _destroyed = true;

            destroyed?.Invoke();
        }
    }
}
