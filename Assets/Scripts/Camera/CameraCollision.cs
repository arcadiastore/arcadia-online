using UnityEngine;

namespace ArcadiaOnline.CameraSystem
{
    /// <summary>Lihat docs/02_TDD/Camera.md - Camera Collision.</summary>
    public class CameraCollision : MonoBehaviour
    {
        [SerializeField] private float _minDistance = 1f;
        [SerializeField] private LayerMask _collisionLayer;

        public float GetAdjustedDistance(Transform target, Transform cameraTransform, float maxDistance)
        {
            if (Physics.Raycast(target.position, -cameraTransform.forward,
                out RaycastHit hit, maxDistance, _collisionLayer))
            {
                return Mathf.Clamp(hit.distance, _minDistance, maxDistance);
            }
            return maxDistance;
        }
    }
}
