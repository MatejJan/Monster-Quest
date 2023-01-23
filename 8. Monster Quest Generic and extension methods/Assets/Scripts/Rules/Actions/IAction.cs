using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public interface IAction
    {
        IEnumerator Execute();
    }
}
