namespace MonsterQuest
{
    public class SavingThrowResult : AbilityCheckResult
    {
        public SavingThrowResult(Creature creature, Ability ability, int successAmount) : base(creature, ability, successAmount) { }
        public SavingThrowResult(Creature creature, Ability ability, int successAmount, RollResult rollResult, bool success) : base(creature, ability, successAmount, rollResult, success) { }
    }
}
