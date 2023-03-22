using System;
using System.Collections.Generic;

namespace MonsterQuest
{
    [Serializable]
    public class GameState
    {
        private List<MonsterType> _remainingMonsterTypes;
        
        public GameState(Party party, MonsterType[] monsterTypes)
        {
            this.party = party;
            _remainingMonsterTypes = new List<MonsterType>(monsterTypes);
        }
        
        public Party party { get; }
        public Combat combat { get; private set; }
        
        public bool EnterCombatWithNextMonster()
        {
            if (_remainingMonsterTypes.Count == 0) return false;
            
            Monster monster = new(_remainingMonsterTypes[0]);
            _remainingMonsterTypes.RemoveAt(0);
                
            Console.WriteLine($"Watch out, {monster.displayName} with {monster.hitPoints} HP appears!");

            combat = new Combat(this, monster);
            return true;
        }

        public void EndCombat()
        {
            combat = null;
        }
    }
}
