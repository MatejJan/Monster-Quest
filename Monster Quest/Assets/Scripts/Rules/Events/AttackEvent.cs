using System.Collections.Generic;

namespace MonsterQuest.Events
{
    public class AttackEvent
    {
        public AttackAction attackAction;
        public AttackRollMethod[] attackRollMethods;
        public IEnumerable<MultipleValue<AttackRollMethod>> attackRollMethodValues;
        public int attackRollModifier;
        public IEnumerable<IntegerValue> attackRollModifierValues;
        public int attackRollTotal;
        public DamageRollResult[] damageRollResults;
        public IEnumerable<ArrayValue<DamageRoll>> damageRollValues;
        public RollResult firstAttackRollResult;
        public bool hadAdvantage;
        public bool hadDisadvantage;
        public Creature redirectedFromTarget;
        public RollResult secondAttackRollResult;
        public int targetArmorClass;
        public IEnumerable<IntegerValue> targetArmorClassValues;
        public IEnumerable<SingleValue<Creature>> targetRedirectionValues;
        public bool wasCritical;
        public bool wasHit;

        public class DamageRollResult
        {
            public int amount;
            public DamageRoll damageRoll;
            public int? damageRollModifier;
            public IEnumerable<IntegerValue> damageRollModifierValues;
            public RollResult firstDamageRollResult;
            public RollResult secondDamageRollResult;
        }
    }
}
