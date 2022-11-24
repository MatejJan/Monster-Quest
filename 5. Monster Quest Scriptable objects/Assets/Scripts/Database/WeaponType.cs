using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
    public class WeaponType : ItemType
    {
        public string damageRoll;
    }
}
