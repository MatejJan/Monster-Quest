using System.Collections;
using MonsterQuest.Effects;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Narrative
{
    public class UseItemEventPresenter : EventPresenter, IEventPresenter<UseItemEvent>
    {
        public UseItemEventPresenter(IOutput output) : base(output) { }

        public IEnumerator Present(UseItemEvent useItemEvent)
        {
            Item item = useItemEvent.useItemAction.item;
            Creature creature = useItemEvent.useItemAction.creature;
            Creature target = useItemEvent.useItemAction.target;

            if (item.HasEffect<HealingItem>())
            {
                if (target is not null)
                {
                    output.WriteLine($"{creature.definiteName.ToUpperFirst()} administers {item.definiteName} to {target.definiteName}.");
                }
                else
                {
                    target = creature;
                    HealingItemType healingItemType = item.GetEffect<HealingItem>().healingItemType;
                    output.WriteLine($"{target.definiteName.ToUpperFirst()} {healingItemType.verb} {item.definiteName}.");
                }
            }

            yield return null;
        }
    }
}
