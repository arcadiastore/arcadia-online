using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.VFX
{
    /// <summary>
    /// Damage popup menggunakan Canvas World Space (lebih reliable).
    /// </summary>
    public class DamagePopupSpawner : MonoBehaviour
    {
        public static DamagePopupSpawner Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float spawnHeight = 1.5f; // Lebih dekat ke monster
        [SerializeField] private float randomOffset = 0.3f;
        [SerializeField] private float moveSpeed = 1.5f; // Lebih lambat
        [SerializeField] private float lifetime = 1.2f; // Lebih singkat

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
        public void SpawnDamagePopup(Vector3 targetPosition, float damage, bool isCritical)
        {
            Vector3 spawnPos = targetPosition + Vector3.up * spawnHeight;
            spawnPos += new Vector3(
                Random.Range(-randomOffset, randomOffset),
                0,
                Random.Range(-randomOffset, randomOffset)
            );

            // Buat Canvas World Space
            GameObject canvasObj = new GameObject("DamageCanvas");
            canvasObj.transform.position = spawnPos;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100; // Di atas semua

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 100;

            // Buat Text
            GameObject textObj = new GameObject("DamageText");
            textObj.transform.SetParent(canvasObj.transform, false);

            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(80, 30); // Lebih kecil
            rectTransform.anchoredPosition = Vector2.zero;

            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 16; // Lebih kecil
            text.alignment = TextAnchor.MiddleCenter;

            // Set warna dan text
            if (isCritical)
            {
                text.text = Mathf.CeilToInt(damage).ToString() + "!";
                text.color = new Color(0.6f, 0f, 0.8f); // Ungu
                text.fontSize = 20; // Sedikit lebih besar untuk critical
                text.fontStyle = FontStyle.Bold;
            }
            else
            {
                text.text = Mathf.CeilToInt(damage).ToString();
                text.color = Color.red;
                text.fontSize = 16;
                text.fontStyle = FontStyle.Bold;
            }

            // Tambah script untuk animasi
            DamagePopupAnim anim = canvasObj.AddComponent<DamagePopupAnim>();
            anim.Setup(text, moveSpeed, lifetime);

            // Face camera
            if (Camera.main != null)
            {
                canvasObj.transform.LookAt(Camera.main.transform);
                canvasObj.transform.Rotate(0, 180, 0);
            }

            Debug.Log($"[DamagePopup] Spawned: {damage} (Critical: {isCritical})");
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
