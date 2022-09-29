using MonsterQuest.Actions;

namespace MonsterQuest
{
    public interface ICoverRule
    {
        CoverValue GetCover(Attack attack);
    }
}
