namespace MonsterQuest
{
    public class AbilityCheckResult
    {
        public AbilityCheckResult(Creature creature, Ability ability, int successAmount)
        {
            this.creature = creature;
            this.ability = ability;
            this.successAmount = successAmount;

            success = creature.MakeAbilityCheck(ability, successAmount, out RollResult rollResultValue);
            rollResult = rollResultValue;
        }

        public AbilityCheckResult(Creature creature, Ability ability, int successAmount, RollResult rollResult, bool success)
        {
            this.creature = creature;
            this.ability = ability;
            this.successAmount = successAmount;
            this.rollResult = rollResult;
            this.success = success;
        }

        public Creature creature { get; }
        public Ability ability { get; }
        public int successAmount { get; }
        public RollResult rollResult { get; }
        public bool success { get; }

        public override string ToString()
        {
            return $"{(success ? "Successful" : "Failed")} DC {successAmount} {ability} check ({rollResult})";
        }
    }
}
