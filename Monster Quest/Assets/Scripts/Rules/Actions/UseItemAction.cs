using System.Collections;

namespace MonsterQuest
{
    public class UseItemAction : IAction
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

        public IEnumerator Execute()
        {
            DebugHelper.StartLog($"{creature.definiteName.ToUpperFirst()} is using {item.definiteName} … ");

            // Use the item.
            yield return gameState.CallRules((IReactToUseItem rule) => rule.ReactToUseItem(this));

            // Inform that item was used.
            yield return gameState.CallRules((IReactToUseItem rule) => rule.ReactToItemUsed(this));

            DebugHelper.EndLog();
        }
    }
}
