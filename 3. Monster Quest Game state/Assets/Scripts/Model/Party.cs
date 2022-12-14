using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class Party
    {
        public Party(IEnumerable<Character> initialCharacters)
        {
            characters = new List<Character>(initialCharacters);
        }
        
        public List<Character> characters { get; private set; }
        
        public override string ToString()
        {
            return StringHelper.JoinWithAnd(characters.Select(character => character.displayName));
        }
    }
}
