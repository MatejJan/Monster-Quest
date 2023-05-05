namespace MonsterQuest.Events
{
    public class DamageEvent
    {
        public Creature attacker;
        public Creature creature;
        public Damage damage;
        public DamageAmountResult[] damageAmountResults;
        public int hitPointsEnd;
        public int hitPointsMaximum;
        public int hitPointsStart;
        public int remainingDamageAmount;

        public class DamageAmountResult
        {
            public DamageAlteration damageAlteration;
            public DamageAmount damageAmount;
            public DamageAmount finalDamageAmount;
            public DamageAmount modifiedDamageAmount;
            public SavingThrowResult savingThrowResult;
        }
    }
}
