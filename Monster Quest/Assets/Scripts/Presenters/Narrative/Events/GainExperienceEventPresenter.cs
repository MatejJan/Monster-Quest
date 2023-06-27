using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Narrative
{
    public class GainExperienceEventPresenter : EventPresenter, IEventPresenter<GainExperienceEvent>
    {
        public GainExperienceEventPresenter(IOutput output) : base(output) { }

        public IEnumerator Present(GainExperienceEvent gainExperienceEvent)
        {
            output.WriteLine($"{gainExperienceEvent.character.displayName.ToUpperFirst()} gains {gainExperienceEvent.amount} experience points.");

            yield return null;
        }
    }
}
