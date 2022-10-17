namespace MonsterQuest
{
    public interface ICoverRule
    {
        CoverValue GetCover(AttackAction attackAction);
    }
}
