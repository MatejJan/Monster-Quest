using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public static class CoroutineHelper
    {
        public static IEnumerator WaitForAll(this IEnumerable<IEnumerator> enumerators, MonoBehaviour monoBehaviour)
        {
            List<Coroutine> coroutines = new();

            foreach (IEnumerator enumerator in enumerators)
            {
                coroutines.Add(monoBehaviour.StartCoroutine(enumerator));
            }

            // Wait until all presenters have presented this event.
            foreach (Coroutine coroutine in coroutines)
            {
                yield return coroutine;
            }
        }
    }
}
