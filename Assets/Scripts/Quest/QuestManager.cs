using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ArcadiaOnline.Player;

namespace ArcadiaOnline.Quest
{
    /// <summary>
    /// Manager untuk quest system.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quest Data")]
        [SerializeField] private List<QuestData> allQuests;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        // Quest states
        private Dictionary<string, QuestStatus> questStatuses = new Dictionary<string, QuestStatus>();
        private Dictionary<string, QuestData> activeQuests = new Dictionary<string, QuestData>();

        // Events
        public System.Action<QuestData> OnQuestAccepted;
        public System.Action<QuestData> OnQuestCompleted;
        public System.Action<QuestData> OnQuestClaimed;
        public System.Action<QuestData, QuestObjective> OnObjectiveUpdated;
        public System.Action<QuestData> OnQuestFailed;

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

        void Start()
        {
            // Initialize all quests as locked
            InitializeQuests();
        }

        /// <summary>
        /// Initialize semua quest.
        /// </summary>
        private void InitializeQuests()
        {
            Debug.Log($"[Quest] InitializeQuests called. allQuests count: {(allQuests != null ? allQuests.Count : 0)}");

            if (allQuests == null)
            {
                Debug.LogWarning("[Quest] allQuests is null!");
                return;
            }

            foreach (QuestData quest in allQuests)
            {
                if (quest == null)
                {
                    Debug.LogWarning("[Quest] Quest is null in list!");
                    continue;
                }

                // Set initial status
                if (string.IsNullOrEmpty(quest.previousQuestID))
                {
                    // Quest tanpa prerequisite = Available
                    questStatuses[quest.questID] = QuestStatus.Available;
                    Debug.Log($"[Quest] Set AVAILABLE: {quest.questName} ({quest.questID})");
                }
                else
                {
                    // Quest dengan prerequisite = Locked
                    questStatuses[quest.questID] = QuestStatus.Locked;
                    Debug.Log($"[Quest] Set LOCKED: {quest.questName} (needs: {quest.previousQuestID})");
                }
            }

            Debug.Log($"[Quest] Initialized {allQuests.Count} quests. Statuses count: {questStatuses.Count}");
        }

        /// <summary>
        /// Accept quest.
        /// </summary>
        public bool AcceptQuest(string questID)
        {
            if (!questStatuses.ContainsKey(questID))
            {
                Debug.LogWarning($"[Quest] Quest not found: {questID}");
                return false;
            }

            QuestStatus status = questStatuses[questID];
            if (status != QuestStatus.Available)
            {
                Debug.LogWarning($"[Quest] Quest {questID} is not available (status: {status})");
                return false;
            }

            // Find quest data
            QuestData quest = GetQuestData(questID);
            if (quest == null) return false;

            // Cek level requirement
            if (LevelUpSystem.Instance != null)
            {
                if (LevelUpSystem.Instance.CurrentLevel < quest.recommendedLevel)
                {
                    Debug.Log($"[Quest] Level too low! Need Lv.{quest.recommendedLevel}");
                    return false;
                }
            }

            // Cek previous quest
            if (!string.IsNullOrEmpty(quest.previousQuestID))
            {
                QuestStatus prevStatus = GetQuestStatus(quest.previousQuestID);
                if (prevStatus != QuestStatus.Claimed)
                {
                    Debug.Log($"[Quest] Previous quest not completed: {quest.previousQuestID}");
                    return false;
                }
            }

            // Accept quest
            questStatuses[questID] = QuestStatus.Active;
            activeQuests[questID] = quest;

            // Reset objectives
            foreach (var objective in quest.objectives)
            {
                objective.currentAmount = 0;
            }

            // Callback
            OnQuestAccepted?.Invoke(quest);

            Debug.Log($"[Quest] Accepted: {quest.questName}");
            return true;
        }

        /// <summary>
        /// Update objective progress.
        /// </summary>
        public void UpdateObjective(string questID, QuestType type, string targetID, int amount = 1)
        {
            if (!activeQuests.ContainsKey(questID))
            {
                return;
            }

            QuestData quest = activeQuests[questID];
            if (quest == null) return;

            // Find matching objective
            foreach (var objective in quest.objectives)
            {
                if (objective.type == type && objective.targetID == targetID)
                {
                    // Update amount
                    objective.currentAmount = Mathf.Min(objective.currentAmount + amount, objective.requiredAmount);

                    // Callback
                    OnObjectiveUpdated?.Invoke(quest, objective);

                    Debug.Log($"[Quest] Objective updated: {objective.description} ({objective.GetProgressString()})");

                    // Cek apakah semua objective selesai
                    if (quest.IsAllObjectivesComplete())
                    {
                        CompleteQuest(questID);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Complete quest (semua objective selesai).
        /// </summary>
        private void CompleteQuest(string questID)
        {
            if (!activeQuests.ContainsKey(questID)) return;

            QuestData quest = activeQuests[questID];
            questStatuses[questID] = QuestStatus.Completed;

            // Callback
            OnQuestCompleted?.Invoke(quest);

            Debug.Log($"[Quest] Completed: {quest.questName}!");
        }

        /// <summary>
        /// Claim quest reward.
        /// </summary>
        public bool ClaimReward(string questID)
        {
            if (!questStatuses.ContainsKey(questID))
            {
                return false;
            }

            QuestStatus status = questStatuses[questID];
            if (status != QuestStatus.Completed)
            {
                Debug.LogWarning($"[Quest] Quest {questID} is not completed yet");
                return false;
            }

            QuestData quest = GetQuestData(questID);
            if (quest == null) return false;

            // Give rewards
            GiveRewards(quest);

            // Update status
            questStatuses[questID] = QuestStatus.Claimed;
            activeQuests.Remove(questID);

            // Unlock next quest
            if (!string.IsNullOrEmpty(quest.nextQuestID))
            {
                if (questStatuses.ContainsKey(quest.nextQuestID))
                {
                    questStatuses[quest.nextQuestID] = QuestStatus.Available;
                    Debug.Log($"[Quest] Unlocked: {quest.nextQuestID}");
                }
            }

            // Callback
            OnQuestClaimed?.Invoke(quest);

            Debug.Log($"[Quest] Claimed reward for: {quest.questName}");
            return true;
        }

        /// <summary>
        /// Give rewards to player.
        /// </summary>
        private void GiveRewards(QuestData quest)
        {
            if (quest.rewards == null) return;

            // EXP reward
            if (quest.rewards.expReward > 0)
            {
                if (LevelUpSystem.Instance != null)
                {
                    LevelUpSystem.Instance.AddEXP(quest.rewards.expReward);
                }
            }

            // Gold reward
            if (quest.rewards.goldReward > 0)
            {
                // TODO: Add gold to player inventory
                Debug.Log($"[Quest] +{quest.rewards.goldReward} Gold");
            }

            // Item rewards
            if (quest.rewards.itemIDs != null)
            {
                foreach (string itemID in quest.rewards.itemIDs)
                {
                    // TODO: Add item to inventory
                    Debug.Log($"[Quest] +Item: {itemID}");
                }
            }

            // Equipment reward
            if (!string.IsNullOrEmpty(quest.rewards.equipmentID))
            {
                // TODO: Add equipment to inventory
                Debug.Log($"[Quest] +Equipment: {quest.rewards.equipmentID}");
            }
        }

        /// <summary>
        /// Fail quest.
        /// </summary>
        public void FailQuest(string questID)
        {
            if (!activeQuests.ContainsKey(questID)) return;

            QuestData quest = activeQuests[questID];
            questStatuses[questID] = QuestStatus.Failed;
            activeQuests.Remove(questID);

            // Callback
            OnQuestFailed?.Invoke(quest);

            Debug.Log($"[Quest] Failed: {quest.questName}");
        }

        /// <summary>
        /// Abandon quest.
        /// </summary>
        public bool AbandonQuest(string questID)
        {
            if (!activeQuests.ContainsKey(questID))
            {
                return false;
            }

            QuestData quest = activeQuests[questID];
            questStatuses[questID] = QuestStatus.Available;
            activeQuests.Remove(questID);

            // Reset objectives
            foreach (var objective in quest.objectives)
            {
                objective.currentAmount = 0;
            }

            Debug.Log($"[Quest] Abandoned: {quest.questName}");
            return true;
        }

        /// <summary>
        /// Get quest status.
        /// </summary>
        public QuestStatus GetQuestStatus(string questID)
        {
            if (questStatuses.ContainsKey(questID))
            {
                return questStatuses[questID];
            }
            return QuestStatus.Locked;
        }

        /// <summary>
        /// Get quest data.
        /// </summary>
        public QuestData GetQuestData(string questID)
        {
            if (allQuests == null) return null;

            foreach (QuestData quest in allQuests)
            {
                if (quest != null && quest.questID == questID)
                {
                    return quest;
                }
            }

            return null;
        }

        /// <summary>
        /// Get all active quests.
        /// </summary>
        public List<QuestData> GetActiveQuests()
        {
            return activeQuests.Values.ToList();
        }

        /// <summary>
        /// Get all available quests.
        /// </summary>
        public List<QuestData> GetAvailableQuests()
        {
            List<QuestData> available = new List<QuestData>();

            Debug.Log($"[Quest] GetAvailableQuests called. Statuses count: {questStatuses.Count}");

            foreach (var kvp in questStatuses)
            {
                Debug.Log($"[Quest] Checking: {kvp.Key} = {kvp.Value}");

                if (kvp.Value == QuestStatus.Available)
                {
                    QuestData quest = GetQuestData(kvp.Key);
                    if (quest != null)
                    {
                        available.Add(quest);
                        Debug.Log($"[Quest] Added to available: {quest.questName}");
                    }
                    else
                    {
                        Debug.LogWarning($"[Quest] Quest data not found for: {kvp.Key}");
                    }
                }
            }

            Debug.Log($"[Quest] Available quests: {available.Count}");
            return available;
        }

        /// <summary>
        /// Get all completed quests (waiting for claim).
        /// </summary>
        public List<QuestData> GetCompletedQuests()
        {
            List<QuestData> completed = new List<QuestData>();

            foreach (var kvp in questStatuses)
            {
                if (kvp.Value == QuestStatus.Completed)
                {
                    QuestData quest = GetQuestData(kvp.Key);
                    if (quest != null)
                    {
                        completed.Add(quest);
                    }
                }
            }

            return completed;
        }

        /// <summary>
        /// Cek apakah quest tersedia.
        /// </summary>
        public bool IsQuestAvailable(string questID)
        {
            return GetQuestStatus(questID) == QuestStatus.Available;
        }

        /// <summary>
        /// Cek apakah quest aktif.
        /// </summary>
        public bool IsQuestActive(string questID)
        {
            return GetQuestStatus(questID) == QuestStatus.Active;
        }

        /// <summary>
        /// Cek apakah quest sudah selesai.
        /// </summary>
        public bool IsQuestCompleted(string questID)
        {
            return GetQuestStatus(questID) == QuestStatus.Completed;
        }

        /// <summary>
        /// Cek apakah quest sudah di-claim.
        /// </summary>
        public bool IsQuestClaimed(string questID)
        {
            return GetQuestStatus(questID) == QuestStatus.Claimed;
        }
    }
}
