using UnityEngine;
using System.Collections.Generic;

namespace ArcadiaOnline.Quest
{
    /// <summary>
    /// Tipe quest.
    /// </summary>
    public enum QuestType
    {
        Kill,       // Bunuh monster
        Collect,    // Kumpulkan item
        Talk,       // Bicara dengan NPC
        Explore,    // Jelajahi area
        Escort,     // Eskort NPC
        Boss        // Kalahkan boss
    }

    /// <summary>
    /// Status quest.
    /// </summary>
    public enum QuestStatus
    {
        Locked,     // Belum bisa diambil
        Available,  // Bisa diambil
        Active,     // Sedang dikerjakan
        Completed,  // Selesai (belum di-claim)
        Claimed,    // Hadiah sudah di-claim
        Failed      // Gagal
    }

    /// <summary>
    /// Data objective quest.
    /// </summary>
    [System.Serializable]
    public class QuestObjective
    {
        public string description;      // Deskripsi objective
        public QuestType type;          // Tipe objective
        public string targetID;         // ID target (monster, item, NPC)
        public int requiredAmount;      // Jumlah yang dibutuhkan
        public int currentAmount;       // Jumlah saat ini

        /// <summary>
        /// Cek apakah objective selesai.
        /// </summary>
        public bool IsComplete()
        {
            return currentAmount >= requiredAmount;
        }

        /// <summary>
        /// Get progress string.
        /// </summary>
        public string GetProgressString()
        {
            return $"{currentAmount}/{requiredAmount}";
        }
    }

    /// <summary>
    /// Data reward quest.
    /// </summary>
    [System.Serializable]
    public class QuestReward
    {
        public int expReward;           // EXP reward
        public int goldReward;          // Gold reward
        public List<string> itemIDs;    // Item rewards
        public string equipmentID;      // Equipment reward (opsional)
    }

    /// <summary>
    /// Data quest (ScriptableObject).
    /// </summary>
    [CreateAssetMenu(fileName = "New Quest", menuName = "Arcadia/Quest")]
    public class QuestData : ScriptableObject
    {
        [Header("Quest Info")]
        public string questID;              // ID unik quest
        public string questName;            // Nama quest
        [TextArea(3, 6)]
        public string description;          // Deskripsi quest
        public QuestType mainType;          // Tipe quest utama
        public int recommendedLevel = 1;    // Level yang disarankan

        [Header("Quest Chain")]
        public string previousQuestID;      // Quest yang harus selesai dulu
        public string nextQuestID;          // Quest selanjutnya

        [Header("Objectives")]
        public List<QuestObjective> objectives;

        [Header("Rewards")]
        public QuestReward rewards;

        [Header("Time")]
        public bool hasTimeLimit = false;
        public float timeLimitMinutes = 0;

        [Header("Dialogue")]
        public string startDialogueID;      // Dialogue saat mulai quest
        public string completeDialogueID;   // Dialogue saat quest selesai

        void OnValidate()
        {
            if (objectives == null)
                objectives = new List<QuestObjective>();
            if (rewards == null)
                rewards = new QuestReward();
            if (rewards.itemIDs == null)
                rewards.itemIDs = new List<string>();
        }

        /// <summary>
        /// Get total objectives count.
        /// </summary>
        public int GetObjectiveCount()
        {
            return objectives != null ? objectives.Count : 0;
        }

        /// <summary>
        /// Get completed objectives count.
        /// </summary>
        public int GetCompletedObjectiveCount()
        {
            if (objectives == null) return 0;

            int count = 0;
            foreach (var obj in objectives)
            {
                if (obj.IsComplete()) count++;
            }
            return count;
        }

        /// <summary>
        /// Cek apakah semua objective selesai.
        /// </summary>
        public bool IsAllObjectivesComplete()
        {
            return GetCompletedObjectiveCount() == GetObjectiveCount();
        }

        /// <summary>
        /// Get progress string.
        /// </summary>
        public string GetProgressString()
        {
            return $"{GetCompletedObjectiveCount()}/{GetObjectiveCount()}";
        }
    }
}
