using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class Party : List<Character>, IRulesHandler
    {
        public IEnumerable<object> rules => this.SelectMany(character => character.rules);

        public override string ToString()
        {
            return StringHelpers.JoinWithAnd(this.Select(character => character.name));
        }
    }
}
