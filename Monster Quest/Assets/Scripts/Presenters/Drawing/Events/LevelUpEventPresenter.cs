using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Drawing
{
    public class LevelUpEventPresenter : CombatEventPresenter, IEventPresenter<LevelUpEvent>
    {
        public LevelUpEventPresenter(CombatPresenter combatPresenter) : base(combatPresenter) { }

        public IEnumerator Present(LevelUpEvent levelUpEvent)
        {
            CreaturePresenter creaturePresenter = combatPresenter.GetCreaturePresenterForCreature(levelUpEvent.character);

            // Make sure the character didn't die yet.
            if (creaturePresenter is null) yield break;

            yield return creaturePresenter.LevelUp();
        }
    }
}
