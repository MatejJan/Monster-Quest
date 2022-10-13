using System;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Finesse", menuName = "Effects/Finesse")]
    public class FinesseType : EffectType
    {
        public override Effect Create(object parent)
        {
            return new Finesse(this, parent);
        }
    }

    [Serializable]
    public class Finesse : Effect
    {
        public Finesse(EffectType type, object parent) : base(type, parent) { }
    }
}