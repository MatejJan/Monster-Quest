using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Console
{
    public class HealEventPresenter : IEventPresenter<HealEvent>
    {
        public IEnumerator Present(HealEvent healEvent)
        {
            MonsterQuest.Console.WriteLine($"{healEvent.creature.definiteName.ToUpperFirst()} heals {healEvent.amount} HP and is at {(healEvent.hitPointsEnd == healEvent.hitPointsMaximum ? "full health" : $"{healEvent.hitPointsEnd} HP")}.");

            yield return null;
        }
    }
}
