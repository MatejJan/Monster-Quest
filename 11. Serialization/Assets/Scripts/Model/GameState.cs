namespace MonsterQuest
{
    public class GameState
    {
        public GameState(Party party)
        {
            this.party = party;
        }
        
        public Party party { get; private set; }
        public Combat combat { get; private set; }
        
        public void EnterCombatWithMonster(Monster monster)
        {
            combat = new Combat(monster);
        }
    }
}
