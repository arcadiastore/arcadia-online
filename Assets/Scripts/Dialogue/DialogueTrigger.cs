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
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private bool showInteractPrompt = true;

        [Header("Quest")]
        [SerializeField] private string questID;

        // State
        private bool isPlayerNear;
        private GameObject player;

        void Start()
        {
            // Find player
            player = GameObject.FindGameObjectWithTag("Player");
        }

        void Update()
        {
            if (player == null) return;

            // Cek jarak player
            float distance = Vector3.Distance(transform.position, player.transform.position);
            isPlayerNear = distance <= interactRange;

            // Input interact
            if (isPlayerNear && Input.GetKeyDown(interactKey))
            {
                Interact();
            }
        }

        /// <summary>
        /// Interact dengan NPC.
        /// </summary>
        public void Interact()
        {
            if (DialogueManager.Instance == null) return;

            // Cek apakah dialogue sedang aktif
            if (DialogueManager.Instance.IsDialogueActive()) return;

            // Get dialogue yang sesuai
            DialogueData dialogue = GetAppropriateDialogue();
            if (dialogue == null)
            {
                Debug.Log($"[Dialogue] Tidak ada dialogue untuk {npcName}");
                return;
            }

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
