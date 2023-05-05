namespace MonsterQuest.Presenters.Drawing
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
