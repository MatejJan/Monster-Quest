using System;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Healing Item", menuName = "Effects/Healing Item")]
    public class HealingItemType : EffectType
    {
        public string amountRoll;
        public string verb;

        public override Effect Create(object parent)
        {
            return new HealingItem(this, parent);
        }
    }

    [Serializable]
    public class HealingItem : Effect, IReactToUseItem
    {
        public HealingItem(EffectType type, object parent) : base(type, parent) { }
        public HealingItemType healingItemType => (HealingItemType)type;

        public void ReactToUseItem(UseItemAction useItemAction)
        {
            // Only provide information for the current item.
            if (useItemAction.item != parent) return;

            // Heal the rolled amount of hit points.
            int healAmount = DiceHelper.Roll(healingItemType.amountRoll);

            // The target is the specified target or the user.
            Creature target = useItemAction.target ?? useItemAction.creature;

            target.Heal(healAmount);
        }
    }
}
