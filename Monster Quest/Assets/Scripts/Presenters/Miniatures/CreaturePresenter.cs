using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MonsterQuest.Presenters.Miniatures
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

        [SerializeField] private GameObject[] stands;

        private Animator _animator;
        private Animator _bodySpriteAnimator;
        private Animator _bodyVerticalDisplacementAnimator;
        private Animator _standAnimator;

        private AsyncOperationHandle<GameObject> _modelHandle;

        private bool _destroyed;

        private bool _standing;

        private CombatPresenter _combatPresenter;

        private Creature _creature;

        private float _currentHitPointRatio;
        private IEnumerator _hitPointAnimationCoroutine;
        private Material _material;
        private Transform _bodyMeshTransform;

        private Transform _bodyTransform;
        private Transform _bodyVerticalDisplacementTransform;
        private Transform _deathSavingThrowsTransform;
        private Transform _hitPointsMaskTransform;
        private Transform _standMeshTransform;
        private Transform _standTransform;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _bodyTransform = transform.Find("Body");

            _bodyVerticalDisplacementTransform = _bodyTransform.Find("Vertical displacement");
            _bodyVerticalDisplacementAnimator = _bodyVerticalDisplacementTransform.GetComponent<Animator>();

            _bodyMeshTransform = _bodyVerticalDisplacementTransform.Find("Mesh");

            _standTransform = transform.Find("Stand");
            _standMeshTransform = _standTransform.Find("Mesh");
        }

        private void OnDestroy()
        {
            Addressables.Release(_modelHandle);
        }

        private void OnBodyMeshLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _bodyMeshTransform.GetComponent<MeshFilter>().sharedMesh = handle.Result.GetComponent<MeshFilter>().sharedMesh;
                _bodyMeshTransform.GetComponent<MeshRenderer>().material = _material;
            }
            else
            {
                Debug.LogError("Creature body model failed to load.");
            }
        }

        public void Initialize(CombatPresenter combatPresenter, Creature creature, Material material)
        {
            _combatPresenter = combatPresenter;
            _creature = creature;
            _material = material;

            // Add stand model.
            GameObject stand = stands[(int)_creature.sizeCategory - 1];
            _standMeshTransform.GetComponent<MeshFilter>().sharedMesh = stand.GetComponent<MeshFilter>().sharedMesh;
            _standMeshTransform.GetComponent<MeshRenderer>().material = _material;

            // Load creature model.
            BodyAsset bodyAsset = Assets.GetBodyAsset(creature.bodyAssetName);
            _modelHandle = Addressables.LoadAssetAsync<GameObject>(bodyAsset.modelReference);
            _modelHandle.Completed += OnBodyMeshLoaded;
        }
    }
}
