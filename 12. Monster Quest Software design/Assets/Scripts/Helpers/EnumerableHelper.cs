using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    public static class EnumerableHelper
    {
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            T[] elements = enumerable.ToArray();
            return elements[UnityEngine.Random.Range(0, elements.Length)];
        }
    }
}
