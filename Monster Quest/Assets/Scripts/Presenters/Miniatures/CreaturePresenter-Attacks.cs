using System.Collections;
using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public partial class CreaturePresenter
    {
        // Public methods

        public IEnumerator Attack()
        {
            yield return WaitForResetMiniature();

            // Trigger the attack animation.
            _miniatureAnimator.SetTrigger(_attackHash);

            yield return new WaitForSeconds(15f / 60f);
        }

        public IEnumerator GetAttacked(Vector3 sourcePosition, bool knockedOut = false, bool instantDeath = false)
        {
            // Update hit points indicator.
            UpdateHitPoints();

            // Move the miniature with force.
            EnablePhysics();

            if (knockedOut)
            {
                Destroy(_bodyFixedJoint);
            }

            Vector3 position = transform.position;
            Vector3 forceDirection = (position - sourcePosition).normalized;
            Vector3 forcePosition = position + Vector3.up * attackedForceHeight;

            _bodyRigidBody.AddForceAtPosition(forceDirection * (instantDeath ? attackedForceInstantDeath : attackedForce), forcePosition);

            yield return new WaitForSeconds(1);

            DisablePhysics();
        }
    }
}
