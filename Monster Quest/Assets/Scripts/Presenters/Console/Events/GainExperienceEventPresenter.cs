using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Console
{
    public class GainExperienceEventPresenter : IEventPresenter<GainExperienceEvent>
    {
        public IEnumerator Present(GainExperienceEvent gainExperienceEvent)
        {
            MonsterQuest.Console.WriteLine($"{gainExperienceEvent.character.displayName.ToUpperFirst()} gains {gainExperienceEvent.amount} experience points.");

            yield return null;
        }
    }
}
