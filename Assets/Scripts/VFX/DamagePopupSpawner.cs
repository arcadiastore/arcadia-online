using UnityEngine;

namespace ArcadiaOnline.VFX
{
    /// <summary>
    /// Damage popup menggunakan 3D Text Mesh langsung.
    /// Pasti terlihat karena pakai mesh renderer.
    /// </summary>
    public class DamagePopupSpawner : MonoBehaviour
    {
        public static DamagePopupSpawner Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float spawnHeight = 1f;
        [SerializeField] private float randomOffset = 0.15f;
        [SerializeField] private float moveSpeed = 0.8f;
        [SerializeField] private float lifetime = 0.8f;

        [Header("Ukuran")]
        [SerializeField] private float characterSize = 0.2f; // Ukuran karakter 3D
        [SerializeField] private int normalFontSize = 60;
        [SerializeField] private int criticalFontSize = 80;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Spawn damage popup di posisi target.
        /// </summary>
        public void SpawnDamagePopup(Vector3 targetPosition, float damage, bool isCritical, Transform target = null)
        {
            // Hitung ukuran body monster
            float bodySize = 1f;
            if (target != null)
            {
                Renderer renderer = target.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    Vector3 size = renderer.bounds.size;
                    bodySize = Mathf.Max(size.x, size.y, size.z);
                }
            }

            // Hitung spawn position di atas monster
            Vector3 spawnPos = targetPosition + Vector3.up * (bodySize * 1.2f + spawnHeight);
            spawnPos += new Vector3(
                Random.Range(-randomOffset, randomOffset),
                0,
                Random.Range(-randomOffset, randomOffset)
            );

            // Buat GameObject
            GameObject popupObj = new GameObject("DamagePopup");
            popupObj.transform.position = spawnPos;

            // Tambah TextMesh (3D text)
            TextMesh textMesh = popupObj.AddComponent<TextMesh>();
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;

            // Set font default
            textMesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (textMesh.font == null)
            {
                textMesh.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            // Set material untuk font
            Renderer textRenderer = popupObj.GetComponent<Renderer>();
            if (textRenderer != null)
            {
                textRenderer.material = new Material(Shader.Find("GUI/Text Shader"));
            }

            // Hitung character size berdasarkan body
            float charSize = bodySize * 0.1f; // 10% dari body size

            // Set text dan warna
            if (isCritical)
            {
                textMesh.text = Mathf.CeilToInt(damage).ToString() + "!";
                textMesh.color = new Color(0.6f, 0f, 0.8f); // Ungu
                textMesh.characterSize = charSize * 1.3f; // Sedikit lebih besar untuk critical
                textMesh.fontSize = criticalFontSize;
                textMesh.fontStyle = FontStyle.Bold;
            }
            else
            {
                textMesh.text = Mathf.CeilToInt(damage).ToString();
                textMesh.color = Color.red;
                textMesh.characterSize = charSize;
                textMesh.fontSize = normalFontSize;
                textMesh.fontStyle = FontStyle.Bold;
            }

            // Tambah script animasi
            DamagePopupAnim anim = popupObj.AddComponent<DamagePopupAnim>();
            anim.Setup(textMesh, moveSpeed, lifetime);

            // Face camera
            if (Camera.main != null)
            {
                popupObj.transform.LookAt(Camera.main.transform);
                popupObj.transform.Rotate(0, 180, 0);
            }

            Debug.Log($"[DamagePopup] Spawned: {damage} (Critical: {isCritical}, BodySize: {bodySize}, CharSize: {charSize})");
        }
    }

    /// <summary>
    /// Animasi untuk damage popup.
    /// </summary>
    public class DamagePopupAnim : MonoBehaviour
    {
        private TextMesh textMesh;
        private float moveSpeed;
        private float lifetime;
        private float elapsed = 0f;
        private Color startColor;

        public void Setup(TextMesh textMesh, float moveSpeed, float lifetime)
        {
            this.textMesh = textMesh;
            this.moveSpeed = moveSpeed;
            this.lifetime = lifetime;
            this.startColor = textMesh.color;
        }

        void Update()
        {
            elapsed += Time.deltaTime;

            // Bergerak ke atas
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);

            // Face camera terus
            if (Camera.main != null)
            {
                transform.LookAt(Camera.main.transform);
                transform.Rotate(0, 180, 0);
            }

            // Fade out
            float fadeProgress = elapsed / lifetime;
            if (fadeProgress < 1f && textMesh != null)
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
    }
}
