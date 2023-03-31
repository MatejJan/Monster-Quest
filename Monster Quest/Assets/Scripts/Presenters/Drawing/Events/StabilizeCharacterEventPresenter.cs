using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Drawing
{
    public class StabilizeCharacterEventPresenter : CombatEventPresenter, IEventPresenter<StabilizeCharacterEvent>
    {
        public StabilizeCharacterEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(StabilizeCharacterEvent stabilizeCharacterEvent)
        {
            CreaturePresenter characterPresenter = combatPresenter.GetCreaturePresenterForCreature(stabilizeCharacterEvent.stabilizeCharacterAction.character);

            // Face the target.
            yield return characterPresenter.FaceCreature(stabilizeCharacterEvent.stabilizeCharacterAction.target);

            // Roll the dice.
            yield return characterPresenter.PerformAbilityCheck(stabilizeCharacterEvent.wisdomAbilityCheckResult.success, stabilizeCharacterEvent.wisdomAbilityCheckResult.rollResult.rolls[0]);
        }
    }
}
