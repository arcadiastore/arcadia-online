using System.Collections;
using UnityEngine;

namespace ArcadiaOnline.CameraSystem
{
    /// <summary>Lihat docs/02_TDD/Camera.md - Camera Shake.</summary>
    public class CameraShake : MonoBehaviour
    {
        private Coroutine _shakeRoutine;
        private Vector3 _originalLocalPosition;

        public void Shake(float intensity, float duration)
        {
            if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
            _originalLocalPosition = transform.localPosition;
            _shakeRoutine = StartCoroutine(ShakeCoroutine(intensity, duration));
        }

        private IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;
                transform.localPosition = _originalLocalPosition + new Vector3(x, y, 0f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = _originalLocalPosition;
        }
    }
}
