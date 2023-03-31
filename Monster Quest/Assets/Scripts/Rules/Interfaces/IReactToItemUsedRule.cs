namespace MonsterQuest
{
    public interface IReactToUseItem
    {
        public void ReactToUseItem(UseItemAction useItemAction) { }

        public void ReactToItemUsed(UseItemAction useItemAction) { }
    }
}
