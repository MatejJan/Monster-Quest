using System.Collections;

namespace MonsterQuest
{
    public interface IReactToUseItem
    {
        public IEnumerator ReactToUseItem(UseItemAction useItemAction)
        {
            yield break;
        }

        public IEnumerator ReactToItemUsed(UseItemAction useItemAction)
        {
            yield break;
        }
    }
}
