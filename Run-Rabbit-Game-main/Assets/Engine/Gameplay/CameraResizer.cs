#region

using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class CameraResizer
    {
        private readonly Camera _camera;
        private readonly GameObject _roadPrefab;

        public CameraResizer(Camera camera, GameObject roadPrefab)
        {
            _camera = camera;
            _roadPrefab = roadPrefab;
        }

        public void Adjust()
        {
            if (_camera == null || !_camera.orthographic) return;

            float screenAspect = (float)Screen.width / Screen.height;
            float desiredHalfHeight = GetRoadWidth(_roadPrefab) / (2f * screenAspect);

            if (desiredHalfHeight > _camera.orthographicSize)
            {
                _camera.transform.position = new Vector3(0, desiredHalfHeight - _camera.orthographicSize, -10f);
            }
            _camera.orthographicSize = desiredHalfHeight;
        }

        private float GetRoadWidth(GameObject roadPrefab)
        {
            var renderer = roadPrefab.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                return renderer.bounds.size.x;
            }

            var collider = roadPrefab.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                return collider.size.x;
            }

            return 5f;
        }
    }
}
