using System.Collections;
using UnityEngine;

namespace MonsterQuest
{
    public class DicePresenter : MonoBehaviour
    {
        [SerializeField] private GameObject d20Prefab;
        private GameObject _lastDiceObject;

        public IEnumerator RollD20(int result, Vector3 position)
        {
            yield return Roll(d20Prefab, result, position);
        }

        private IEnumerator Roll(GameObject prefab, int result, Vector3 position)
        {
            // Place the dice on the correct depth layer.
            position.z = transform.position.z;

            // Instantiate the dice prefab at specified position.
            _lastDiceObject = Instantiate(prefab, position, Quaternion.identity, transform);

            DiceRollPresenter diceRollPresenter = _lastDiceObject.GetComponent<DiceRollPresenter>();

            yield return diceRollPresenter.Roll(result);
        }

        public void EndRoll()
        {
            Destroy(_lastDiceObject);
            _lastDiceObject = null;
        }
    }
}
