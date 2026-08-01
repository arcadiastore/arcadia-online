using UnityEngine;
using ArcadiaOnline.Managers;

namespace ArcadiaOnline.World
{
    /// <summary>
    /// Trigger BGM saat player masuk area tertentu.
    /// Attach ke GameObject dengan Collider (isTrigger = true).
    /// </summary>
    public class MapBGMTrigger : MonoBehaviour
    {
        [Header("Area")]
        [SerializeField] private string mapName = "BeginnerVillage";

        [Header("Visual (Optional)")]
        [SerializeField] private bool showGizmo = true;
        [SerializeField] private Color gizmoColor = new Color(0, 1, 0, 0.3f);

        private void OnTriggerEnter(Collider other)
        {
            // Cek apakah yang masuk adalah Player
            if (other.CompareTag("Player"))
            {
                Debug.Log($"[MapBGM] Player masuk area: {mapName}");

                if (MapBGMManager.Instance != null)
                {
                    MapBGMManager.Instance.PlayBGM(mapName);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!showGizmo) return;

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(transform.position, col.bounds.size);
            }
        }
    }
}
