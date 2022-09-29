using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Two-handed", menuName = "Effects/Two-handed")]
    public class TwoHandedType : EffectType
    {
        public override Effect Create(object parent)
        {
            return new TwoHanded(this, parent);
        }
    }

    public class TwoHanded : Effect
    {
        public TwoHanded(EffectType type, object parent) : base(type, parent) { }
    }
}
