using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Thrown", menuName = "Effects/Thrown")]
    public class ThrownType : EffectType
    {
        public override Effect Create(object parent)
        {
            return new Thrown(this, parent);
        }
    }

    public class Thrown : Effect
    {
        public Thrown(EffectType type, object parent) : base(type, parent) { }
    }
}
