using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Console
{
    public class LevelUpEventPresenter : IEventPresenter<LevelUpEvent>
    {
        public IEnumerator Present(LevelUpEvent levelUpEvent)
        {
            MonsterQuest.Console.WriteLine($"{levelUpEvent.character.displayName.ToUpperFirst()} levels up to level {levelUpEvent.character.characterClass.level}! Their maximum HP increases to {levelUpEvent.character.hitPointsMaximum}.");

            yield return null;
        }
    }
}
