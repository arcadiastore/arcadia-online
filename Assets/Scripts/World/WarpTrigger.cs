using UnityEngine;

namespace ArcadiaOnline.World
{
    /// <summary>
    /// Trigger untuk warp player ke lokasi lain.
    /// Attach ke GameObject warp point.
    /// </summary>
    public class WarpTrigger : MonoBehaviour
    {
        [Header("Warp Settings")]
        [SerializeField] private Vector3 destination;
        [SerializeField] private string destinationName;

        [Header("Effects")]
        [SerializeField] private bool showWarpEffect = true;
        [SerializeField] private Color warpColor = Color.cyan;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        // State
        private bool isWarping = false;

        void Start()
        {
            // Make warp point glow
            if (showWarpEffect)
            {
                CreateGlowEffect();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (isWarping) return;

            // Check if player
            if (other.CompareTag("Player"))
            {
                WarpPlayer(other.gameObject);
            }
        }

        /// <summary>
        /// Warp player to destination.
        /// </summary>
        private void WarpPlayer(GameObject player)
        {
            if (player == null) return;

            isWarping = true;

            // Teleport player
            player.transform.position = destination;

            if (showDebug)
            {
                Debug.Log($"[Warp] Warped to {destinationName} ({destination})");
            }

            // Reset warping state after delay
            StartCoroutine(ResetWarping());
        }

        /// <summary>
        /// Reset warping state.
        /// </summary>
        private System.Collections.IEnumerator ResetWarping()
        {
            yield return new WaitForSeconds(1f);
            isWarping = false;
        }

        /// <summary>
        /// Set destination.
        /// </summary>
        public void SetDestination(Vector3 dest)
        {
            destination = dest;
        }

        /// <summary>
        /// Set destination name.
        /// </summary>
        public void SetDestinationName(string name)
        {
            destinationName = name;
        }

        /// <summary>
        /// Create glow effect.
        /// </summary>
        private void CreateGlowEffect()
        {
            // Add point light
            GameObject lightObj = new GameObject("WarpGlow");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = Vector3.up;

            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = warpColor;
            light.intensity = 2f;
            light.range = 5f;
        }

        void OnDrawGizmos()
        {
            // Draw destination
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destination);
            Gizmos.DrawWireSphere(destination, 0.5f);
        }
    }
}
