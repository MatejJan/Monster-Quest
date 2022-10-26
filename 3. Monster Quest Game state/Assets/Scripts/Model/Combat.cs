namespace MonsterQuest
{
    public class Combat
    {
        public Combat(Monster monster)
        {
            this.monster = monster;
        }
        
        public Monster monster { get; private set; }
    }
}
