using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Armor", menuName = "Armor")]
    public class ArmorType : ItemType
    {
        public int armorClass;
        public ArmorCategory category;
    }
}
