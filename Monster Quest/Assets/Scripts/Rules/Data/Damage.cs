using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class Damage
    {
        public Damage(IEnumerable<DamageAmount> amounts)
        {
            this.amounts = amounts.ToArray();

            hit = this.amounts[0].hit;
        }

        public Hit hit { get; }
        public DamageAmount[] amounts { get; }

        public override string ToString()
        {
            IEnumerable<string> amountParts = amounts.Select(amount => amount.roll.type.ToString().ToLowerInvariant());

            return $"{StringHelper.JoinWithAnd(amountParts)} damage";
        }
    }
}
