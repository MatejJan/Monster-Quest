using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MonsterQuest.Presenters
{
    [CreateAssetMenu(fileName = "New Body Asset", menuName = "Assets/Body Asset")]
    public class BodyAsset : ScriptableObject
    {
        public AssetReferenceSprite spriteReference;
        public AssetReferenceGameObject modelReference;
    }
}
