using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public class HitPointsPresenter : MonoBehaviour
    {
        private float _positionY;
        private Transform _miniatureTransform;

        private void Awake()
        {
            _positionY = transform.position.y;
            _miniatureTransform = transform.parent.Find("Orientation").Find("Miniature");
        }

        private void LateUpdate()
        {
            // Align with miniature position.
            Vector3 position = _miniatureTransform.position;
            position.y = _positionY;
            transform.position = position;
        }
    }
}
