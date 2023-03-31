using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Drawing
{
    public class GainExperienceEventPresenter : CombatEventPresenter, IEventPresenter<GainExperienceEvent>
    {
        public GainExperienceEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(GainExperienceEvent gainExperienceEvent)
        {
            CreaturePresenter creaturePresenter = combatPresenter.GetCreaturePresenterForCreature(gainExperienceEvent.character);

            // Make sure the character didn't die yet.
            if (creaturePresenter is null) yield break;

            creaturePresenter.GainExperiencePoints();

            yield return null;
        }
    }
}
