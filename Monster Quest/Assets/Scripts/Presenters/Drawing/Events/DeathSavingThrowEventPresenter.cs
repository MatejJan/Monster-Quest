using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Drawing
{
    public class DeathSavingThrowEventPresenter : CombatEventPresenter, IEventPresenter<DeathSavingThrowEvent>
    {
        public DeathSavingThrowEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(DeathSavingThrowEvent deathSavingThrowEvent)
        {
            CreaturePresenter creaturePresenter = combatPresenter.GetCreaturePresenterForCreature(deathSavingThrowEvent.creature);

            for (int i = 0; i < deathSavingThrowEvent.amount; i++)
            {
                yield return creaturePresenter.PerformDeathSavingThrow(deathSavingThrowEvent.succeeded, i == 0 ? deathSavingThrowEvent.rollResult?.result : null);
            }

            if (deathSavingThrowEvent.deathSavingThrows.Length == 0)
            {
                creaturePresenter.ResetDeathSavingThrows();
            }
        }
    }
}
