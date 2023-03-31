namespace MonsterQuest.Presenters.Drawing
{
    public class CombatEventPresenter
    {
        protected CombatPresenter combatPresenter;

        public CombatEventPresenter(CombatPresenter combatPresenter)
        {
            this.combatPresenter = combatPresenter;
        }
    }
}
