using UnityEngine;

namespace ArcadiaOnline.CameraSystem
{
    /// <summary>
    /// Third Person Follow + rotasi bebas. Lihat docs/02_TDD/Camera.md.
    /// Mode Lock-On dan Cutscene sebaiknya diimplementasikan sebagai
    /// state terpisah yang mengubah target/behaviour komponen ini.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _distance = 5f;
        [SerializeField] private float _height = 2f;
        [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 3f;
        [SerializeField] private float _minPitch = -30f;
        [SerializeField] private float _maxPitch = 60f;

        [SerializeField] private CameraCollision _collision;

        private float _yaw;
        private float _pitch;

        public void SetTarget(Transform target) => _target = target;

        private void LateUpdate()
        {
            if (_target == null) return;

            _yaw += Input.GetAxis("Mouse X") * _rotationSpeed;
            _pitch -= Input.GetAxis("Mouse Y") * _rotationSpeed;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            float distance = _collision != null ? _collision.GetAdjustedDistance(_target, transform, _distance) : _distance;
            Vector3 offset = rotation * new Vector3(0f, _height, -distance);
            Vector3 targetPosition = _target.position + offset;

            transform.position = Vector3.Lerp(
                transform.position, targetPosition, _smoothSpeed * Time.deltaTime);
            transform.LookAt(_target.position + Vector3.up * (_height * 0.5f));
        }
    }
}
