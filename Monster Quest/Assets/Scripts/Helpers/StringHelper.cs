using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public static class StringHelper
    {
        public static string JoinWithAnd(IEnumerable items, bool useSerialComma = true)
        {
            string[] itemsList = (from object item in items select item.ToString()).ToArray();

            int count = itemsList.Length;

            switch (count)
            {
                case 0:
                    return "";

                case 1:
                    return itemsList[0];

                case 2:
                    return $"{itemsList[0]} and {itemsList[1]}";
            }

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
