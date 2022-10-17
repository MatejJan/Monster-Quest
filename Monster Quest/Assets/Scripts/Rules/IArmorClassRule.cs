namespace MonsterQuest
{
    public interface IArmorClassRule
    {
        IntegerValue GetArmorClass(Creature creature);

        IntegerValue GetArmorClass(AttackAction attackAction)
        {
            return GetArmorClass(attackAction.target);
        }
    }
}
