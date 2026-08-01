using UnityEngine;
using TMPro;

namespace ArcadiaOnline.VFX
{
    /// <summary>
    /// Damage popup menggunakan TextMeshPro (lebih reliable untuk 3D).
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
        [SerializeField] private float normalFontSize = 0.3f;
        [SerializeField] private float criticalFontSize = 0.4f;

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

            // Buat GameObject
            GameObject popupObj = new GameObject("DamagePopup");
            popupObj.transform.position = spawnPos;

            // Tambah TextMeshPro
            TextMeshPro textMeshPro = popupObj.AddComponent<TextMeshPro>();
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textMeshPro.enableWordWrapping = false;
            textMeshPro.overflowMode = TextOverflowModes.Overflow;

            // Set text dan warna
            if (isCritical)
            {
                textMeshPro.text = Mathf.CeilToInt(damage).ToString() + "!";
                textMeshPro.color = new Color(0.6f, 0f, 0.8f); // Ungu
                textMeshPro.fontSize = criticalFontSize;
                textMeshPro.fontStyle = FontStyles.Bold;
            }
            else
            {
                textMeshPro.text = Mathf.CeilToInt(damage).ToString();
                textMeshPro.color = Color.red;
                textMeshPro.fontSize = normalFontSize;
                textMeshPro.fontStyle = FontStyles.Bold;
            }

            // Tambah script animasi
            DamagePopupAnim anim = popupObj.AddComponent<DamagePopupAnim>();
            anim.Setup(textMeshPro, moveSpeed, lifetime);

            // Face camera
            if (Camera.main != null)
            {
                popupObj.transform.LookAt(Camera.main.transform);
                popupObj.transform.Rotate(0, 180, 0);
            }

            Debug.Log($"[DamagePopup] Spawned: {damage} (Critical: {isCritical})");
        }
    }

    /// <summary>
    /// Animasi untuk damage popup.
    /// </summary>
    public class DamagePopupAnim : MonoBehaviour
    {
        private TextMeshPro textMeshPro;
        private float moveSpeed;
        private float lifetime;
        private float elapsed = 0f;
        private Color startColor;

        public void Setup(TextMeshPro textMeshPro, float moveSpeed, float lifetime)
        {
            this.textMeshPro = textMeshPro;
            this.moveSpeed = moveSpeed;
            this.lifetime = lifetime;
            this.startColor = textMeshPro.color;
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
            if (fadeProgress < 1f && textMeshPro != null)
            {
                Color c = startColor;
                c.a = 1f - fadeProgress;
                textMeshPro.color = c;
            }

            // Destroy setelah lifetime
            if (elapsed >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
