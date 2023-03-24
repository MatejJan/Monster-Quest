using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu]
    public class ClassType : ScriptableObject
    {
        public string displayName;
        public WeaponCategory[] weaponProficiencies;
        public int hitDiceSides;
        
        public string hitDice => $"d{hitDiceSides}";
    }
}
