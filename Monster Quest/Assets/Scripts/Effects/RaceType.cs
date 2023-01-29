using System;
using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Race", menuName = "Effects/Race")]
    public class RaceType : EffectType
    {
        public string displayName;
        public SizeCategory sizeCategory;
        public Speed speed;

        public override Effect Create(object parent)
        {
            return new Race(this, parent);
        }
    }

    [Serializable]
    public class Race : Effect
    {
        public Race(RaceType type, object parent) : base(type, parent) { }
        public RaceType raceType => (RaceType)type;
    }
}
