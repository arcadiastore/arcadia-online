using UnityEngine;
using System.Collections.Generic;

namespace ArcadiaOnline.Dialogue
{
    /// <summary>
    /// Trigger untuk memulai dialogue dengan NPC.
    /// Attach ke NPC GameObject.
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("NPC Info")]
        [SerializeField] private string npcName = "NPC";
        [SerializeField] private Sprite npcPortrait;

        [Header("Dialogue Data")]
        [SerializeField] private List<DialogueData> dialogues;

        [Header("Interaction")]
        [SerializeField] private KeyCode interactKey = KeyCode.F;
        [SerializeField] private float interactRange = 5f; // Increased range
        [SerializeField] private bool showInteractPrompt = true;

        [Header("Quest")]
        [SerializeField] private string questID;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        // State
        private bool isPlayerNear;
        private GameObject player;

        void Start()
        {
            // Find player by tag or name
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                // Try to find by name
                player = GameObject.Find("Player");
                if (player == null)
                {
                    // Try to find any player controller
                    MonoBehaviour[] scripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
                    foreach (MonoBehaviour script in scripts)
                    {
                        if (script.GetType().Name.Contains("PlayerController"))
                        {
                            player = script.gameObject;
                            break;
                        }
                    }
                }
            }

            if (player == null)
            {
                Debug.LogWarning("[DialogueTrigger] Player not found! Make sure Player has 'Player' tag or name.");
            }
            else
            {
                Debug.Log($"[DialogueTrigger] Player found: {player.name}");
            }

            // Log dialogue status
            if (dialogues == null || dialogues.Count == 0)
            {
                Debug.LogWarning($"[DialogueTrigger] No dialogues assigned to {npcName}!");
            }
            else
            {
                Debug.Log($"[DialogueTrigger] {npcName} has {dialogues.Count} dialogue(s)");
            }
        }

        void Update()
        {
            if (player == null) return;

            // Cek jarak player
            float distance = Vector3.Distance(transform.position, player.transform.position);
            isPlayerNear = distance <= interactRange;

            // Debug log
            if (showDebug && isPlayerNear)
            {
                // Only log when near
            }

            // Input interact
            if (isPlayerNear && Input.GetKeyDown(interactKey))
            {
                Debug.Log($"[DialogueTrigger] F pressed! Interacting with {npcName}");
                Interact();
            }
        }

        /// <summary>
        /// Interact dengan NPC.
        /// </summary>
        public void Interact()
        {
            if (DialogueManager.Instance == null)
            {
                Debug.LogError("[DialogueTrigger] DialogueManager.Instance is null!");
                return;
            }

            // Cek apakah dialogue sedang aktif
            if (DialogueManager.Instance.IsDialogueActive())
            {
                Debug.Log("[DialogueTrigger] Dialogue already active!");
                return;
            }

            // Get dialogue yang sesuai
            DialogueData dialogue = GetAppropriateDialogue();
            if (dialogue == null)
            {
                Debug.Log($"[DialogueTrigger] Tidak ada dialogue untuk {npcName}");
                return;
            }

            Debug.Log($"[DialogueTrigger] Starting dialogue: {dialogue.dialogueID}");

            // Start dialogue
            DialogueManager.Instance.StartDialogue(dialogue);

            // Register event handler
            DialogueManager.Instance.OnDialogueEvent += HandleDialogueEvent;
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
        }

        /// <summary>
        /// Get dialogue yang sesuai berdasarkan kondisi.
        /// </summary>
        private DialogueData GetAppropriateDialogue()
        {
            if (dialogues == null || dialogues.Count == 0) return null;

            // TODO: Implementasi logic pemilihan dialogue
            // Misalnya berdasarkan quest progress, level, dll

            // Untuk sekarang, return dialogue pertama
            return dialogues[0];
        }

        /// <summary>
        /// Handle dialogue event.
        /// </summary>
        private void HandleDialogueEvent(string eventName)
        {
            Debug.Log($"[Dialogue] Event: {eventName}");

            // Implementasi event handler
            // Misalnya: start quest, give item, open shop, dll
        }

        /// <summary>
        /// On dialogue end.
        /// </summary>
        private void OnDialogueEnd()
        {
            // Unregister event handler
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueEvent -= HandleDialogueEvent;
                DialogueManager.Instance.OnDialogueEnd -= OnDialogueEnd;
            }
        }

        /// <summary>
        /// Cek apakah player sedang dekat.
        /// </summary>
        public bool IsPlayerNear()
        {
            return isPlayerNear;
        }

        /// <summary>
        /// Get NPC name.
        /// </summary>
        public string GetNPCName()
        {
            return npcName;
        }

        void OnDrawGizmosSelected()
        {
            // Draw interaction range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}
