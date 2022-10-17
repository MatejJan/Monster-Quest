using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    public static class RandomHelper
    {
        public static T Element<T>(IEnumerable<T> enumerable)
        {
            T[] elements = enumerable.ToArray();

            return elements[Random.Range(0, elements.Length)];
        }
    }
}
