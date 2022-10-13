using System;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Ammunition", menuName = "Effects/Ammunition")]
    public class AmmunitionType : EffectType
    {
        public override Effect Create(object parent)
        {
            return new Ammunition(this, parent);
        }
    }

    [Serializable]
    public class Ammunition : Effect
    {
        public Ammunition(EffectType type, object parent) : base(type, parent) { }
    }
}