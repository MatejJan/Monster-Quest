using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Narrative
{
    public class StabilizeCharacterEventPresenter : EventPresenter, IEventPresenter<StabilizeCharacterEvent>
    {
        public StabilizeCharacterEventPresenter(IOutput output) : base(output) { }

        public IEnumerator Present(StabilizeCharacterEvent stabilizeCharacterEvent)
        {
            Character character = stabilizeCharacterEvent.stabilizeCharacterAction.character;
            Character target = stabilizeCharacterEvent.stabilizeCharacterAction.target;

            output.WriteLine($"{character.definiteName.ToUpperFirst()} administer first aid to {target.definiteName} {(stabilizeCharacterEvent.wisdomAbilityCheckResult.success ? "and manages" : "but fails")} to stabilize them.");

            yield return null;
        }
    }
}
