using UnityEngine;

namespace ArcadiaOnline.VFX
{
    /// <summary>
    /// Spawn damage popup di atas target.
    /// </summary>
    public class DamagePopupSpawner : MonoBehaviour
    {
        public static DamagePopupSpawner Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float spawnHeight = 3f; // Tinggi di atas target (lebih tinggi)
        [SerializeField] private float randomOffset = 0.5f; // Random X/Z offset

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

            // Tambah TextMesh
            TextMesh textMesh = popupObj.AddComponent<TextMesh>();
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;

            // Set font
            textMesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (textMesh.font == null)
            {
                textMesh.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            // Set material untuk font
            Renderer renderer = popupObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("GUI/Text Shader"));
            }

            // Set warna dan ukuran
            if (isCritical)
            {
                textMesh.text = Mathf.CeilToInt(damage).ToString() + "!";
                textMesh.color = new Color(0.6f, 0f, 0.8f); // Ungu
                textMesh.characterSize = 0.15f;
                textMesh.fontStyle = FontStyle.Bold;
            }
            else
            {
                textMesh.text = Mathf.CeilToInt(damage).ToString();
                textMesh.color = Color.red;
                textMesh.characterSize = 0.1f;
                textMesh.fontStyle = FontStyle.Bold;
            }

            // Tambah script DamagePopup
            DamagePopup popup = popupObj.AddComponent<DamagePopup>();
            popup.Setup(damage, isCritical);

            Debug.Log($"[DamagePopup] Spawned: {damage} (Critical: {isCritical})");
        }
    }
}
