using UnityEngine;
using System.Collections.Generic;
using ArcadiaOnline.Quest;

namespace ArcadiaOnline.World
{
    /// <summary>
    /// NPC yang memberikan quest.
    /// Attach ke GameObject NPC.
    /// </summary>
    public class QuestGiver : MonoBehaviour
    {
        [Header("Quest Settings")]
        [SerializeField] private List<string> availableQuestIDs = new List<string>();
        [SerializeField] private string currentQuestID;

        [Header("Interaction")]
        [SerializeField] private float interactDistance = 3f;
        [SerializeField] private KeyCode interactKey = KeyCode.F;

        [Header("Visual Feedback")]
        [SerializeField] private bool showInteractPrompt = true;
        [SerializeField] private string promptText = "Press F to Talk";

        [Header("Quest Icons")]
        [SerializeField] private bool showQuestIcon = true;
        [SerializeField] private Color questAvailableColor = Color.yellow;
        [SerializeField] private Color questInProgressColor = Color.white;
        [SerializeField] private Color questCompleteColor = Color.green;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        // State
        private bool playerInRange = false;
        private GameObject player;
        private GameObject questIconObj;

        void Start()
        {
            // Find player
            player = GameObject.FindGameObjectWithTag("Player");

            // Create quest icon
            if (showQuestIcon)
            {
                CreateQuestIcon();
            }

            if (showDebug)
            {
                Debug.Log($"[QuestGiver] Initialized with {availableQuestIDs.Count} quests");
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
                GiveQuest();
            }

            // Update quest icon
            if (showQuestIcon)
            {
                UpdateQuestIcon();
            }
        }

        /// <summary>
        /// Give quest to player.
        /// </summary>
        private void GiveQuest()
        {
            if (QuestManager.Instance == null)
            {
                Debug.LogError("[QuestGiver] QuestManager not found!");
                return;
            }

            // Check if player has active quest from this NPC
            if (!string.IsNullOrEmpty(currentQuestID))
            {
                // Check if quest is active
                if (QuestManager.Instance.IsQuestActive(currentQuestID))
                {
                    // Check if quest is complete
                    if (QuestManager.Instance.IsQuestComplete(currentQuestID))
                    {
                        // Complete quest
                        QuestManager.Instance.CompleteQuest(currentQuestID);
                        Debug.Log($"[QuestGiver] Quest completed: {currentQuestID}");
                    }
                    else
                    {
                        Debug.Log($"[QuestGiver] Quest in progress: {currentQuestID}");
                    }
                    return;
                }
            }

            // Find next available quest
            string nextQuestID = GetNextAvailableQuest();

            if (!string.IsNullOrEmpty(nextQuestID))
            {
                // Accept quest
                bool success = QuestManager.Instance.AcceptQuest(nextQuestID);

                if (success)
                {
                    currentQuestID = nextQuestID;
                    Debug.Log($"[QuestGiver] Quest accepted: {nextQuestID}");
                }
                else
                {
                    Debug.LogWarning($"[QuestGiver] Failed to accept quest: {nextQuestID}");
                }
            }
            else
            {
                Debug.Log("[QuestGiver] No quests available");
            }
        }

        /// <summary>
        /// Get next available quest.
        /// </summary>
        private string GetNextAvailableQuest()
        {
            if (QuestManager.Instance == null) return null;

            foreach (string questID in availableQuestIDs)
            {
                // Check if quest is available (not accepted, not completed)
                if (QuestManager.Instance.IsQuestAvailable(questID))
                {
                    return questID;
                }
            }

            return null;
        }

        /// <summary>
        /// Check if quest is active.
        /// </summary>
        public bool HasActiveQuest()
        {
            return !string.IsNullOrEmpty(currentQuestID);
        }

        /// <summary>
        /// Get current quest ID.
        /// </summary>
        public string GetCurrentQuestID()
        {
            return currentQuestID;
        }

        /// <summary>
        /// Create quest icon above NPC.
        /// </summary>
        private void CreateQuestIcon()
        {
            questIconObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            questIconObj.name = "QuestIcon";
            questIconObj.transform.SetParent(transform);
            questIconObj.transform.localPosition = Vector3.up * 3f;
            questIconObj.transform.localScale = Vector3.one * 0.5f;

            // Remove collider from icon
            Destroy(questIconObj.GetComponent<Collider>());

            // Set initial color
            questIconObj.GetComponent<Renderer>().material.color = questAvailableColor;
        }

        /// <summary>
        /// Update quest icon color based on state.
        /// </summary>
        private void UpdateQuestIcon()
        {
            if (questIconObj == null) return;

            Renderer rend = questIconObj.GetComponent<Renderer>();
            if (rend == null) return;

            // Check quest state
            if (!string.IsNullOrEmpty(currentQuestID) && QuestManager.Instance != null)
            {
                if (QuestManager.Instance.IsQuestComplete(currentQuestID))
                {
                    rend.material.color = questCompleteColor; // Green = ready to complete
                }
                else if (QuestManager.Instance.IsQuestActive(currentQuestID))
                {
                    rend.material.color = questInProgressColor; // White = in progress
                }
                else
                {
                    rend.material.color = questAvailableColor; // Yellow = available
                }
            }
            else
            {
                // Check if any quest is available
                if (GetNextAvailableQuest() != null)
                {
                    rend.material.color = questAvailableColor;
                }
                else
                {
                    // No quests available - hide icon
                    rend.enabled = false;
                }
            }
        }

        /// <summary>
        /// Set available quest IDs.
        /// </summary>
        public void SetAvailableQuests(List<string> questIDs)
        {
            availableQuestIDs = questIDs;
        }

        /// <summary>
        /// Add quest ID.
        /// </summary>
        public void AddQuestID(string questID)
        {
            if (!availableQuestIDs.Contains(questID))
            {
                availableQuestIDs.Add(questID);
            }
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
