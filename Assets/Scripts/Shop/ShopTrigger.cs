using UnityEngine;

namespace ArcadiaOnline.Shop
{
    /// <summary>
    /// Trigger untuk membuka shop saat player dekat NPC.
    /// Attach ke GameObject NPC yang punya shop.
    /// </summary>
    public class ShopTrigger : MonoBehaviour
    {
        [Header("Shop Settings")]
        [SerializeField] private string shopID;             // ID shop yang akan dibuka

        [Header("Interaction")]
        [SerializeField] private float interactDistance = 3f;
        [SerializeField] private KeyCode interactKey = KeyCode.F;

        [Header("Visual Feedback")]
        [SerializeField] private bool showInteractPrompt = true;
        [SerializeField] private string promptText = "Press F to Shop";

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        // State
        private bool playerInRange = false;
        private GameObject player;

        void Start()
        {
            // Find player
            player = GameObject.FindGameObjectWithTag("Player");

            if (showDebug)
            {
                Debug.Log($"[ShopTrigger] Initialized for shop: {shopID}");
            }
        }

        void Update()
        {
            if (player == null) return;

            // Cek jarak player
            float distance = Vector3.Distance(transform.position, player.transform.position);
            playerInRange = distance <= interactDistance;

            // Interact
            if (playerInRange && Input.GetKeyDown(interactKey))
            {
                OpenShop();
            }
        }

        /// <summary>
        /// Open shop.
        /// </summary>
        private void OpenShop()
        {
            if (ShopManager.Instance == null)
            {
                Debug.LogError("[ShopTrigger] ShopManager not found!");
                return;
            }

            if (string.IsNullOrEmpty(shopID))
            {
                Debug.LogWarning("[ShopTrigger] Shop ID not set!");
                return;
            }

            // Open shop
            bool success = ShopManager.Instance.OpenShop(shopID);

            if (!success)
            {
                Debug.LogWarning($"[ShopTrigger] Failed to open shop: {shopID}");
            }
        }

        /// <summary>
        /// Set shop ID.
        /// </summary>
        public void SetShopID(string id)
        {
            shopID = id;
        }

        /// <summary>
        /// Get shop ID.
        /// </summary>
        public string GetShopID()
        {
            return shopID;
        }

        void OnDrawGizmosSelected()
        {
            // Draw interact range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactDistance);
        }

        void OnGUI()
        {
            if (!showInteractPrompt || !playerInRange) return;

            // Draw interact prompt
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 16;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            string prompt = $"[F] {promptText}";

            // Position at screen center-bottom
            float width = 200;
            float height = 40;
            float x = (Screen.width - width) / 2;
            float y = Screen.height - 100;

            GUI.Box(new Rect(x, y, width, height), prompt, style);
        }
    }
}
