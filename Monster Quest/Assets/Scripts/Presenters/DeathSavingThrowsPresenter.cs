using System.Collections;
using UnityEngine;

namespace MonsterQuest
{
    public class DeathSavingThrowsPresenter : MonoBehaviour
    {
        private static readonly int _fail = Animator.StringToHash("Fail");

        [SerializeField] private GameObject deathSavingThrowPrefab;

        public void Reset()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            SetXPosition(0);
        }

        public IEnumerator AddDeathSavingThrow(bool success)
        {
            // Instantiate a new death saving throw.
            int index = transform.childCount;
            GameObject deathSavingThrow = Instantiate(deathSavingThrowPrefab, transform);
            deathSavingThrow.transform.localPosition = new Vector3(index, 0, 0);

            // Recenter the UI.
            SetXPosition(-index / 2f);

            // Unless it was a success, animate the failure.
            if (!success)
            {
                deathSavingThrow.GetComponent<Animator>().SetTrigger(_fail);
            }

            yield return new WaitForSeconds(1);
        }

        private void SetXPosition(float x)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.x = x;
            transform.localPosition = localPosition;
        }
    }
}
