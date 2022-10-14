using System;

namespace MonsterQuest
{
    [Serializable]
    public class CoinValue
    {
        public int copper;
        public int silver;
        public int electrum;
        public int gold;
        public int platinum;

        public int value => copper + silver * 10 + electrum * 50 + gold * 100 + platinum * 1000;
    }
}
