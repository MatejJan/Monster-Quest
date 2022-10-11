using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Light", menuName = "Effects/Light")]
    public class LightType : EffectType
    {
        public override Effect Create(object parent)
        {
            return new Light(this, parent);
        }
    }

    public class Light : Effect
    {
        public Light(EffectType type, object parent) : base(type, parent) { }
    }
}
