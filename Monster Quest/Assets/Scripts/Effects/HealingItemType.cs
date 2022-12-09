using System;
using System.Collections;
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

        public IEnumerator ReactToUseItem(UseItemAction useItemAction)
        {
            // Only provide information for the current item.
            if (useItemAction.item != parent) yield break;

            // If a target is specified, turn towards it.
            if (useItemAction.target is not null && useItemAction.creature.presenter is not null)
            {
                yield return useItemAction.creature.presenter.FaceCreature(useItemAction.target);
            }

            // Heal the rolled amount of hit points.
            int healAmount = DiceHelper.Roll(healingItemType.amountRoll);

            // The target is the specified target or the user.
            Creature target = useItemAction.target;

            if (target is not null)
            {
                Console.WriteLine($"{useItemAction.creature.definiteName.ToUpperFirst()} administers {useItemAction.item.definiteName} to {target.definiteName}.");
            }
            else
            {
                target = useItemAction.creature;
                Console.WriteLine($"{target.definiteName.ToUpperFirst()} {healingItemType.verb} {useItemAction.item.definiteName}.");
            }

            yield return target.Heal(healAmount);
        }
    }
}
