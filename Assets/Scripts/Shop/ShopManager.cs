using UnityEngine;
using System.Collections.Generic;
using ArcadiaOnline.Player;
using ArcadiaOnline.Inventory;

namespace ArcadiaOnline.Shop
{
    /// <summary>
    /// Manager untuk shop system.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }

        [Header("Shop Data")]
        [SerializeField] private List<ShopData> allShops;

        [Header("Player Gold")]
        [SerializeField] private int playerGold = 1000;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        // Events
        public System.Action<ShopData> OnShopOpened;
        public System.Action<ShopData> OnShopClosed;
        public System.Action<ShopItem, int> OnItemBought;
        public System.Action<ShopItem, int> OnItemSold;
        public System.Action<int> OnGoldChanged;

        // State
        private ShopData currentShop;
        private bool isShopOpen = false;

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

        /// <summary>
        /// Open shop.
        /// </summary>
        public bool OpenShop(string shopID)
        {
            if (isShopOpen)
            {
                Debug.LogWarning("[Shop] Shop already open!");
                return false;
            }

            ShopData shop = GetShopData(shopID);
            if (shop == null)
            {
                Debug.LogWarning($"[Shop] Shop not found: {shopID}");
                return false;
            }

            // Cek level requirement
            if (LevelUpSystem.Instance != null)
            {
                // TODO: Check level requirement per item
            }

            currentShop = shop;
            isShopOpen = true;

            // Disable player movement
            DisablePlayerMovement();

            // Callback
            OnShopOpened?.Invoke(shop);

            Debug.Log($"[Shop] Opened: {shop.shopName}");
            return true;
        }

        /// <summary>
        /// Close shop.
        /// </summary>
        public void CloseShop()
        {
            if (!isShopOpen) return;

            ShopData shop = currentShop;
            currentShop = null;
            isShopOpen = false;

            // Enable player movement
            EnablePlayerMovement();

            // Callback
            OnShopClosed?.Invoke(shop);

            Debug.Log("[Shop] Closed");
        }

        /// <summary>
        /// Buy item from shop.
        /// </summary>
        public bool BuyItem(string itemID, int amount = 1)
        {
            if (!isShopOpen || currentShop == null)
            {
                Debug.LogWarning("[Shop] Shop not open!");
                return false;
            }

            // Find item in shop
            ShopItem shopItem = FindShopItem(itemID);
            if (shopItem == null)
            {
                Debug.LogWarning($"[Shop] Item not found: {itemID}");
                return false;
            }

            // Cek availability
            if (!shopItem.isAvailable)
            {
                Debug.LogWarning($"[Shop] Item not available: {itemID}");
                return false;
            }

            // Cek stock
            if (shopItem.stock != -1 && shopItem.stock < amount)
            {
                Debug.LogWarning($"[Shop] Not enough stock: {itemID} (stock: {shopItem.stock})");
                return false;
            }

            // Cek level requirement
            if (LevelUpSystem.Instance != null)
            {
                if (LevelUpSystem.Instance.CurrentLevel < shopItem.requiredLevel)
                {
                    Debug.LogWarning($"[Shop] Level too low! Need Lv.{shopItem.requiredLevel}");
                    return false;
                }
            }

            // Cek gold
            int totalPrice = currentShop.GetBuyPrice(shopItem) * amount;
            if (playerGold < totalPrice)
            {
                Debug.LogWarning($"[Shop] Not enough gold! Need {totalPrice}, have {playerGold}");
                return false;
            }

            // Kurangi gold
            playerGold -= totalPrice;
            OnGoldChanged?.Invoke(playerGold);

            // Kurangi stock
            if (shopItem.stock != -1)
            {
                shopItem.stock -= amount;
            }

            // Tambah item ke inventory
            AddItemToInventory(itemID, amount);

            // Callback
            OnItemBought?.Invoke(shopItem, amount);

            Debug.Log($"[Shop] Bought {amount}x {shopItem.itemName} for {totalPrice}G");
            return true;
        }

        /// <summary>
        /// Sell item to shop.
        /// </summary>
        public bool SellItem(string itemID, int amount = 1)
        {
            if (!isShopOpen || currentShop == null)
            {
                Debug.LogWarning("[Shop] Shop not open!");
                return false;
            }

            // Cek apakah shop bisa jual
            if (!currentShop.canSell)
            {
                Debug.LogWarning("[Shop] This shop doesn't buy items!");
                return false;
            }

            // Cek apakah player punya item
            if (!HasItemInInventory(itemID, amount))
            {
                Debug.LogWarning($"[Shop] Not enough items: {itemID}");
                return false;
            }

            // Hitung harga jual
            ShopItem shopItem = FindShopItem(itemID);
            int sellPrice = 0;

            if (shopItem != null)
            {
                sellPrice = currentShop.GetSellPrice(shopItem) * amount;
            }
            else
            {
                // Default sell price jika item tidak ada di shop
                sellPrice = 10 * amount; // TODO: Get from item data
            }

            // Hapus item dari inventory
            RemoveItemFromInventory(itemID, amount);

            // Tambah gold
            playerGold += sellPrice;
            OnGoldChanged?.Invoke(playerGold);

            // Callback
            if (shopItem != null)
            {
                OnItemSold?.Invoke(shopItem, amount);
            }

            Debug.Log($"[Shop] Sold {amount}x {itemID} for {sellPrice}G");
            return true;
        }

        /// <summary>
        /// Find shop item by ID.
        /// </summary>
        private ShopItem FindShopItem(string itemID)
        {
            if (currentShop == null || currentShop.items == null) return null;

            foreach (ShopItem item in currentShop.items)
            {
                if (item.itemID == itemID)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Add item to player inventory.
        /// </summary>
        private void AddItemToInventory(string itemID, int amount)
        {
            // TODO: Implement with InventoryManager
            // For now, just log
            Debug.Log($"[Shop] Added {amount}x {itemID} to inventory");
        }

        /// <summary>
        /// Remove item from player inventory.
        /// </summary>
        private void RemoveItemFromInventory(string itemID, int amount)
        {
            // TODO: Implement with InventoryManager
            // For now, just log
            Debug.Log($"[Shop] Removed {amount}x {itemID} from inventory");
        }

        /// <summary>
        /// Check if player has item.
        /// </summary>
        private bool HasItemInInventory(string itemID, int amount)
        {
            // TODO: Implement with InventoryManager
            // For now, return true
            return true;
        }

        /// <summary>
        /// Disable player movement.
        /// </summary>
        private void DisablePlayerMovement()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    if (script.GetType().Name.Contains("PlayerController"))
                    {
                        script.enabled = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Enable player movement.
        /// </summary>
        private void EnablePlayerMovement()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    if (script.GetType().Name.Contains("PlayerController"))
                    {
                        script.enabled = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Get shop data by ID.
        /// </summary>
        public ShopData GetShopData(string shopID)
        {
            if (allShops == null) return null;

            foreach (ShopData shop in allShops)
            {
                if (shop != null && shop.shopID == shopID)
                {
                    return shop;
                }
            }

            return null;
        }

        /// <summary>
        /// Get current shop.
        /// </summary>
        public ShopData GetCurrentShop()
        {
            return currentShop;
        }

        /// <summary>
        /// Cek apakah shop sedang buka.
        /// </summary>
        public bool IsShopOpen()
        {
            return isShopOpen;
        }

        /// <summary>
        /// Get player gold.
        /// </summary>
        public int GetPlayerGold()
        {
            return playerGold;
        }

        /// <summary>
        /// Set player gold.
        /// </summary>
        public void SetPlayerGold(int gold)
        {
            playerGold = gold;
            OnGoldChanged?.Invoke(playerGold);
        }

        /// <summary>
        /// Add gold to player.
        /// </summary>
        public void AddGold(int amount)
        {
            playerGold += amount;
            OnGoldChanged?.Invoke(playerGold);
        }

        /// <summary>
        /// Remove gold from player.
        /// </summary>
        public bool RemoveGold(int amount)
        {
            if (playerGold < amount)
            {
                return false;
            }

            playerGold -= amount;
            OnGoldChanged?.Invoke(playerGold);
            return true;
        }
    }
}
