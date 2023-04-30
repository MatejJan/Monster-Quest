using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MonsterQuest.Presenters.Drawing
{
    public partial class CreaturePresenter : MonoBehaviour
    {
        private const float _creatureUnitScale = 0.8f;

        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private static readonly int _attackedHash = Animator.StringToHash("Attacked");
        private static readonly int _attackedToLyingHash = Animator.StringToHash("Attacked to lying");
        private static readonly int _attackedToInstantDeathHash = Animator.StringToHash("Attacked to instant death");
        private static readonly int _standHash = Animator.StringToHash("Stand");
        private static readonly int _flyHash = Animator.StringToHash("Fly");
        private static readonly int _lyingStateHash = Animator.StringToHash("Attacked standing to lying");
        private static readonly int _dieHash = Animator.StringToHash("Die");
        private static readonly int _stableHash = Animator.StringToHash("Stable");
        private static readonly int _deathSavingThrowFailuresHash = Animator.StringToHash("Death saving throw failures");
        private static readonly int _celebrateHash = Animator.StringToHash("Celebrate");
        private static readonly int _levelUpHash = Animator.StringToHash("Level up");

        [SerializeField] private Color damagedColor;
        [SerializeField] private Sprite[] standSprites;

        private Animator _animator;
        private Animator _bodySpriteAnimator;
        private Animator _bodyVerticalDisplacementAnimator;
        private Animator _standAnimator;

        private AsyncOperationHandle<Sprite> _bodySpriteHandle;

        private bool _destroyed;

        private bool _standing;

        private CombatPresenter _combatPresenter;

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
            _animator = GetComponent<Animator>();

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

        private void OnDestroy()
        {
            Addressables.Release(_bodySpriteHandle);
        }

        public event Action destroyed;

        public void Initialize(CombatPresenter combatPresenter, Creature creature)
        {
            _combatPresenter = combatPresenter;
            _creature = creature;

            // Set stand sprite.
            SpriteRenderer[] standSpriteRenderers = _standBaseTransform.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer standSpriteRenderer in standSpriteRenderers)
            {
                standSpriteRenderer.sprite = standSprites[(int)_creature.sizeCategory - 1];
            }

            // Load body sprite.
            BodyAsset bodyAsset = Assets.GetBodyAsset(creature.bodyAssetName);
            _bodySpriteHandle = Addressables.LoadAssetAsync<Sprite>(bodyAsset.spriteReference);
            _bodySpriteHandle.Completed += OnBodySpriteLoaded;

            _bodySpriteRenderer = _bodySpriteTransform.GetComponent<SpriteRenderer>();

            // Animate flying if needed.
            if (_creature.lifeStatus == LifeStatus.Conscious) FlyIfPossible(false);

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

        // Instantiate the loaded prefab on complete
        private void OnBodySpriteLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // Set body sprite.
                _bodySpriteRenderer.sprite = handle.Result;
            }
            else
            {
                Debug.LogError("Creature body sprite failed to load.");
            }
        }
    }
}
