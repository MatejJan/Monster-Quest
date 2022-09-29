using System;
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

        public string definiteName => EnglishHelpers.GetDefiniteNounForm(displayName);
        public string indefiniteName => EnglishHelpers.GetIndefiniteNounForm(displayName);

        public Item Create()
        {
            return new Item(this);
        }

        public T GetEffectType<T>() where T : EffectType
        {
            return Array.Find(effects, effect => effect is T) as T;
        }
    }
}
