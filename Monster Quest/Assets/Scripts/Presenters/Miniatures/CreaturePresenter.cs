using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace MonsterQuest.Presenters.Miniatures
{
    public partial class CreaturePresenter : MonoBehaviour
    {
        public enum ModelQuality
        {
            Low,
            High
        }

        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private static readonly int _useHash = Animator.StringToHash("Use");
        private static readonly int _angleRatio = Shader.PropertyToID("_AngleRatio");

        [SerializeField] private GameObject[] stands;
        [SerializeField] private GameObject verticalExtension5;
        [SerializeField] private float attackedForce;
        [SerializeField] private float attackedForceKnockedOut;
        [SerializeField] private float attackedForceInstantDeath;
        [SerializeField] private float attackedForceOnKnockedOut;
        [SerializeField] private float attackedForceHeight;
        private readonly List<Coroutine> _resetStandingMiniatureCoroutines = new();

        private Animator _animator;
        private Animator _bodyAnimator;
        private Animator _bodyVerticalDisplacementAnimator;
        private Animator _miniatureAnimator;
        private Animator _standAnimator;
        private AsyncOperationHandle<GameObject> _bodyConvexColliderModelHandle;

        private AsyncOperationHandle<GameObject> _modelHandle;

        private BodyAsset _bodyAsset;

        private bool _destroyed;
        private bool _standing;

        private CombatPresenter _combatPresenter;
        private ConfigurableJoint _configurableJoint;

        private Coroutine _hitPointAnimationCoroutine;

        private Creature _creature;

        private FixedJoint _bodyFixedJoint;

        private float _currentHitPointRatio;

        private Material _material;

        private Rigidbody _bodyMeshRigidBody;
        private Rigidbody _standRigidBody;

        private SpriteRenderer _hitPointsSpriteRenderer;

        private Transform _bodyMeshTransform;
        private Transform _bodyTransform;
        private Transform _bodyVerticalDisplacementTransform;
        private Transform _hitPointsTransform;
        private Transform _miniatureTransform;
        private Transform _orientationTransform;
        private Transform _standMeshTransform;
        private Transform _standTransform;
        private Transform _standVerticalExtensionMeshTransform;
        private Transform _visibilityTransform;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _visibilityTransform = transform.Find("Visibility");

            _orientationTransform = _visibilityTransform.Find("Orientation");

            _miniatureTransform = _orientationTransform.Find("Miniature");
            _miniatureAnimator = _miniatureTransform.GetComponent<Animator>();

            _bodyTransform = _miniatureTransform.Find("Body");

            _bodyVerticalDisplacementTransform = _bodyTransform.Find("Vertical displacement");
            _bodyVerticalDisplacementAnimator = _bodyVerticalDisplacementTransform.GetComponent<Animator>();

            _bodyMeshTransform = _bodyVerticalDisplacementTransform.Find("Mesh");
            _bodyMeshRigidBody = _bodyMeshTransform.GetComponent<Rigidbody>();

            _standTransform = _miniatureTransform.Find("Stand");
            _standRigidBody = _standTransform.GetComponent<Rigidbody>();
            _configurableJoint = _standTransform.GetComponent<ConfigurableJoint>();

            _standMeshTransform = _standTransform.Find("Mesh");
            _standVerticalExtensionMeshTransform = _standTransform.Find("Vertical extension mesh");

            _hitPointsTransform = _visibilityTransform.Find("Hit points");
            _hitPointsSpriteRenderer = _hitPointsTransform.GetComponent<SpriteRenderer>();
        }

        private void OnDestroy()
        {
            Addressables.Release(_modelHandle);
            Addressables.Release(_bodyConvexColliderModelHandle);
        }

        public event Action destroyed;

        public void Initialize(CombatPresenter combatPresenter, Creature creature, Material material, ModelQuality modelQuality)
        {
            _combatPresenter = combatPresenter;
            _creature = creature;
            _material = material;

            // Disable display until we are fully loaded.
            _visibilityTransform.gameObject.SetActive(false);

            // Load creature model.
            _bodyAsset = Assets.GetBodyAsset(creature.bodyAssetName);
            LoadModel(modelQuality);

            // Set convex collider.
            _bodyConvexColliderModelHandle = Addressables.LoadAssetAsync<GameObject>(_bodyAsset.convexColliderModelReference);

            _bodyConvexColliderModelHandle.Completed += handle =>
            {
                Mesh convexColliderMesh = handle.Result.GetComponent<MeshFilter>().sharedMesh;
                _bodyMeshTransform.GetComponent<MeshCollider>().sharedMesh = convexColliderMesh;
            };

            // Set stand model.
            GameObject stand = stands[(int)_creature.sizeCategory - 1];
            Mesh standMesh = stand.GetComponent<MeshFilter>().sharedMesh;

            _standMeshTransform.GetComponent<MeshFilter>().sharedMesh = standMesh;
            _standMeshTransform.GetComponent<MeshRenderer>().material = _material;
            _standMeshTransform.GetComponent<MeshCollider>().sharedMesh = standMesh;

            if (_bodyAsset.verticalExtensionHeight > 0)
            {
                // Resize to desired height and radius.
                float radiusScale = _creature.spaceInFeet / 5;
                _standVerticalExtensionMeshTransform.localScale = new Vector3(radiusScale, _bodyAsset.verticalExtensionHeight / 5, radiusScale);

                // Set vertical extension model.
                Mesh verticalExtensionMesh = verticalExtension5.GetComponent<MeshFilter>().sharedMesh;

                _standVerticalExtensionMeshTransform.GetComponent<MeshFilter>().sharedMesh = verticalExtensionMesh;
                _standVerticalExtensionMeshTransform.GetComponent<MeshRenderer>().material = _material;

                _standVerticalExtensionMeshTransform.GetComponent<MeshCollider>().sharedMesh = verticalExtensionMesh;
            }

            // Set body height.
            StartCoroutine(ResetBodyMeshTransform(0, Easing.None));

            // Set initial hit points.
            SetHitPointRatio((float)_creature.hitPoints / _creature.hitPointsMaximum);

            _hitPointsTransform.localScale = Vector3.one * _creature.spaceInFeet;

            // Set initial standing/lying state.
            _standing = creature.hitPoints > 0;

            if (_standing)
            {
                // Stand and body start joined.
                JoinStandAndBody();
            }
            else
            {
                // Let body fall down on its side.
                _bodyMeshTransform.rotation = Quaternion.Euler(90, 0, 0);
                _bodyMeshRigidBody.isKinematic = false;
            }

            // Activate when all loading has finished.
            StartCoroutine(ActivateOnLoaded());
        }

        private IEnumerator ActivateOnLoaded()
        {
            yield return _modelHandle;
            yield return _bodyConvexColliderModelHandle;

            _visibilityTransform.gameObject.SetActive(true);
        }

        public void SetModelQuality(ModelQuality modelQuality)
        {
            // Release the previous model.
            Addressables.Release(_modelHandle);

            // Load the new model.
            LoadModel(modelQuality);
        }

        private void LoadModel(ModelQuality modelQuality)
        {
            _modelHandle = Addressables.LoadAssetAsync<GameObject>(modelQuality == ModelQuality.Low ? _bodyAsset.lowPolyModelReference : _bodyAsset.highPolyModelReference);

            _modelHandle.Completed += handle =>
            {
                // Set body model.
                Mesh bodyMesh = handle.Result.GetComponent<MeshFilter>().sharedMesh;
                _bodyMeshTransform.GetComponent<MeshFilter>().sharedMesh = bodyMesh;
                _bodyMeshTransform.GetComponent<MeshRenderer>().material = _material;
            };
        }

        private void EnableStandingMiniaturePhysics()
        {
            _bodyMeshRigidBody.isKinematic = false;
            _standRigidBody.isKinematic = false;
        }

        private void ResetStandingMiniature()
        {
            _resetStandingMiniatureCoroutines.Add(StartCoroutine(ResetStandingMiniatureCoroutine()));
        }

        private IEnumerator ResetStandingMiniatureCoroutine()
        {
            const float delayDuration = 0.5f;
            const float transitionDuration = 1;

            yield return new WaitForSeconds(delayDuration);

            _standRigidBody.isKinematic = true;
            _bodyMeshRigidBody.isKinematic = true;

            Coroutine resetStandCoroutine = StartCoroutine(ResetStandTransform(transitionDuration, Easing.EaseInOut));
            Coroutine resetBodyMeshCoroutine = StartCoroutine(ResetBodyMeshTransform(transitionDuration, Easing.EaseInOut));

            _resetStandingMiniatureCoroutines.Add(resetStandCoroutine);
            _resetStandingMiniatureCoroutines.Add(resetBodyMeshCoroutine);

            yield return resetStandCoroutine;
            yield return resetBodyMeshCoroutine;

            _resetStandingMiniatureCoroutines.Clear();
        }

        private IEnumerator WaitForResetStandingMiniature()
        {
            if (_resetStandingMiniatureCoroutines.Count == 0) yield break;

            yield return _resetStandingMiniatureCoroutines[0];
        }

        private void CancelResetStandingMiniature()
        {
            if (_resetStandingMiniatureCoroutines.Count == 0) return;

            foreach (Coroutine coroutine in _resetStandingMiniatureCoroutines)
            {
                StopCoroutine(coroutine);
            }

            _resetStandingMiniatureCoroutines.Clear();
        }

        private void BreakDown()
        {
            Destroy(_bodyFixedJoint);
            _standing = false;
            _bodyMeshRigidBody.isKinematic = false;
            _configurableJoint.enableCollision = true;
        }

        private IEnumerator StandUp()
        {
            const float floatingTransitionDuration = 1;
            const float resetTransitionDuration = 1;

            _bodyMeshRigidBody.isKinematic = true;
            _configurableJoint.enableCollision = false;

            // The creature should float up.
            Vector3 floatingPosition = _bodyMeshTransform.localPosition;
            floatingPosition.y = 3;

            yield return LerpTransform(_bodyMeshTransform, floatingPosition, Quaternion.identity, floatingTransitionDuration, Easing.EaseOut);

            // Reset both transforms in parallel.
            Coroutine resetStandCoroutine = StartCoroutine(ResetStandTransform(resetTransitionDuration, Easing.EaseInOut));
            Coroutine resetBodyMeshCoroutine = StartCoroutine(ResetBodyMeshTransform(resetTransitionDuration, Easing.EaseInOut));

            yield return resetStandCoroutine;
            yield return resetBodyMeshCoroutine;

            // Join stand and body.
            JoinStandAndBody();

            _standing = true;
        }

        private void JoinStandAndBody()
        {
            _bodyFixedJoint = _standTransform.gameObject.AddComponent<FixedJoint>();
            _bodyFixedJoint.connectedBody = _bodyMeshRigidBody;
        }

        private IEnumerator ResetStandTransform(float transitionDuration, Easing easing)
        {
            return LerpTransform(_standTransform, Vector3.zero, Quaternion.identity, transitionDuration, easing);
        }

        private IEnumerator ResetBodyMeshTransform(float transitionDuration, Easing easing)
        {
            float verticalExtensionHeight = _bodyAsset.verticalExtensionHeight * Random.Range(0.9f, 1.1f);
            Vector3 position = new(0, verticalExtensionHeight, 0);

            return LerpTransform(_bodyMeshTransform, position, Quaternion.identity, transitionDuration, easing);
        }

        private static IEnumerator LerpTransform(Transform targetTransform, Vector3 position, Quaternion rotation, float transitionDuration, Easing easing)
        {
            Vector3 startPosition = targetTransform.localPosition;
            Quaternion startRotation = targetTransform.localRotation;
            float startTime = Time.time;

            float transitionProgress;

            do
            {
                transitionProgress = transitionDuration > 0 ? Mathf.Clamp01((Time.time - startTime) / transitionDuration) : 1;

                float easedTransitionProgress = EasingHelper.Ease(transitionProgress, easing);

                // Smoothly interpolate to identity.
                targetTransform.localPosition = Vector3.Lerp(startPosition, position, easedTransitionProgress);
                targetTransform.localRotation = Quaternion.Slerp(startRotation, rotation, easedTransitionProgress);

                yield return null;
            } while (transitionProgress < 1);
        }
    }
}
