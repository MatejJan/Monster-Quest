using System.Collections;
using UnityEngine;

namespace MonsterQuest
{
    public class CreaturePresenter : MonoBehaviour
    {
        private const float _creatureUnitScale = 0.8f;

        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private static readonly int _attackedHash = Animator.StringToHash("Attacked");
        private static readonly int _attackedToLyingHash = Animator.StringToHash("Attacked to lying");
        private static readonly int _attackedToInstantDeathHash = Animator.StringToHash("Attacked to instant death");
        private static readonly int _standHash = Animator.StringToHash("Stand");
        private static readonly int _flyHash = Animator.StringToHash("Fly");
        private static readonly int _lyingStateHash = Animator.StringToHash("Attacked standing to lying");

        private static readonly int _stableHash = Animator.StringToHash("Stable");
        private static readonly int _deathSavingThrowFailuresHash = Animator.StringToHash("Death saving throw failures");

        [SerializeField] private Color damagedColor;
        [SerializeField] private Sprite[] standSprites;

        private Animator _bodySpriteAnimator;
        private Animator _bodyVerticalDisplacementAnimator;
        private Animator _standAnimator;

        private bool _destroyed;

        private bool _standing;

        private Creature _creature;

        private DeathSavingThrowsPresenter _deathSavingThrowsPresenter;
        private DicePresenter _dicePresenter;

        private float _currentHitPointRatio;
        private IEnumerator _hitPointAnimationCoroutine;

        private SpriteRenderer _bodySpriteRenderer;

        private Transform _bodyOrientationTransform;
        private Transform _bodySpriteTransform;
        private Transform _bodyTransform;
        private Transform _bodyVerticalDisplacementTransform;
        private Transform _deathSavingThrowsTransform;
        private Transform _hitPointsMaskTransform;
        private Transform _standBaseTransform;

        private void Awake()
        {
            _bodyTransform = transform.Find("Body");

            _bodyVerticalDisplacementTransform = _bodyTransform.Find("Vertical displacement");
            _bodyVerticalDisplacementAnimator = _bodyVerticalDisplacementTransform.GetComponent<Animator>();

            _bodyOrientationTransform = _bodyVerticalDisplacementTransform.Find("Orientation");

            _bodySpriteTransform = _bodyOrientationTransform.Find("Sprite");
            _bodySpriteAnimator = _bodySpriteTransform.GetComponent<Animator>();

            Transform standTransform = transform.Find("Stand");
            _standAnimator = standTransform.GetComponent<Animator>();

            _standBaseTransform = standTransform.Find("Base");
            _hitPointsMaskTransform = standTransform.Find("Hit points mask");

            _deathSavingThrowsTransform = transform.Find("UI").Find("Death saving throws");
            _deathSavingThrowsPresenter = _deathSavingThrowsTransform.GetComponent<DeathSavingThrowsPresenter>();

            _dicePresenter = GameObject.Find("Dice").GetComponent<DicePresenter>();
        }

        public void Initialize(Creature creature)
        {
            // Connect model and presenter.
            _creature = creature;
            _creature.InitializePresenter(this);

            // Set body sprite.
            _bodySpriteRenderer = _bodySpriteTransform.GetComponent<SpriteRenderer>();
            _bodySpriteRenderer.sprite = _creature.bodySprite;

            // Set stand sprite.
            SpriteRenderer[] standSpriteRenderers = _standBaseTransform.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer standSpriteRenderer in standSpriteRenderers)
            {
                standSpriteRenderer.sprite = standSprites[(int)_creature.sizeCategory - 1];
            }

            // Animate flying if needed.
            if (_creature.lifeStatus == LifeStatus.Alive) FlyIfPossible(false);

            // Set initial hit points.
            SetHitPointRatio((float)creature.hitPoints / creature.hitPointsMaximum);

            // Set initial standing/lying state.
            _standing = creature.hitPoints > 0;

            if (!_standing)
            {
                _bodySpriteAnimator.Play(_lyingStateHash, 0, 1);
            }

            // Set initial stable state.
            UpdateStableStatus();

            // Position the death saving throws.
            float offset = _creature.spaceInFeet * 0.5f + 1.5f;
            _deathSavingThrowsTransform.localPosition = new Vector3(0, offset, 0);

            // Set initial death saving throws.
            UpdateDeathSavingThrowFailures();

            foreach (bool deathSavingThrow in _creature.deathSavingThrows)
            {
                _deathSavingThrowsPresenter.AddDeathSavingThrow(deathSavingThrow);
            }
        }

        public IEnumerator FaceDirection(CardinalDirection direction, bool immediate = false)
        {
            // Get the angle we need to rotate by.
            float angleDegrees = CardinalDirectionHelper.cardinalDirectionRotationsDegrees[direction];

            yield return FaceAngle(angleDegrees, immediate);
        }

        public IEnumerator FaceCreature(Creature creature, bool immediate = false)
        {
            // Make sure the creature is not destroyed.
            if (creature == null || creature.presenter == null) yield break;

            // Get the angle we need to rotate by.
            Vector3 directionToCreature = creature.presenter.transform.position - transform.position;
            float angleDegrees = Mathf.Atan2(directionToCreature.y, directionToCreature.x) * Mathf.Rad2Deg;

            yield return FaceAngle(angleDegrees, immediate);
        }

        private IEnumerator FaceAngle(float angleDegrees, bool immediate)
        {
            // Account for sprites being positioned in the south direction.
            angleDegrees -= CardinalDirectionHelper.cardinalDirectionRotationsDegrees[CardinalDirection.South];

            if (immediate)
            {
                SetLocalRotation(angleDegrees);

                yield break;
            }

            yield return StartCoroutine(AnimateLocalRotation(angleDegrees));
        }

        private IEnumerator AnimateLocalRotation(float angleDegrees)
        {
            float startAngleDegrees = _bodyOrientationTransform.localRotation.eulerAngles.z;
            float deltaAngle = Mathf.DeltaAngle(startAngleDegrees, angleDegrees);
            angleDegrees = startAngleDegrees + deltaAngle;

            float startTime = Time.time;

            float transitionDuration = Mathf.Abs(angleDegrees - startAngleDegrees) * 0.01f;
            float transitionProgress;

            do
            {
                if (_destroyed) yield break;

                transitionProgress = transitionDuration > 0 ? (Time.time - startTime) / transitionDuration : 1;

                // Smoothly interpolate to desired height.
                float currentAngleDegrees = Mathf.SmoothStep(startAngleDegrees, angleDegrees, transitionProgress);
                SetLocalRotation(currentAngleDegrees);

                yield return null;
            } while (transitionProgress < 1);
        }

        private void SetLocalRotation(float angleDegrees)
        {
            // Rotate the body orientation.
            _bodyOrientationTransform.localRotation = Quaternion.Euler(0, 0, angleDegrees);
        }

        public IEnumerator Attack()
        {
            // Trigger the attack animation.
            _bodySpriteAnimator.SetTrigger(_attackHash);

            yield return new WaitForSeconds(15f / 60f);
        }

        public IEnumerator GetAttacked(bool instantDeath = false)
        {
            // Update hit points indicator.
            UpdateHitPoints();

            if (_standing && _creature.hitPoints == 0)
            {
                if (instantDeath)
                {
                    // The more extreme version of dying should play.
                    _bodySpriteAnimator.SetTrigger(_attackedToInstantDeathHash);
                }
                else
                {
                    // The creature should get attacked and end up lying down.
                    _bodySpriteAnimator.SetTrigger(_attackedToLyingHash);
                }

                _standing = false;

                // Stop flying.
                StopFlying();

                yield return new WaitForSeconds(2f);
            }
            else
            {
                // The creature gets attacked in its current state.
                _bodySpriteAnimator.SetTrigger(_attackedHash);
            }

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator Heal()
        {
            // Update hit points indicator.
            UpdateHitPoints();

            yield break;
        }

        public void UpdateStableStatus()
        {
            _standAnimator.SetBool(_stableHash, _creature.lifeStatus != LifeStatus.UnstableUnconscious);
        }

        public IEnumerator RegainConsciousness()
        {
            // The creature should stand.
            _bodySpriteAnimator.SetTrigger(_standHash);
            _standing = true;

            // Start flying again.
            FlyIfPossible();

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator Die()
        {
            Destroy(gameObject);
            _destroyed = true;

            yield return new WaitForSeconds(0.5f);
        }

        public IEnumerator PerformDeathSavingThrow(bool success, int? rollResult = null)
        {
            Vector3 deathSavingThrowPosition = _deathSavingThrowsPresenter.StartDeathSavingThrow();

            if (rollResult is not null)
            {
                yield return _dicePresenter.RollD20(rollResult.Value, deathSavingThrowPosition + Vector3.right * 2.5f);
            }

            yield return _deathSavingThrowsPresenter.EndDeathSavingThrow(success);

            _dicePresenter.EndRoll();
            UpdateDeathSavingThrowFailures();
        }

        public void ResetDeathSavingThrows()
        {
            _deathSavingThrowsPresenter.Reset();
            UpdateDeathSavingThrowFailures();
        }

        private void UpdateDeathSavingThrowFailures()
        {
            _standAnimator.SetInteger(_deathSavingThrowFailuresHash, _creature.deathSavingThrowFailures);
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

        private void SetHitPointRatio(float ratio)
        {
            float spaceInFeet = _creature.spaceInFeet * _creatureUnitScale;
            _hitPointsMaskTransform.localPosition = new Vector3(0, -spaceInFeet / 2, 0);
            _hitPointsMaskTransform.localScale = new Vector3(spaceInFeet, spaceInFeet * ratio, 1);

            _bodySpriteRenderer.color = Color.Lerp(damagedColor, Color.white, ratio);

            _currentHitPointRatio = ratio;
        }

        private void UpdateHitPoints()
        {
            AnimateToHitPointRatio((float)_creature.hitPoints / _creature.hitPointsMaximum);
        }

        private void AnimateToHitPointRatio(float ratio)
        {
            if (_hitPointAnimationCoroutine != null)
            {
                StopCoroutine(_hitPointAnimationCoroutine);
            }

            _hitPointAnimationCoroutine = AnimateToHitPointRatioCoroutine(ratio, 0.5f);
            StartCoroutine(_hitPointAnimationCoroutine);
        }

        private IEnumerator AnimateToHitPointRatioCoroutine(float endRatio, float transitionDuration)
        {
            float startRatio = _currentHitPointRatio;
            float startTime = Time.time;

            float transitionProgress;

            do
            {
                transitionProgress = transitionDuration > 0 ? (Time.time - startTime) / transitionDuration : 1;

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
