using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu]
    public class ClassType : ScriptableObject
    {
        public string displayName;
        public WeaponCategory[] weaponProficiencies;
    }
}
