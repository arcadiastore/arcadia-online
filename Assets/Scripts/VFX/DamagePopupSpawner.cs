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
        [SerializeField] private GameObject damagePopupPrefab;
        [SerializeField] private float spawnHeight = 1.5f; // Tinggi di atas target
        [SerializeField] private float randomOffset = 0.3f; // Random X/Z offset

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
            // Jika tidak ada prefab, buat secara programmatic
            if (damagePopupPrefab == null)
            {
                SpawnProgrammaticPopup(targetPosition, damage, isCritical);
                return;
            }

            // Spawn dari prefab
            Vector3 spawnPos = targetPosition + Vector3.up * spawnHeight;
            spawnPos += new Vector3(
                Random.Range(-randomOffset, randomOffset),
                0,
                Random.Range(-randomOffset, randomOffset)
            );

            GameObject popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null)
            {
                popupScript.Setup(damage, isCritical);
            }
        }

        /// <summary>
        /// Buat damage popup secara programmatic (tanpa prefab).
        /// </summary>
        private void SpawnProgrammaticPopup(Vector3 targetPosition, float damage, bool isCritical)
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

            // Set warna dan ukuran
            if (isCritical)
            {
                textMesh.text = Mathf.CeilToInt(damage).ToString() + "!";
                textMesh.color = new Color(0.6f, 0f, 0.8f); // Ungu
                textMesh.fontSize = 80;
                textMesh.fontStyle = FontStyle.Bold;
            }
            else
            {
                textMesh.text = Mathf.CeilToInt(damage).ToString();
                textMesh.color = Color.red;
                textMesh.fontSize = 60;
                textMesh.fontStyle = FontStyle.Bold;
            }

            // Tambah script DamagePopup
            DamagePopup popup = popupObj.AddComponent<DamagePopup>();
            popup.Setup(damage, isCritical);

            // Face camera
            popupObj.transform.LookAt(Camera.main.transform);
            popupObj.transform.Rotate(0, 180, 0);
        }
    }
}
