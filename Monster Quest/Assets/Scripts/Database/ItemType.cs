using UnityEngine;

namespace MonsterQuest
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Item")]
    public class ItemType : ScriptableObject
    {
        public string displayName;
        public CoinValue cost;
        public float weight;

        public EffectType[] effects;

        public string definiteName => EnglishHelper.GetDefiniteNounForm(displayName);
        public string indefiniteName => EnglishHelper.GetIndefiniteNounForm(displayName);

        public Item Create()
        {
            return new Item(this);
        }
    }
}
