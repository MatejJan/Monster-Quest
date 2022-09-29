using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public static class ListHelpers
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;

            for (int i = 0; i < n - 1; i++)
            {
                int j = Random.Range(i, n);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
