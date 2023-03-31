using System;
using MonsterQuest.Events;

namespace MonsterQuest
{
    public class StabilizeCharacterAction : IAction, IStateEventProvider
    {
        public StabilizeCharacterAction(Character character, Character target)
        {
            this.character = character;
            this.target = target;
        }

        private GameState gameState { get; }
        public Character character { get; }
        public Character target { get; }

        public void Execute()
        {
            DebugHelper.StartLog($"Performing stabilize character action by {character.definiteName} targeting {target.definiteName} … ");

            // Perform a DC 10 Wisdom (Medicine) check.
            StabilizeCharacterEvent stabilizeCharacterEvent = new()
            {
                stabilizeCharacterAction = this,
                wisdomAbilityCheckResult = new AbilityCheckResult(character, Ability.Wisdom, 10)
            };

            ReportStateEvent(stabilizeCharacterEvent);

            if (stabilizeCharacterEvent.wisdomAbilityCheckResult.success) target.Stabilize();

            DebugHelper.EndLog();
        }

        // Events 

        public event Action<object> stateEvent;

        // Methods 

        private void ReportStateEvent(object eventData)
        {
            stateEvent?.Invoke(eventData);
        }
    }
}
