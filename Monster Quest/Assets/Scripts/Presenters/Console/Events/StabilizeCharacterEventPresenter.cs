using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Console
{
    public class StabilizeCharacterEventPresenter : IEventPresenter<StabilizeCharacterEvent>
    {
        public IEnumerator Present(StabilizeCharacterEvent stabilizeCharacterEvent)
        {
            Character character = stabilizeCharacterEvent.stabilizeCharacterAction.character;
            Character target = stabilizeCharacterEvent.stabilizeCharacterAction.target;

            MonsterQuest.Console.WriteLine($"{character.definiteName.ToUpperFirst()} administer first aid to {target.definiteName} {(stabilizeCharacterEvent.wisdomAbilityCheckResult.success ? "and manages" : "but fails")} to stabilize them.");

            yield return null;
        }
    }
}
