using System;
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
        [SerializeField] private float attackedForce;
        [SerializeField] private float attackedForceInstantDeath;

        private Animator _animator;
        private Animator _bodyAnimator;
        private Animator _bodyVerticalDisplacementAnimator;
        private Animator _miniatureAnimator;
        private Animator _standAnimator;

        private AsyncOperationHandle<GameObject> _modelHandle;

        private bool _destroyed;

        private bool _standing;

        private CombatPresenter _combatPresenter;

        private Creature _creature;

        private FixedJoint _bodyFixedJoint;

        private float _currentHitPointRatio;
        private IEnumerator _hitPointAnimationCoroutine;
        private Material _material;
        private Rigidbody _bodyRigidBody;
        private Rigidbody _miniatureRigidBody;
        private Transform _bodyMeshTransform;
        private Transform _bodyTransform;
        private Transform _bodyVerticalDisplacementTransform;
        private Transform _deathSavingThrowsTransform;
        private Transform _hitPointsMaskTransform;

        private Transform _miniatureTransform;
        private Transform _standMeshTransform;
        private Transform _standTransform;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _miniatureTransform = transform.Find("Miniature");
            _miniatureAnimator = _miniatureTransform.GetComponent<Animator>();

            _bodyTransform = _miniatureTransform.Find("Body");

            _bodyVerticalDisplacementTransform = _bodyTransform.Find("Vertical displacement");
            _bodyVerticalDisplacementAnimator = _bodyVerticalDisplacementTransform.GetComponent<Animator>();

            _bodyMeshTransform = _bodyVerticalDisplacementTransform.Find("Mesh");

            _standTransform = _miniatureTransform.Find("Stand");
            _standMeshTransform = _standTransform.Find("Mesh");

            _bodyFixedJoint = _miniatureTransform.GetComponent<FixedJoint>();
            _bodyRigidBody = _bodyMeshTransform.GetComponent<Rigidbody>();
            _miniatureRigidBody = _miniatureTransform.GetComponent<Rigidbody>();
        }

        private void OnDestroy()
        {
            Addressables.Release(_modelHandle);
        }

        private void EnablePhysics()
        {
            SetRigidBodiesIsKinematic(false);
        }

        private void DisablePhysics()
        {
            SetRigidBodiesIsKinematic(true);
        }

        private void SetRigidBodiesIsKinematic(bool value)
        {
            _bodyRigidBody.isKinematic = value;
            _miniatureRigidBody.isKinematic = value;
        }

        private void OnBodyMeshLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Mesh bodyMesh = handle.Result.GetComponent<MeshFilter>().sharedMesh;
                _bodyMeshTransform.GetComponent<MeshFilter>().sharedMesh = bodyMesh;
                _bodyMeshTransform.GetComponent<MeshCollider>().sharedMesh = bodyMesh;
                _bodyMeshTransform.GetComponent<MeshRenderer>().material = _material;
            }
            else
            {
                Debug.LogError("Creature body model failed to load.");
            }
        }

        public event Action destroyed;

        public void Initialize(CombatPresenter combatPresenter, Creature creature, Material material)
        {
            _combatPresenter = combatPresenter;
            _creature = creature;
            _material = material;

            // Add stand model.
            GameObject stand = stands[(int)_creature.sizeCategory - 1];
            Mesh standMesh = stand.GetComponent<MeshFilter>().sharedMesh;

            _standMeshTransform.GetComponent<MeshFilter>().sharedMesh = standMesh;
            _standMeshTransform.GetComponent<MeshRenderer>().material = _material;

            _miniatureTransform.GetComponent<MeshCollider>().sharedMesh = standMesh;

            // Load creature model.
            BodyAsset bodyAsset = Assets.GetBodyAsset(creature.bodyAssetName);
            _modelHandle = Addressables.LoadAssetAsync<GameObject>(bodyAsset.modelReference);
            _modelHandle.Completed += OnBodyMeshLoaded;
        }
    }
}
