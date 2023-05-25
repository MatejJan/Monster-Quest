using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Miniatures
{
    public class UseItemEventPresenter : CombatEventPresenter, IEventPresenter<UseItemEvent>
    {
        public UseItemEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(UseItemEvent useItemEvent)
        {
            // If a target is specified, turn towards it.
            if (useItemEvent.useItemAction.target is null) yield break;

            CreaturePresenter creaturePresenter = combatPresenter.GetCreaturePresenterForCreature(useItemEvent.useItemAction.creature);

            yield return creaturePresenter.FaceCreature(useItemEvent.useItemAction.target);
        }
    }
}
