using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Race", menuName = "Race")]
    public class Race : ScriptableObject
    {
        public string displayName;
        public SizeCategory size;
    }
}
