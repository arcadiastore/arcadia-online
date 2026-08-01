using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.VFX
{
    /// <summary>
    /// Damage popup yang otomatis menyesuaikan ukuran body monster.
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
        [SerializeField] private float canvasScaleMultiplier = 0.008f; // Sangat kecil
        [SerializeField] private int normalFontSize = 8;
        [SerializeField] private int criticalFontSize = 10;

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
        /// Spawn damage popup di posisi target dengan ukuran sesuai body.
        /// </summary>
        public void SpawnDamagePopup(Vector3 targetPosition, float damage, bool isCritical, Transform target = null)
        {
            // Hitung ukuran body monster
            float bodySize = 1f; // Default
            if (target != null)
            {
                Renderer renderer = target.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    // Ambil ukuran bounds monster
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

            // Hitung canvas scale berdasarkan body size
            float canvasScale = bodySize * canvasScaleMultiplier;

            // Buat Canvas World Space
            GameObject canvasObj = new GameObject("DamageCanvas");
            canvasObj.transform.position = spawnPos;
            canvasObj.transform.localScale = Vector3.one * canvasScale;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100;

            // Buat Text child
            GameObject textObj = new GameObject("DamageText");
            textObj.transform.SetParent(canvasObj.transform, false);

            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 30);
            rectTransform.anchoredPosition = Vector2.zero;

            // Tambah Text component
            Text text = textObj.AddComponent<Text>();
            text.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            // Set text dan warna
            if (isCritical)
            {
                text.text = Mathf.CeilToInt(damage).ToString() + "!";
                text.color = new Color(0.6f, 0f, 0.8f); // Ungu
                text.fontSize = criticalFontSize;
                text.fontStyle = FontStyle.Bold;
            }
            else
            {
                text.text = Mathf.CeilToInt(damage).ToString();
                text.color = Color.red;
                text.fontSize = normalFontSize;
                text.fontStyle = FontStyle.Bold;
            }

            // Tambah script animasi
            DamagePopupAnim anim = canvasObj.AddComponent<DamagePopupAnim>();
            anim.Setup(text, moveSpeed, lifetime);

            // Face camera
            if (Camera.main != null)
            {
                canvasObj.transform.LookAt(Camera.main.transform);
                canvasObj.transform.Rotate(0, 180, 0);
            }

            Debug.Log($"[DamagePopup] Spawned: {damage} (Critical: {isCritical}, BodySize: {bodySize}, Scale: {canvasScale})");
        }
    }

    /// <summary>
    /// Animasi untuk damage popup.
    /// </summary>
    public class DamagePopupAnim : MonoBehaviour
    {
        private Text text;
        private float moveSpeed;
        private float lifetime;
        private float elapsed = 0f;
        private Color startColor;

        public void Setup(Text text, float moveSpeed, float lifetime)
        {
            this.text = text;
            this.moveSpeed = moveSpeed;
            this.lifetime = lifetime;
            this.startColor = text.color;
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
            if (fadeProgress < 1f && text != null)
            {
                Color c = startColor;
                c.a = 1f - fadeProgress;
                text.color = c;
            }

            // Destroy setelah lifetime
            if (elapsed >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
