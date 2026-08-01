using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArcadiaOnline.Quest
{
    /// <summary>
    /// Auto-create sample quests.
    /// Attach ke GameObject lalu klik checkbox.
    /// </summary>
    public class QuestSampleCreator : MonoBehaviour
    {
        [Header("Create Samples")]
        [SerializeField] private bool createSamples;

        void OnValidate()
        {
            if (createSamples)
            {
                createSamples = false;
                CreateAllSamples();
            }
        }

        /// <summary>
        /// Create semua sample quests.
        /// </summary>
        private void CreateAllSamples()
        {
            // Create folder
            string folderPath = "Assets/Resources/Quests";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log($"[QuestSample] Created folder: {folderPath}");
            }

            // Create sample quests
            CreateKillSlimeQuest();
            CreateCollectHerbsQuest();
            CreateTalkToElderQuest();
            CreateBossQuest();

            Debug.Log("[QuestSample] All sample quests created!");
        }

        /// <summary>
        /// Kill Slime quest (beginner).
        /// </summary>
        private void CreateKillSlimeQuest()
        {
            QuestData quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questID = "quest_kill_slime";
            quest.questName = "Slime Hunter";
            quest.description = "Desa diserang oleh Slime! Kalahkan 5 Slime untuk melindungi desa.";
            quest.mainType = QuestType.Kill;
            quest.recommendedLevel = 1;

            // Objectives
            quest.objectives = new List<QuestObjective>();

            QuestObjective obj1 = new QuestObjective();
            obj1.description = "Kalahkan Slime";
            obj1.type = QuestType.Kill;
            obj1.targetID = "slime";
            obj1.requiredAmount = 5;
            obj1.currentAmount = 0;
            quest.objectives.Add(obj1);

            // Rewards
            quest.rewards = new QuestReward();
            quest.rewards.expReward = 50;
            quest.rewards.goldReward = 100;
            quest.rewards.itemIDs = new List<string> { "HP_Potion_Small" };

            // Dialogue
            quest.startDialogueID = "village_chief_intro";
            quest.completeDialogueID = "village_chief_complete";

            // Next quest
            quest.nextQuestID = "quest_collect_herbs";

            SaveQuest(quest, "Quest_KillSlime");
        }

        /// <summary>
        /// Collect Herbs quest.
        /// </summary>
        private void CreateCollectHerbsQuest()
        {
            QuestData quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questID = "quest_collect_herbs";
            quest.questName = "Herb Collector";
            quest.description = "Kumpulkan 3 Healing Herb dari huntuk untuk membuat obat.";
            quest.mainType = QuestType.Collect;
            quest.recommendedLevel = 3;
            quest.previousQuestID = "quest_kill_slime";

            // Objectives
            quest.objectives = new List<QuestObjective>();

            QuestObjective obj1 = new QuestObjective();
            obj1.description = "Kumpulkan Healing Herb";
            obj1.type = QuestType.Collect;
            obj1.targetID = "healing_herb";
            obj1.requiredAmount = 3;
            obj1.currentAmount = 0;
            quest.objectives.Add(obj1);

            // Rewards
            quest.rewards = new QuestReward();
            quest.rewards.expReward = 100;
            quest.rewards.goldReward = 200;
            quest.rewards.itemIDs = new List<string> { "HP_Potion_Medium", "MP_Potion_Small" };

            // Next quest
            quest.nextQuestID = "quest_talk_elder";

            SaveQuest(quest, "Quest_CollectHerbs");
        }

        /// <summary>
        /// Talk to Elder quest.
        /// </summary>
        private void CreateTalkToElderQuest()
        {
            QuestData quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questID = "quest_talk_elder";
            quest.questName = "Elder's Wisdom";
            quest.description = "Bicara dengan Village Elder untuk mendapatkan nasihat.";
            quest.mainType = QuestType.Talk;
            quest.recommendedLevel = 5;
            quest.previousQuestID = "quest_collect_herbs";

            // Objectives
            quest.objectives = new List<QuestObjective>();

            QuestObjective obj1 = new QuestObjective();
            obj1.description = "Bicara dengan Village Elder";
            obj1.type = QuestType.Talk;
            obj1.targetID = "village_elder";
            obj1.requiredAmount = 1;
            obj1.currentAmount = 0;
            quest.objectives.Add(obj1);

            // Rewards
            quest.rewards = new QuestReward();
            quest.rewards.expReward = 150;
            quest.rewards.goldReward = 300;

            // Next quest
            quest.nextQuestID = "quest_boss_wolf";

            SaveQuest(quest, "Quest_TalkElder");
        }

        /// <summary>
        /// Boss quest (Wolf Alpha).
        /// </summary>
        private void CreateBossQuest()
        {
            QuestData quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questID = "quest_boss_wolf";
            quest.questName = "Wolf Alpha";
            quest.description = "Kalahkan Wolf Alpha yang mengancam hutan!";
            quest.mainType = QuestType.Boss;
            quest.recommendedLevel = 10;
            quest.previousQuestID = "quest_talk_elder";

            // Objectives
            quest.objectives = new List<QuestObjective>();

            QuestObjective obj1 = new QuestObjective();
            obj1.description = "Kalahkan Wolf Alpha";
            obj1.type = QuestType.Boss;
            obj1.targetID = "wolf_alpha";
            obj1.requiredAmount = 1;
            obj1.currentAmount = 0;
            quest.objectives.Add(obj1);

            // Rewards
            quest.rewards = new QuestReward();
            quest.rewards.expReward = 500;
            quest.rewards.goldReward = 1000;
            quest.rewards.equipmentID = "Iron_Sword";

            SaveQuest(quest, "Quest_BossWolf");
        }

        /// <summary>
        /// Save quest as ScriptableObject asset.
        /// </summary>
        private void SaveQuest(QuestData quest, string fileName)
        {
#if UNITY_EDITOR
            string path = $"Assets/Resources/Quests/{fileName}.asset";
            AssetDatabase.CreateAsset(quest, path);
            Debug.Log($"[QuestSample] Created: {path}");
#else
            Debug.Log($"[QuestSample] {fileName} created (runtime mode)");
#endif
        }
    }
}
