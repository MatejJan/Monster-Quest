using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Narrative
{
    public class HealEventPresenter : EventPresenter, IEventPresenter<HealEvent>
    {
        public HealEventPresenter(IOutput output) : base(output) { }

        public IEnumerator Present(HealEvent healEvent)
        {
            output.WriteLine($"{healEvent.creature.definiteName.ToUpperFirst()} heals {healEvent.amount} HP and is at {(healEvent.hitPointsEnd == healEvent.hitPointsMaximum ? "full health" : $"{healEvent.hitPointsEnd} HP")}.");

            yield return null;
        }
    }
}
