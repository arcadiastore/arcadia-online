using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using ArcadiaOnline.Player;
using ArcadiaOnline.Equipment;
using ArcadiaOnline.Inventory;
using ArcadiaOnline.Shop;

namespace ArcadiaOnline.Save
{
    /// <summary>
    /// Manager untuk save/load system.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [Header("Save Settings")]
        [SerializeField] private string saveFileName = "save_data.json";
        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 300f; // 5 menit

        [Header("Save Slots")]
        [SerializeField] private int maxSaveSlots = 3;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        // Events
        public System.Action<SaveData> OnGameSaved;
        public System.Action<SaveData> OnGameLoaded;
        public System.Action<string> OnSaveError;

        // State
        private SaveData currentSaveData;
        private float autoSaveTimer;
        private string saveDirectory;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Setup save directory
            saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);

                if (showDebug)
                {
                    Debug.Log($"[SaveManager] Created save directory: {saveDirectory}");
                }
            }
        }

        void Start()
        {
            // Initialize save data
            currentSaveData = new SaveData();

            if (showDebug)
            {
                Debug.Log("[SaveManager] Initialized");
                Debug.Log($"[SaveManager] Save path: {saveDirectory}");
            }
        }

        void Update()
        {
            // Auto-save timer
            if (autoSave)
            {
                autoSaveTimer += Time.deltaTime;

                if (autoSaveTimer >= autoSaveInterval)
                {
                    autoSaveTimer = 0f;
                    AutoSave();
                }
            }

            // Quick save/load shortcuts (untuk testing)
            if (Input.GetKeyDown(KeyCode.F5))
            {
                QuickSave();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                QuickLoad();
            }
        }

        /// <summary>
        /// Save game ke slot.
        /// </summary>
        public bool SaveGame(int slot = 0)
        {
            try
            {
                // Collect data dari game systems
                SaveData saveData = CollectSaveData();

                // Set metadata
                saveData.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                saveData.saveVersion = "1.0";

                // Get file path
                string filePath = GetSaveFilePath(slot);

                // Serialize to JSON
                string json = JsonUtility.ToJson(saveData, true);

                // Write to file
                File.WriteAllText(filePath, json);

                // Update current save
                currentSaveData = saveData;

                // Callback
                OnGameSaved?.Invoke(saveData);

                if (showDebug)
                {
                    Debug.Log($"[SaveManager] Game saved to slot {slot}");
                    Debug.Log($"[SaveManager] File: {filePath}");
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Save failed: {e.Message}");
                OnSaveError?.Invoke(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Load game dari slot.
        /// </summary>
        public bool LoadGame(int slot = 0)
        {
            try
            {
                // Get file path
                string filePath = GetSaveFilePath(slot);

                // Check if file exists
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[SaveManager] Save file not found: {filePath}");
                    return false;
                }

                // Read JSON
                string json = File.ReadAllText(filePath);

                // Deserialize
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData == null)
                {
                    Debug.LogError("[SaveManager] Failed to deserialize save data");
                    return false;
                }

                // Apply data ke game systems
                ApplySaveData(saveData);

                // Update current save
                currentSaveData = saveData;

                // Callback
                OnGameLoaded?.Invoke(saveData);

                if (showDebug)
                {
                    Debug.Log($"[SaveManager] Game loaded from slot {slot}");
                    Debug.Log($"[SaveManager] Player: {saveData.playerName} Lv.{saveData.playerLevel}");
                    Debug.Log($"[SaveManager] Play time: {FormatPlayTime(saveData.playTimeSeconds)}");
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Load failed: {e.Message}");
                OnSaveError?.Invoke(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Quick save (F5).
        /// </summary>
        public void QuickSave()
        {
            if (SaveGame(0))
            {
                Debug.Log("[SaveManager] Quick save successful!");
            }
        }

        /// <summary>
        /// Quick load (F9).
        /// </summary>
        public void QuickLoad()
        {
            if (LoadGame(0))
            {
                Debug.Log("[SaveManager] Quick load successful!");
            }
        }

        /// <summary>
        /// Auto-save.
        /// </summary>
        private void AutoSave()
        {
            if (showDebug)
            {
                Debug.Log("[SaveManager] Auto-saving...");
            }

            SaveGame(0);
        }

        /// <summary>
        /// Collect data dari semua game systems.
        /// </summary>
        private SaveData CollectSaveData()
        {
            SaveData data = new SaveData();

            // Player data
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Position
                data.posX = player.transform.position.x;
                data.posY = player.transform.position.y;
                data.posZ = player.transform.position.z;

                // Player stats
                PlayerStats stats = player.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    data.currentHP = stats.CurrentHP;
                    data.maxHP = stats.MaxHP;
                    data.currentMP = stats.CurrentMP;
                    data.maxMP = stats.MaxMP;
                    data.str = stats.Str;
                    data.agi = stats.Agi;
                    data.vit = stats.Vit;
                    data.intel = stats.Int;
                    data.luk = stats.Luk;
                }

                // Level data
                LevelUpSystem levelSystem = player.GetComponent<LevelUpSystem>();
                if (levelSystem != null)
                {
                    data.playerLevel = levelSystem.CurrentLevel;
                    data.playerEXP = levelSystem.CurrentEXP;
                }
            }

            // Equipment data
            if (EquipmentManager.Instance != null)
            {
                data.equippedItems = CollectEquipmentData();
            }

            // Inventory data
            if (InventoryManager.Instance != null)
            {
                data.inventoryItems = CollectInventoryData();
            }

            // Skill data
            if (SkillSystem.Instance != null)
            {
                data.learnedSkills = CollectSkillData();
            }

            // Quest data
            if (QuestManager.Instance != null)
            {
                data.activeQuests = CollectQuestData();
                data.completedQuests = CollectCompletedQuests();
            }

            // Gold
            if (ShopManager.Instance != null)
            {
                data.gold = ShopManager.Instance.GetPlayerGold();
            }

            return data;
        }

        /// <summary>
        /// Collect equipment data.
        /// </summary>
        private List<EquipmentSaveData> CollectEquipmentData()
        {
            List<EquipmentSaveData> equipmentList = new List<EquipmentSaveData>();

            // TODO: Implement with EquipmentManager
            // For now, return empty list

            return equipmentList;
        }

        /// <summary>
        /// Collect inventory data.
        /// </summary>
        private List<InventorySaveData> CollectInventoryData()
        {
            List<InventorySaveData> inventoryList = new List<InventorySaveData>();

            // TODO: Implement with InventoryManager
            // For now, return empty list

            return inventoryList;
        }

        /// <summary>
        /// Collect skill data.
        /// </summary>
        private List<SkillSaveData> CollectSkillData()
        {
            List<SkillSaveData> skillList = new List<SkillSaveData>();

            // TODO: Implement with SkillSystem
            // For now, return empty list

            return skillList;
        }

        /// <summary>
        /// Collect active quest data.
        /// </summary>
        private List<QuestSaveData> CollectQuestData()
        {
            List<QuestSaveData> questList = new List<QuestSaveData>();

            // TODO: Implement with QuestManager
            // For now, return empty list

            return questList;
        }

        /// <summary>
        /// Collect completed quest IDs.
        /// </summary>
        private List<string> CollectCompletedQuests()
        {
            List<string> completedList = new List<string>();

            // TODO: Implement with QuestManager
            // For now, return empty list

            return completedList;
        }

        /// <summary>
        /// Apply save data ke game systems.
        /// </summary>
        private void ApplySaveData(SaveData data)
        {
            // Player data
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Position
                player.transform.position = new Vector3(data.posX, data.posY, data.posZ);

                // Player stats
                PlayerStats stats = player.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.SetHP(data.currentHP);
                    stats.SetMP(data.currentMP);
                    stats.SetStats(data.str, data.agi, data.vit, data.intel, data.luk);
                }

                // Level data
                LevelUpSystem levelSystem = player.GetComponent<LevelUpSystem>();
                if (levelSystem != null)
                {
                    levelSystem.SetLevel(data.playerLevel);
                    levelSystem.SetEXP(data.playerEXP);
                }
            }

            // Equipment data
            if (EquipmentManager.Instance != null && data.equippedItems != null)
            {
                ApplyEquipmentData(data.equippedItems);
            }

            // Inventory data
            if (InventoryManager.Instance != null && data.inventoryItems != null)
            {
                ApplyInventoryData(data.inventoryItems);
            }

            // Skill data
            if (SkillSystem.Instance != null && data.learnedSkills != null)
            {
                ApplySkillData(data.learnedSkills);
            }

            // Quest data
            if (QuestManager.Instance != null)
            {
                if (data.activeQuests != null)
                {
                    ApplyQuestData(data.activeQuests);
                }

                if (data.completedQuests != null)
                {
                    ApplyCompletedQuests(data.completedQuests);
                }
            }

            // Gold
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.SetPlayerGold(data.gold);
            }
        }

        /// <summary>
        /// Apply equipment data.
        /// </summary>
        private void ApplyEquipmentData(List<EquipmentSaveData> equipmentList)
        {
            // TODO: Implement with EquipmentManager
        }

        /// <summary>
        /// Apply inventory data.
        /// </summary>
        private void ApplyInventoryData(List<InventorySaveData> inventoryList)
        {
            // TODO: Implement with InventoryManager
        }

        /// <summary>
        /// Apply skill data.
        /// </summary>
        private void ApplySkillData(List<SkillSaveData> skillList)
        {
            // TODO: Implement with SkillSystem
        }

        /// <summary>
        /// Apply quest data.
        /// </summary>
        private void ApplyQuestData(List<QuestSaveData> questList)
        {
            // TODO: Implement with QuestManager
        }

        /// <summary>
        /// Apply completed quests.
        /// </summary>
        private void ApplyCompletedQuests(List<string> completedQuests)
        {
            // TODO: Implement with QuestManager
        }

        /// <summary>
        /// Get save file path.
        /// </summary>
        private string GetSaveFilePath(int slot)
        {
            string fileName = $"save_{slot}.json";
            return Path.Combine(saveDirectory, fileName);
        }

        /// <summary>
        /// Check if save file exists.
        /// </summary>
        public bool SaveFileExists(int slot = 0)
        {
            return File.Exists(GetSaveFilePath(slot));
        }

        /// <summary>
        /// Delete save file.
        /// </summary>
        public bool DeleteSave(int slot = 0)
        {
            try
            {
                string filePath = GetSaveFilePath(slot);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

                    if (showDebug)
                    {
                        Debug.Log($"[SaveManager] Deleted save slot {slot}");
                    }

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Delete failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all save slots info.
        /// </summary>
        public List<SaveSlotInfo> GetAllSaveSlots()
        {
            List<SaveSlotInfo> slots = new List<SaveSlotInfo>();

            for (int i = 0; i < maxSaveSlots; i++)
            {
                SaveSlotInfo info = new SaveSlotInfo();
                info.slotIndex = i;
                info.exists = SaveFileExists(i);

                if (info.exists)
                {
                    try
                    {
                        string filePath = GetSaveFilePath(i);
                        string json = File.ReadAllText(filePath);
                        SaveData data = JsonUtility.FromJson<SaveData>(json);

                        info.playerName = data.playerName;
                        info.playerLevel = data.playerLevel;
                        info.currentMap = data.currentMap;
                        info.saveDate = data.saveDate;
                        info.playTime = FormatPlayTime(data.playTimeSeconds);
                    }
                    catch
                    {
                        info.exists = false;
                    }
                }

                slots.Add(info);
            }

            return slots;
        }

        /// <summary>
        /// Get current save data.
        /// </summary>
        public SaveData GetCurrentSaveData()
        {
            return currentSaveData;
        }

        /// <summary>
        /// Format play time.
        /// </summary>
        private string FormatPlayTime(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }
    }

    /// <summary>
    /// Info untuk save slot.
    /// </summary>
    [System.Serializable]
    public class SaveSlotInfo
    {
        public int slotIndex;
        public bool exists;
        public string playerName;
        public int playerLevel;
        public string currentMap;
        public string saveDate;
        public string playTime;
    }
}
