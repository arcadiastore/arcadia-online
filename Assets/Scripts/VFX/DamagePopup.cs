using UnityEngine;

namespace ArcadiaOnline.VFX
{
    /// <summary>
    /// Damage popup yang muncul di atas monster saat kena hit.
    /// Angka naik ke atas lalu hilang.
    /// </summary>
    public class DamagePopup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float lifetime = 1.5f;

        private TextMesh textMesh;
        private Color startColor;
        private float elapsed = 0f;
        private Vector3 startPos;

        void Awake()
        {
            textMesh = GetComponent<TextMesh>();
            if (textMesh == null)
            {
                textMesh = gameObject.AddComponent<TextMesh>();
            }
        }

        void Start()
        {
            startPos = transform.position;
            startColor = textMesh.color;
        }

        void Update()
        {
            elapsed += Time.deltaTime;

            // Bergerak ke atas
            transform.position = startPos + Vector3.up * (moveSpeed * elapsed);

            // Fade out
            float fadeProgress = elapsed / fadeDuration;
            if (fadeProgress < 1f)
            {
                Color c = startColor;
                c.a = 1f - fadeProgress;
                textMesh.color = c;
            }

            // Destroy setelah lifetime
            if (elapsed >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Setup damage popup dengan damage dan critical status.
        /// </summary>
        public void Setup(float damage, bool isCritical)
        {
            if (textMesh == null)
            {
                textMesh = GetComponent<TextMesh>();
            }

            // Set text
            textMesh.text = Mathf.CeilToInt(damage).ToString();

            // Set warna berdasarkan critical
            if (isCritical)
            {
                // Critical = Ungu
                textMesh.color = new Color(0.6f, 0f, 0.8f); // Ungu
                textMesh.fontSize = 80; // Lebih besar
                textMesh.fontStyle = FontStyle.Bold;
            }
            else
            {
                // Normal = Merah
                textMesh.color = Color.red;
                textMesh.fontSize = 60;
                textMesh.fontStyle = FontStyle.Bold;
            }

            startColor = textMesh.color;

            // Scale effect (muncul besar lalu kecil)
            StartCoroutine(ScaleEffect(isCritical));
        }

        private System.Collections.IEnumerator ScaleEffect(bool isCritical)
        {
            float duration = 0.2f;
            float elapsed = 0f;
            float startScale = isCritical ? 0.03f : 0.02f;
            float endScale = isCritical ? 0.02f : 0.015f;

            transform.localScale = Vector3.one * startScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scale = Mathf.Lerp(startScale, endScale, t);
                transform.localScale = Vector3.one * scale;
                yield return null;
            }

            transform.localScale = Vector3.one * endScale;
        }
    }
}
