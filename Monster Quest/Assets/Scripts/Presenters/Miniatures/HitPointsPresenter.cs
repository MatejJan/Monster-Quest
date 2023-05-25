using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public class HitPointsPresenter : MonoBehaviour
    {
        private float _positionY;
        private Transform _standTransform;

        private void Awake()
        {
            _positionY = transform.position.y;
            _standTransform = transform.parent.Find("Orientation").Find("Miniature").Find("Stand");
        }

        private void LateUpdate()
        {
            // Align with stand position.
            Vector3 position = _standTransform.position;
            position.y = _positionY;
            transform.position = position;
        }
    }
}
