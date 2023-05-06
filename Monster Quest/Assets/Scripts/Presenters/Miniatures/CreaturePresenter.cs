using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MonsterQuest.Presenters.Miniatures
{
    public partial class CreaturePresenter : MonoBehaviour
    {
        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private static readonly int _useHash = Animator.StringToHash("Use");
        private static readonly int _angleRatio = Shader.PropertyToID("_AngleRatio");

        [SerializeField] private GameObject[] stands;
        [SerializeField] private float attackedForce;
        [SerializeField] private float attackedForceInstantDeath;
        [SerializeField] private float attackedForceHeight;

        private Animator _animator;
        private Animator _bodyAnimator;
        private Animator _bodyVerticalDisplacementAnimator;
        private Animator _miniatureAnimator;
        private Animator _standAnimator;

        private AsyncOperationHandle<GameObject> _modelHandle;

        private bool _destroyed;

        private CombatPresenter _combatPresenter;

        private Coroutine _hitPointAnimationCoroutine;
        private Coroutine _resetMiniatureCoroutine;

        private Creature _creature;

        private FixedJoint _bodyFixedJoint;

        private float _currentHitPointRatio;

        private Material _material;

        private Rigidbody _bodyRigidBody;
        private Rigidbody _miniatureRigidBody;

        private SpriteRenderer _hitPointsSpriteRenderer;

        private Transform _bodyMeshTransform;
        private Transform _bodyTransform;
        private Transform _bodyVerticalDisplacementTransform;
        private Transform _hitPointsTransform;
        private Transform _miniatureTransform;
        private Transform _orientationTransform;
        private Transform _standMeshTransform;
        private Transform _standTransform;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _orientationTransform = transform.Find("Orientation");

            _miniatureTransform = _orientationTransform.Find("Miniature");
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

            _hitPointsTransform = transform.Find("Hit points");
            _hitPointsSpriteRenderer = _hitPointsTransform.GetComponent<SpriteRenderer>();
            _hitPointsSpriteRenderer.enabled = false;
        }

        private void OnDestroy()
        {
            Addressables.Release(_modelHandle);
        }

        private void EnablePhysics()
        {
            SetRigidBodiesIsKinematic(false);
            _miniatureAnimator.enabled = false;
        }

        private void DisablePhysics()
        {
            SetRigidBodiesIsKinematic(true);
            _resetMiniatureCoroutine = StartCoroutine(ResetMiniatureTransform(0.5f));
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
                // Set body model.
                Mesh bodyMesh = handle.Result.GetComponent<MeshFilter>().sharedMesh;
                _bodyMeshTransform.GetComponent<MeshFilter>().sharedMesh = bodyMesh;
                _bodyMeshTransform.GetComponent<MeshCollider>().sharedMesh = bodyMesh;
                _bodyMeshTransform.GetComponent<MeshRenderer>().material = _material;

                // Set stand model.
                GameObject stand = stands[(int)_creature.sizeCategory - 1];
                Mesh standMesh = stand.GetComponent<MeshFilter>().sharedMesh;

                _standMeshTransform.GetComponent<MeshFilter>().sharedMesh = standMesh;
                _standMeshTransform.GetComponent<MeshRenderer>().material = _material;

                _miniatureTransform.GetComponent<MeshCollider>().sharedMesh = standMesh;

                // Set initial hit points.
                SetHitPointRatio((float)_creature.hitPoints / _creature.hitPointsMaximum);
                _hitPointsSpriteRenderer.enabled = true;
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

            // Load creature model.
            BodyAsset bodyAsset = Assets.GetBodyAsset(creature.bodyAssetName);
            _modelHandle = Addressables.LoadAssetAsync<GameObject>(bodyAsset.modelReference);
            _modelHandle.Completed += OnBodyMeshLoaded;
        }

        private IEnumerator WaitForResetMiniature()
        {
            yield return _resetMiniatureCoroutine;
        }

        private IEnumerator ResetMiniatureTransform(float transitionDuration)
        {
            Vector3 startPosition = _miniatureTransform.localPosition;
            Quaternion startRotation = _miniatureTransform.localRotation;
            float startTime = Time.time;

            float transitionProgress;

            do
            {
                transitionProgress = transitionDuration > 0 ? (Time.time - startTime) / transitionDuration : 1;

                // Smoothly interpolate to identity.
                _miniatureTransform.localPosition = Vector3.Slerp(startPosition, Vector3.zero, transitionProgress);
                _miniatureTransform.localRotation = Quaternion.Slerp(startRotation, Quaternion.identity, transitionProgress);

                yield return null;
            } while (transitionProgress < 1);

            _resetMiniatureCoroutine = null;
            _miniatureAnimator.enabled = true;
        }
    }
}
