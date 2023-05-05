namespace MonsterQuest.Presenters.Miniatures
{
    public class CombatEventPresenter
    {
        protected readonly CombatPresenter combatPresenter;

        protected CombatEventPresenter(CombatPresenter combatPresenter)
        {
            this.combatPresenter = combatPresenter;
        }
    }
}
