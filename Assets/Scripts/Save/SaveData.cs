using UnityEngine;
using System;
using System.Collections.Generic;

namespace ArcadiaOnline.Save
{
    /// <summary>
    /// Data yang di-save.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        // Player Info
        public string playerName = "Hero";
        public int playerLevel = 1;
        public int playerEXP = 0;

        // Player Stats
        public int currentHP = 100;
        public int maxHP = 100;
        public int currentMP = 50;
        public int maxMP = 50;
        public int currentStamina = 100;
        public int maxStamina = 100;

        // Player Attributes
        public int str = 10;
        public int agi = 10;
        public int vit = 10;
        public int intel = 10;
        public int luk = 10;

        // Position
        public float posX = 0f;
        public float posY = 0f;
        public float posZ = 0f;

        // Gold
        public int gold = 0;

        // Equipment
        public List<EquipmentSaveData> equippedItems = new List<EquipmentSaveData>();

        // Inventory
        public List<InventorySaveData> inventoryItems = new List<InventorySaveData>();

        // Skills
        public List<SkillSaveData> learnedSkills = new List<SkillSaveData>();

        // Quests
        public List<QuestSaveData> activeQuests = new List<QuestSaveData>();
        public List<string> completedQuests = new List<string>();

        // Game State
        public string currentMap = "Beginner Village";
        public int playTimeSeconds = 0;

        // Meta
        public string saveDate;
        public string saveVersion = "1.0";
    }

    /// <summary>
    /// Equipment data untuk save.
    /// </summary>
    [System.Serializable]
    public class EquipmentSaveData
    {
        public string slotName;
        public string itemID;
        public string itemName;
        public int enhancementLevel;
    }

    /// <summary>
    /// Inventory data untuk save.
    /// </summary>
    [System.Serializable]
    public class InventorySaveData
    {
        public string itemID;
        public string itemName;
        public int quantity;
        public int slotIndex;
    }

    /// <summary>
    /// Skill data untuk save.
    /// </summary>
    [System.Serializable]
    public class SkillSaveData
    {
        public string skillID;
        public string skillName;
        public int level;
        public float cooldownRemaining;
    }

    /// <summary>
    /// Quest data untuk save.
    /// </summary>
    [System.Serializable]
    public class QuestSaveData
    {
        public string questID;
        public string questName;
        public List<ObjectiveSaveData> objectives = new List<ObjectiveSaveData>();
    }

    /// <summary>
    /// Quest objective data untuk save.
    /// </summary>
    [System.Serializable]
    public class ObjectiveSaveData
    {
        public string description;
        public int currentAmount;
        public int requiredAmount;
        public bool isCompleted;
    }
}
