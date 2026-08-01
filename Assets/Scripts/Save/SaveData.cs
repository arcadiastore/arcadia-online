using System;
using System.Collections.Generic;

namespace ArcadiaOnline.Save
{
    /// <summary>Lihat docs/02_TDD/SaveArchitecture.md.</summary>
    [Serializable]
    public class SaveData
    {
        public string saveDate;
        public float playTime;
        public string gameVersion;
        public string checksum;

        public PlayerSaveData player;
        public List<CompanionSaveData> companions = new List<CompanionSaveData>();
        public WorldSaveData world;
        public QuestSaveData quests;
        public InventorySaveData inventory;
    }

    [Serializable]
    public class PlayerSaveData
    {
        public string playerName;
        public int level;
        public string jobId;
        public float currentHP;
        public float currentMP;
        public float exp;
        public float[] position = new float[3];
        public string currentScene;
        public List<string> unlockedSkills = new List<string>();
    }

    [Serializable]
    public class CompanionSaveData
    {
        public string companionId;
        public int level;
        public bool isRecruited;
    }

    [Serializable]
    public class WorldSaveData
    {
        public List<WorldStateEntry> worldStates = new List<WorldStateEntry>();
        public List<ReputationEntry> reputation = new List<ReputationEntry>();
        public int dayCount;
        public string weather;
        public string timeOfDay;
    }

    // Unity JsonUtility tidak mendukung Dictionary secara native,
    // jadi WorldState/Reputation disimpan sebagai list of entry.
    [Serializable]
    public class WorldStateEntry
    {
        public string key;
        public bool value;
    }

    [Serializable]
    public class ReputationEntry
    {
        public string factionId;
        public int value;
    }

    [Serializable]
    public class QuestSaveData
    {
        public List<string> activeQuestIds = new List<string>();
        public List<string> completedQuestIds = new List<string>();
        public List<string> failedQuestIds = new List<string>();
    }

    [Serializable]
    public class InventorySaveData
    {
        public List<ItemSaveEntry> items = new List<ItemSaveEntry>();
        public int gold;
    }

    [Serializable]
    public class ItemSaveEntry
    {
        public string itemId;
        public int quantity;
    }
}
