using UnityEngine;
using System.Collections.Generic;

namespace ArcadiaOnline.Shop
{
    /// <summary>
    /// Tipe shop.
    /// </summary>
    public enum ShopType
    {
        General,    // General store (items, potions)
        Weapon,     // Weapon shop
        Armor,      // Armor shop
        Accessory,  // Accessory shop
        Blacksmith, // Craft/upgrade
        Special     // Special items
    }

    /// <summary>
    /// Data item di shop.
    /// </summary>
    [System.Serializable]
    public class ShopItem
    {
        public string itemID;           // ID item
        public string itemName;         // Nama item
        public int buyPrice;            // Harga beli
        public int sellPrice;           // Harga jual
        public int stock;               // Stok (-1 = unlimited)
        public bool isAvailable;        // Apakah tersedia

        [Header("Requirements")]
        public int requiredLevel = 1;   // Level minimal
        public string requiredQuest;    // Quest yang harus selesai
    }

    /// <summary>
    /// Data shop (ScriptableObject).
    /// </summary>
    [CreateAssetMenu(fileName = "New Shop", menuName = "Arcadia/Shop")]
    public class ShopData : ScriptableObject
    {
        [Header("Shop Info")]
        public string shopID;               // ID unik shop
        public string shopName;             // Nama shop
        [TextArea(2, 4)]
        public string description;          // Deskripsi shop
        public ShopType shopType;           // Tipe shop

        [Header("NPC")]
        public string npcName;              // Nama NPC penjual
        public Sprite npcPortrait;          // Portrait NPC

        [Header("Items")]
        public List<ShopItem> items;        // Daftar item

        [Header("Settings")]
        public float buyMultiplier = 1.0f;  // Multiplier harga beli
        public float sellMultiplier = 0.5f; // Multiplier harga jual
        public bool canBuy = true;          // Bisa beli
        public bool canSell = true;         // Bisa jual

        [Header("Dialogue")]
        public string greetingDialogueID;   // Dialogue saat buka shop
        public string buyDialogueID;        // Dialogue saat beli
        public string sellDialogueID;       // Dialogue saat jual

        void OnValidate()
        {
            if (items == null)
                items = new List<ShopItem>();
        }

        /// <summary>
        /// Get item count.
        /// </summary>
        public int GetItemCount()
        {
            return items != null ? items.Count : 0;
        }

        /// <summary>
        /// Get available items.
        /// </summary>
        public List<ShopItem> GetAvailableItems()
        {
            List<ShopItem> available = new List<ShopItem>();

            if (items == null) return available;

            foreach (ShopItem item in items)
            {
                if (item.isAvailable && (item.stock == -1 || item.stock > 0))
                {
                    available.Add(item);
                }
            }

            return available;
        }

        /// <summary>
        /// Get buy price with multiplier.
        /// </summary>
        public int GetBuyPrice(ShopItem item)
        {
            return Mathf.RoundToInt(item.buyPrice * buyMultiplier);
        }

        /// <summary>
        /// Get sell price with multiplier.
        /// </summary>
        public int GetSellPrice(ShopItem item)
        {
            return Mathf.RoundToInt(item.sellPrice * sellMultiplier);
        }
    }
}
