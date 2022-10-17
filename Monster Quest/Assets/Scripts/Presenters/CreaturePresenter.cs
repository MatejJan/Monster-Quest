using System.Collections;
using UnityEngine;

namespace MonsterQuest
{
    public class CreaturePresenter : MonoBehaviour
    {
        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private static readonly int _takeDamageHash = Animator.StringToHash("Take damage");
        private static readonly int _dieHash = Animator.StringToHash("Die");
        private static readonly int _liveHash = Animator.StringToHash("Live");
        private static readonly int _flyHash = Animator.StringToHash("Fly");

        private Animator _bodySpriteAnimator;
        private Animator _bodyVerticalDisplacementAnimator;

        private Creature _creature;
        private Transform _bodyOrientationTransform;

        private Transform _bodySpriteTransform;
        private Transform _bodyTransform;
        private Transform _bodyVerticalDisplacementTransform;
        private Transform _standTransform;

        private void Awake()
        {
            _bodyTransform = transform.Find("Body");

            _bodyVerticalDisplacementTransform = _bodyTransform.Find("Vertical displacement");
            _bodyVerticalDisplacementAnimator = _bodyVerticalDisplacementTransform.GetComponent<Animator>();

            _bodyOrientationTransform = _bodyVerticalDisplacementTransform.Find("Orientation");

            _bodySpriteTransform = _bodyOrientationTransform.Find("Sprite");
            _bodySpriteAnimator = _bodySpriteTransform.GetComponent<Animator>();

            _standTransform = transform.Find("Stand");
        }

        public void Initialize(Creature creature)
        {
            _creature = creature;
            _creature.InitializePresenter(this);

            SpriteRenderer bodySpriteRenderer = _bodySpriteTransform.GetComponent<SpriteRenderer>();
            bodySpriteRenderer.sprite = _creature.bodySprite;

            SpriteRenderer standSpriteRenderer = _standTransform.GetComponent<SpriteRenderer>();
            standSpriteRenderer.sprite = GameManager.database.standSprites[(int)_creature.size];

            FlyIfPossible(false);
        }

        public void FaceDirection(CardinalDirection direction)
        {
            // Get the angle we need to rotate by.
            float angleDegrees = CardinalDirectionHelper.cardinalDirectionRotationsDegrees[direction];

            // Account for sprites being positioned in the south direction.
            angleDegrees -= CardinalDirectionHelper.cardinalDirectionRotationsDegrees[CardinalDirection.South];

            // Rotate the body orientation and stand.
            _bodyOrientationTransform.localRotation = Quaternion.Euler(0, 0, angleDegrees);
            _standTransform.localRotation = _bodyOrientationTransform.localRotation;
        }

        public IEnumerator Attack()
        {
            // Trigger the attack animation.
            _bodySpriteAnimator.SetTrigger(_attackHash);

            yield return new WaitForSeconds(15f / 60f);
        }

        public IEnumerator TakeDamage()
        {
            // Trigger the take damage animation.
            _bodySpriteAnimator.SetTrigger(_takeDamageHash);

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator FallUnconscious()
        {
            // Trigger the die animation.
            _bodySpriteAnimator.SetTrigger(_dieHash);

            yield return new WaitForSeconds(1.9f);
        }

        public IEnumerator RegainConsciousness()
        {
            // Trigger the live animation.
            _bodySpriteAnimator.SetTrigger(_liveHash);

            // Start flying again.
            FlyIfPossible();

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator Die()
        {
            // Trigger the die animation.
            _bodySpriteAnimator.SetTrigger(_dieHash);

            // Stop flying.
            StopFlying();

            yield return new WaitForSeconds(2f);

            Destroy(gameObject);

            yield return new WaitForSeconds(0.5f);
        }

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
