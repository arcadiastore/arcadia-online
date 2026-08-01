using UnityEngine;

namespace ArcadiaOnline.Save
{
    /// <summary>
    /// Auto-create save system.
    /// Attach ke GameObject di scene.
    /// </summary>
    public class SaveSampleCreator : MonoBehaviour
    {
        [Header("Auto-Create")]
        [SerializeField] private bool createOnStart = true;

        [Header("Settings")]
        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 300f; // 5 menit
        [SerializeField] private int maxSaveSlots = 3;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        void Start()
        {
            if (createOnStart)
            {
                CreateSaveSystem();
            }
        }

        /// <summary>
        /// Create save system.
        /// </summary>
        public void CreateSaveSystem()
        {
            if (showDebug)
            {
                Debug.Log("[SaveSampleCreator] Creating save system...");
            }

            // Create SaveManager
            if (SaveManager.Instance == null)
            {
                GameObject managerObj = new GameObject("SaveManager");
                SaveManager manager = managerObj.AddComponent<SaveManager>();

                // Set settings via reflection
                SetPrivateField(manager, "autoSave", autoSave);
                SetPrivateField(manager, "autoSaveInterval", autoSaveInterval);
                SetPrivateField(manager, "maxSaveSlots", maxSaveSlots);
                SetPrivateField(manager, "showDebug", showDebug);

                if (showDebug)
                {
                    Debug.Log("[SaveSampleCreator] Created SaveManager");
                }
            }

            // Create SaveUI
            if (SaveUI.Instance == null)
            {
                GameObject uiObj = new GameObject("SaveUI");
                SaveUI ui = uiObj.AddComponent<SaveUI>();

                // Set settings via reflection
                SetPrivateField(ui, "autoCreateUI", true);
                SetPrivateField(ui, "showDebug", showDebug);

                if (showDebug)
                {
                    Debug.Log("[SaveSampleCreator] Created SaveUI");
                }
            }

            if (showDebug)
            {
                Debug.Log("[SaveSampleCreator] Save system created!");
                Debug.Log("[SaveSampleCreator] Controls:");
                Debug.Log("  F5 = Quick Save");
                Debug.Log("  F6 = Open Save/Load UI");
                Debug.Log("  F9 = Quick Load");
            }
        }

        /// <summary>
        /// Set private field via reflection.
        /// </summary>
        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}
