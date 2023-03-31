using System;
using MonsterQuest.Events;

namespace MonsterQuest
{
    public class UseItemAction : IAction, IStateEventProvider
    {
        public UseItemAction(GameState gameState, Creature creature, Item item, Creature target = null)
        {
            this.gameState = gameState;
            this.creature = creature;
            this.item = item;
            this.target = target;
        }

        public GameState gameState { get; }
        public Creature creature { get; }
        public Item item { get; }
        public Creature target { get; }

        public void Execute()
        {
            DebugHelper.StartLog($"{creature.definiteName.ToUpperFirst()} is using {item.definiteName} … ");

            ReportStateEvent(new UseItemEvent
            {
                useItemAction = this
            });

            // Use the item.
            gameState.CallRules((IReactToUseItem rule) => rule.ReactToUseItem(this));

            // Inform that item was used.
            gameState.CallRules((IReactToUseItem rule) => rule.ReactToItemUsed(this));

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
