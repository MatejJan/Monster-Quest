using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Miniatures
{
    public class AttackEventPresenter : CombatEventPresenter, IEventPresenter<AttackEvent>
    {
        public AttackEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(AttackEvent attackEvent)
        {
            CreaturePresenter attackerPresenter = combatPresenter.GetCreaturePresenterForCreature(attackEvent.attackAction.attacker);

            // Face and attack the target.
            yield return attackerPresenter.FaceCreature(attackEvent.attackAction.target);
            yield return attackerPresenter.Attack();
        }
    }
}
