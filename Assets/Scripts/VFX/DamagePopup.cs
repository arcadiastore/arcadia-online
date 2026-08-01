using UnityEngine;

namespace ArcadiaOnline.VFX
{
    /// <summary>
    /// Damage popup yang muncul di atas monster saat kena hit.
    /// Menggunakan TextMesh dengan font default.
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
            
            // Set default font
            textMesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (textMesh.font == null)
            {
                textMesh.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            
            // Set material untuk font
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("GUI/Text Shader"));
            }
        }

        void Start()
        {
            startPos = transform.position;
            startColor = textMesh.color;

            // Face camera
            if (Camera.main != null)
            {
                transform.LookAt(Camera.main.transform);
                transform.Rotate(0, 180, 0);
            }
        }

        void Update()
        {
            elapsed += Time.deltaTime;

            // Bergerak ke atas
            transform.position = startPos + Vector3.up * (moveSpeed * elapsed);

            // Face camera terus
            if (Camera.main != null)
            {
                transform.LookAt(Camera.main.transform);
                transform.Rotate(0, 180, 0);
            }

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

            // Set font jika belum ada
            if (textMesh.font == null)
            {
                textMesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            // Set text
            if (isCritical)
            {
                textMesh.text = Mathf.CeilToInt(damage).ToString() + "!";
            }
            else
            {
                textMesh.text = Mathf.CeilToInt(damage).ToString();
            }

            // Set warna berdasarkan critical
            if (isCritical)
            {
                // Critical = Ungu
                textMesh.color = new Color(0.6f, 0f, 0.8f); // Ungu
                textMesh.characterSize = 0.15f; // Lebih besar
                textMesh.fontStyle = FontStyle.Bold;
            }
            else
            {
                // Normal = Merah
                textMesh.color = Color.red;
                textMesh.characterSize = 0.1f;
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
            float startScale = isCritical ? 1.5f : 1f;
            float endScale = 1f;

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
