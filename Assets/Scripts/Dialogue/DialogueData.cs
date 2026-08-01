using UnityEngine;
using System.Collections.Generic;

namespace ArcadiaOnline.Dialogue
{
    /// <summary>
    /// Tipe dialogue line.
    /// </summary>
    public enum DialogueType
    {
        Normal,     // Dialog biasa
        Choice,     // Pilihan jawaban
        Quest,      // Quest related
        Shop        // Shop related
    }

    /// <summary>
    /// Data satu baris dialog.
    /// </summary>
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;      // Nama NPC
        [TextArea(2, 4)]
        public string text;             // Teks dialog
        public Sprite portrait;         // Portrait NPC (opsional)
        public DialogueType type;       // Tipe dialog

        [Header("Choices (untuk tipe Choice)")]
        public List<DialogueChoice> choices;

        [Header("Events")]
        public string triggerEvent;     // Event yang di-trigger setelah dialog
    }

    /// <summary>
    /// Data pilihan jawaban.
    /// </summary>
    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;       // Teks pilihan
        public string nextDialogueID;   // ID dialog selanjutnya
        public string triggerEvent;     // Event yang di-trigger
    }

    /// <summary>
    /// Data dialogue tree (ScriptableObject).
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Arcadia/Dialogue")]
    public class DialogueData : ScriptableObject
    {
        [Header("Dialogue Info")]
        public string dialogueID;           // ID unik dialog
        public string npcName;              // Nama NPC
        public List<DialogueLine> lines;    // Semua baris dialog

        [Header("Conditions")]
        public int requiredLevel = 1;       // Level minimal
        public string requiredQuest;        // Quest yang harus selesai
        public string requiredItem;         // Item yang dibutuhkan

        [Header("Rewards")]
        public int expReward;               // EXP reward
        public int goldReward;              // Gold reward

        void OnValidate()
        {
            if (lines == null)
                lines = new List<DialogueLine>();
        }

        /// <summary>
        /// Get total lines count.
        /// </summary>
        public int GetLineCount()
        {
            return lines != null ? lines.Count : 0;
        }

        /// <summary>
        /// Get specific line by index.
        /// </summary>
        public DialogueLine GetLine(int index)
        {
            if (lines != null && index >= 0 && index < lines.Count)
                return lines[index];
            return null;
        }
    }
}
