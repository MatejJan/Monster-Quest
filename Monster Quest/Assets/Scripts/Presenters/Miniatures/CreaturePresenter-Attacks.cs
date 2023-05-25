using System;
using System.Collections;
using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public partial class CreaturePresenter
    {
        // Public methods

        public IEnumerator Attack()
        {
            yield return WaitForResetStandingMiniature();

            // Trigger the attack animation.
            _miniatureAnimator.SetTrigger(_attackHash);

            yield return new WaitForSeconds(15f / 60f);
        }

        public IEnumerator GetAttacked(int hitPointsEnd, Vector3 sourcePosition, bool knockedOut, bool instantDeath)
        {
            // Cancel any resetting animations.
            CancelResetStandingMiniature();

            // Update hit points indicator.
            UpdateHitPoints(hitPointsEnd);

            // Move the miniature with force.
            float forceAmount = attackedForce;

            if (_standing)
            {
                if (knockedOut)
                {
                    forceAmount = attackedForceKnockedOut;
                    BreakDown();
                }

                if (instantDeath)
                {
                    forceAmount = attackedForceInstantDeath;
                    Destroy(_configurableJoint);
                }

                EnableStandingMiniaturePhysics();
            }
            else
            {
                attackedForce = attackedForceOnKnockedOut;
            }

            Vector3 position = transform.position;
            Vector3 forceDirection = (position - sourcePosition).normalized;
            Vector3 forcePosition = position + Vector3.up * Math.Max(attackedForceHeight, _bodyAsset.verticalExtensionHeight);

            _bodyMeshRigidBody.AddForceAtPosition(forceDirection * forceAmount, forcePosition);

            // Let physics do its animation before proceeding.
            yield return new WaitForSeconds(1);

            if (_standing)
            {
                // Standing miniatures should reset to be ready for animations.
                ResetStandingMiniature();
            }
        }
    }
}
