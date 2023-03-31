using System;
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

        public void ReactToItemUsed(UseItemAction useItemAction)
        {
            // Only provide information for the current item.
            if (useItemAction.item != parent) return;

            // A consumable item must be removed when used.
            useItemAction.creature.RemoveItem(useItemAction.gameState, useItemAction.item);
        }
    }
}
