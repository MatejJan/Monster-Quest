using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MonsterQuest.Presenters
{
    [CreateAssetMenu(fileName = "New Body Asset", menuName = "Assets/Body Asset")]
    public class BodyAsset : ScriptableObject
    {
        public AssetReferenceSprite spriteReference;
        public AssetReferenceGameObject highPolyModelReference;
        public AssetReferenceGameObject lowPolyModelReference;
        public AssetReferenceGameObject convexColliderModelReference;
        public float flyHeight;
        public float verticalExtensionHeight;
    }
}
