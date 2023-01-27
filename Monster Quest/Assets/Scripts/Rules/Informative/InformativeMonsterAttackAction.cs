using System.Collections.Generic;
using MonsterQuest.Effects;

namespace MonsterQuest
{
    public class InformativeMonsterAttackAction : InformativeContext
    {
        private readonly List<object> _rules;

        public InformativeMonsterAttackAction(MonsterType attacker, AttackType effect, ItemType weapon)
        {
            this.attacker = attacker;
            this.effect = effect;
            this.weapon = weapon;

            _rules = new List<object>
            {
                new InformativeMonsterAttackAbilityModifier(),
                attacker,
                effect
            };

            if (weapon != null)
            {
                _rules.AddRange(weapon.effects);
            }
        }

        public MonsterType attacker { get; }
        public AttackType effect { get; }
        public ItemType weapon { get; }

        public override IEnumerable<object> rules => _rules;
    }
}
