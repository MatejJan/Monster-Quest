using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu]
    public class WeaponType : ItemType
    {
        public string damageRoll;
        public bool isRanged;
        public bool isFinesse;
    }
}
