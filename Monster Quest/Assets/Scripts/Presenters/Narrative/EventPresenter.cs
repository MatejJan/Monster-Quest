namespace MonsterQuest.Presenters.Narrative
{
    public class EventPresenter
    {
        protected readonly IOutput output;

        protected EventPresenter(IOutput output)
        {
            this.output = output;
        }
    }
}
