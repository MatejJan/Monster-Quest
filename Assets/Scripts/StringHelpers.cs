using System.Collections;
using System.Collections.Generic;

namespace MonsterQuest
{
    public static class StringHelpers
    {
        public static string JoinWithAnd(IEnumerable items, bool useSerialComma = true)
        {
            List<string> itemsList = new();

            foreach (object item in items)
            {
                itemsList.Add(item.ToString());
            }

            int count = itemsList.Count;

            if (count == 0) return "";
            if (count == 1) return itemsList[0];
            if (count == 2) return $"{itemsList[0]} and {itemsList[1]}";

            List<string> itemsCopy = new(itemsList);

            if (useSerialComma)
            {
                itemsCopy[count - 1] = $"and {itemsList[count - 1]}";
            }
            else
            {
                itemsCopy[count - 2] = $"{itemsList[count - 2]} and {itemsList[count - 1]}";
                itemsCopy.RemoveAt(count - 1);
            }

            return string.Join(", ", itemsCopy);
        }
    }
}
