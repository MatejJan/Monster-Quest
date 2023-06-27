using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Narrative
{
    public class LevelUpEventPresenter : EventPresenter, IEventPresenter<LevelUpEvent>
    {
        public LevelUpEventPresenter(IOutput output) : base(output) { }

        public IEnumerator Present(LevelUpEvent levelUpEvent)
        {
            output.WriteLine($"{levelUpEvent.character.displayName.ToUpperFirst()} levels up to level {levelUpEvent.character.characterClass.level}! Their maximum HP increases to {levelUpEvent.character.hitPointsMaximum}.");

            yield return null;
        }
    }
}
