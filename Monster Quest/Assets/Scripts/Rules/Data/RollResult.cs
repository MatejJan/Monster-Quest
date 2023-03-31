namespace MonsterQuest
{
    public class RollResult
    {
        public RollResult(string roll)
        {
            this.roll = roll;

            result = DiceHelper.Roll(roll, out int[] rollsValue);
            rolls = rollsValue;
        }

        public RollResult(string roll, int[] rolls, int result)
        {
            this.roll = roll;
            this.rolls = rolls;
            this.result = result;
        }

        public string roll { get; }
        public int[] rolls { get; }
        public int result { get; }

        public override string ToString()
        {
            return $"{roll} -> {result}";
        }

        public static implicit operator int(RollResult rollResult)
        {
            return rollResult.result;
        }
    }
}
