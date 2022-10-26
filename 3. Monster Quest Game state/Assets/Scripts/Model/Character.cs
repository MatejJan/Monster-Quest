namespace MonsterQuest
{
    public class Character
    {
        public Character(string displayName)
        {
            this.displayName = displayName;
        }
        
        public string displayName { get; private set; }
    }
}
