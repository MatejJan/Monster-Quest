using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Miniatures
{
    public class HealEventPresenter : CombatEventPresenter, IEventPresenter<HealEvent>
    {
        public HealEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(HealEvent healEvent)
        {
            CreaturePresenter creaturePresenter = combatPresenter.GetCreaturePresenterForCreature(healEvent.creature);

            yield return creaturePresenter.Heal(healEvent.hitPointsEnd, healEvent.hitPointsStart > 0);
        }
    }
}
