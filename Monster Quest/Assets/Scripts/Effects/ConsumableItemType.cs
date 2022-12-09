using System;
using System.Collections;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Consumable Item", menuName = "Effects/Consumable Item")]
    public class ConsumableItemType : EffectType
    {
        public override Effect Create(object parent)
        {
            return new ConsumableItem(this, parent);
        }
    }

    [Serializable]
    public class ConsumableItem : Effect, IReactToUseItem
    {
        public ConsumableItem(EffectType type, object parent) : base(type, parent) { }

        public IEnumerator ReactToItemUsed(UseItemAction useItemAction)
        {
            // Only provide information for the current item.
            if (useItemAction.item != parent) yield return null;

            // A consumable item must be removed when used.
            useItemAction.creature.RemoveItem(useItemAction.gameState, useItemAction.item);
        }
    }
}
