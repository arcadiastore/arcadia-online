using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArcadiaOnline.Inventory
{
    /// <summary>
    /// Helper untuk membuat sample consumable items.
    /// </summary>
    public class ConsumableSampleCreator : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Click to create sample consumables")]
        [SerializeField] private bool createSamples = false;

        void OnValidate()
        {
            if (createSamples)
            {
                createSamples = false;
                CreateAllSampleConsumables();
            }
        }

        [ContextMenu("Create Sample Consumables")]
        public void CreateAllSampleConsumables()
        {
            // Create folder
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Items"))
                AssetDatabase.CreateFolder("Assets/Resources", "Items");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Items/Consumables"))
                AssetDatabase.CreateFolder("Assets/Resources/Items", "Consumables");

            // === HP POTIONS ===
            CreateConsumable("HP_Potion_Small", "Small HP Potion", "Memulihkan 50 HP",
                ConsumableType.HPPotion, 50f, 0f, 1f, 10, 5);

            CreateConsumable("HP_Potion_Medium", "Medium HP Potion", "Memulihkan 150 HP",
                ConsumableType.HPPotion, 150f, 0f, 1f, 50, 25);

            CreateConsumable("HP_Potion_Large", "Large HP Potion", "Memulihkan 500 HP",
                ConsumableType.HPPotion, 500f, 0f, 1f, 200, 100);

            // === MP POTIONS ===
            CreateConsumable("MP_Potion_Small", "Small MP Potion", "Memulihkan 30 MP",
                ConsumableType.MPPotion, 30f, 0f, 1f, 10, 5);

            CreateConsumable("MP_Potion_Medium", "Medium MP Potion", "Memulihkan 100 MP",
                ConsumableType.MPPotion, 100f, 0f, 1f, 50, 25);

            CreateConsumable("MP_Potion_Large", "Large MP Potion", "Memulihkan 300 MP",
                ConsumableType.MPPotion, 300f, 0f, 1f, 200, 100);

            // === STAMINA POTIONS ===
            CreateConsumable("Stamina_Potion", "Stamina Potion", "Memulihkan 100 Stamina",
                ConsumableType.StaminaPotion, 100f, 0f, 1f, 30, 15);

            // === BUFF POTIONS ===
            CreateConsumable("ATK_Potion", "ATK Potion", "ATK +20 selama 60 detik",
                ConsumableType.BuffPotion, 0f, 60f, 1f, 100, 50,
                atkBuff: 20f);

            CreateConsumable("DEF_Potion", "DEF Potion", "DEF +20 selama 60 detik",
                ConsumableType.BuffPotion, 0f, 60f, 1f, 100, 50,
                defBuff: 20f);

            CreateConsumable("SPD_Potion", "SPD Potion", "SPD +10 selama 60 detik",
                ConsumableType.BuffPotion, 0f, 60f, 1f, 100, 50,
                spdBuff: 10f);

            CreateConsumable("Crit_Potion", "Critical Potion", "Crit Rate +15% selama 60 detik",
                ConsumableType.BuffPotion, 0f, 60f, 1f, 150, 75,
                critRateBuff: 15f);

            // === FOOD ===
            CreateConsumable("Bread", "Roti", "Regen 20 HP selama 10 detik",
                ConsumableType.Food, 20f, 10f, 1f, 5, 2);

            CreateConsumable("Meat", "Daging", "Regen 50 HP selama 15 detik",
                ConsumableType.Food, 50f, 15f, 1f, 20, 10);

            CreateConsumable("Cooked_Fish", "Ikan Panggang", "Regen 80 HP selama 20 detik",
                ConsumableType.Food, 80f, 20f, 1f, 40, 20);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Inventory] Sample consumables created!");
        }

        private void CreateConsumable(string id, string name, string desc,
            ConsumableType type, float effectValue, float duration, float cooldown,
            int buyPrice, int sellPrice,
            float atkBuff = 0, float defBuff = 0, float spdBuff = 0,
            float critRateBuff = 0, float critDmgBuff = 0)
        {
            string path = $"Assets/Resources/Items/Consumables/{id}.asset";

            // Check if already exists
            if (AssetDatabase.LoadAssetAtPath<ConsumableData>(path) != null)
                return;

            ConsumableData item = ScriptableObject.CreateInstance<ConsumableData>();
            item.id = id;
            item.itemName = name;
            item.description = desc;
            item.type = ItemType.Consumable;
            item.consumableType = type;
            item.effectValue = effectValue;
            item.effectDuration = duration;
            item.cooldown = cooldown;
            item.buyPrice = buyPrice;
            item.sellPrice = sellPrice;
            item.isStackable = true;
            item.maxStackSize = 99;
            item.isUsable = true;
            item.isEquippable = false;
            item.isDroppable = true;
            item.atkBuff = atkBuff;
            item.defBuff = defBuff;
            item.spdBuff = spdBuff;
            item.critRateBuff = critRateBuff;
            item.critDmgBuff = critDmgBuff;

            AssetDatabase.CreateAsset(item, path);
        }
#endif
    }
}
