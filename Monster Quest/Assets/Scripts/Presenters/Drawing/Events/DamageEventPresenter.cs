using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Drawing
{
    public class DamageEventPresenter : CombatEventPresenter, IEventPresenter<DamageEvent>
    {
        public DamageEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(DamageEvent damageEvent)
        {
            CreaturePresenter creaturePresenter = combatPresenter.GetCreaturePresenterForCreature(damageEvent.creature);

            bool knockedOut = damageEvent.hitPointsEnd == 0;
            bool instantDeath = damageEvent.remainingDamageAmount > damageEvent.hitPointsMaximum;

            yield return creaturePresenter.GetAttacked(knockedOut, instantDeath);
        }
    }
}
