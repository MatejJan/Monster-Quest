using System.Collections;
using MonsterQuest.Events;
using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public class DamageEventPresenter : CombatEventPresenter, IEventPresenter<DamageEvent>
    {
        public DamageEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(DamageEvent damageEvent)
        {
            CreaturePresenter creaturePresenter = combatPresenter.GetCreaturePresenterForCreature(damageEvent.creature);
            CreaturePresenter attackerCreaturePresenter = combatPresenter.GetCreaturePresenterForCreature(damageEvent.attacker);

            Vector3 sourcePosition = attackerCreaturePresenter.transform.position;
            bool knockedOut = damageEvent.hitPointsEnd == 0;
            bool instantDeath = damageEvent.remainingDamageAmount > damageEvent.hitPointsMaximum;

            yield return creaturePresenter.GetAttacked(damageEvent.hitPointsEnd, sourcePosition, knockedOut, instantDeath);
        }
    }
}
