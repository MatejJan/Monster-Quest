using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Drawing
{
    public class LifeStatusEventPresenter : CombatEventPresenter, IEventPresenter<LifeStatusEvent>
    {
        public LifeStatusEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(LifeStatusEvent lifeStatusEvent)
        {
            CreaturePresenter creaturePresenter = combatPresenter.GetCreaturePresenterForCreature(lifeStatusEvent.creature);

            switch (lifeStatusEvent.newLifeStatus)
            {
                case LifeStatus.Dead:
                    yield return creaturePresenter.Die();

                    break;

                case LifeStatus.Conscious:
                    yield return creaturePresenter.RegainConsciousness();

                    break;
            }
        }
    }
}
