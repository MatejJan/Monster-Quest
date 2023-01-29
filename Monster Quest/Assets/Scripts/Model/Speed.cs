using System;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class Speed
    {
        // State properties
        [field: SerializeField] public int walk { get; set; }
        [field: SerializeField] public int burrow { get; set; }
        [field: SerializeField] public int climb { get; set; }
        [field: SerializeField] public int fly { get; set; }
        [field: SerializeField] public int swim { get; set; }
        [field: SerializeField] public bool hover { get; set; }

        // Allow access with the enum.
        public int this[MovementType movementType]
        {
            get
            {
                return movementType switch
                {
                    MovementType.Walk => walk,
                    MovementType.Burrow => burrow,
                    MovementType.Climb => climb,
                    MovementType.Fly => fly,
                    MovementType.Swim => swim,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                switch (movementType)
                {
                    case MovementType.Walk:
                        walk = value;

                        break;

                    case MovementType.Burrow:
                        burrow = value;

                        break;

                    case MovementType.Climb:
                        climb = value;

                        break;

                    case MovementType.Fly:
                        fly = value;

                        break;

                    case MovementType.Swim:
                        swim = value;

                        break;
                }
            }
        }
    }
}
