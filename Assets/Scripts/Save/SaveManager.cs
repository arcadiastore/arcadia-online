using System.IO;
using UnityEngine;
using ArcadiaOnline.Core;

namespace ArcadiaOnline.Save
{
    /// <summary>Lihat docs/02_TDD/SaveArchitecture.md.</summary>
    public class SaveManager : Singleton<SaveManager>
    {
        private const string SAVE_FOLDER = "Saves";
        public const int MAX_SLOTS = 10;

        public void Save(int slot)
        {
            SaveData data = CollectSaveData();
            data.saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            data.gameVersion = Application.version;

            string json = JsonUtility.ToJson(data, true);
            string path = GetSavePath(slot);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);

            string backupPath = GetBackupPath(slot);
            File.Copy(path, backupPath, true);
        }

        public void Load(int slot)
        {
            string path = GetSavePath(slot);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"Save slot {slot} tidak ditemukan.");
                return;
            }

            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            ApplySaveData(data);
        }

        public void AutoSave()
        {
            SaveData data = CollectSaveData();
            data.saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            data.gameVersion = Application.version;

            string json = JsonUtility.ToJson(data, true);
            string path = Application.persistentDataPath + $"/{SAVE_FOLDER}/autosave.json";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);
        }

        public void DeleteSave(int slot)
        {
            string path = GetSavePath(slot);
            if (File.Exists(path)) File.Delete(path);
        }

        public bool HasSave(int slot) => File.Exists(GetSavePath(slot));

        public SaveData GetSaveData(int slot)
        {
            string path = GetSavePath(slot);
            if (!File.Exists(path)) return null;
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }

        private string GetSavePath(int slot) =>
            Application.persistentDataPath + $"/{SAVE_FOLDER}/save_{slot}.json";

        private string GetBackupPath(int slot) =>
            Application.persistentDataPath + $"/{SAVE_FOLDER}/save_{slot}_backup.json";

        /// <summary>
        /// Kumpulkan data dari semua sistem relevan (PlayerStats, WorldStateManager,
        /// QuestManager, PlayerInventory, dll) dan susun jadi SaveData.
        /// </summary>
        private SaveData CollectSaveData()
        {
            // TODO: hubungkan ke PlayerStats.Instance, WorldStateManager.Instance, dst.
            return new SaveData();
        }

        /// <summary>Terapkan SaveData yang sudah di-load ke seluruh sistem game.</summary>
        private void ApplySaveData(SaveData data)
        {
            // TODO: set posisi player, load scene, restore world state, dst.
        }
    }
}
