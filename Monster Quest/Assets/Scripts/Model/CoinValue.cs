using System;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class CoinValue
    {
        [field: SerializeField] public int copper { get; set; }
        [field: SerializeField] public int silver { get; set; }
        [field: SerializeField] public int electrum { get; set; }
        [field: SerializeField] public int gold { get; set; }
        [field: SerializeField] public int platinum { get; set; }

        public int value => copper + silver * 10 + electrum * 50 + gold * 100 + platinum * 1000;
    }
}
