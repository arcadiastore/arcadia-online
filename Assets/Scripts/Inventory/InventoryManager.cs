using UnityEngine;
using System.Collections.Generic;
using ArcadiaOnline.Equipment;
using ArcadiaOnline.Player;

namespace ArcadiaOnline.Inventory
{
    /// <summary>
    /// Manager untuk inventory system.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int maxSlots = 30;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent OnInventoryChanged;

        // Inventory slots
        private List<InventoryItem> items = new List<InventoryItem>();

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
            // Initialize empty slots
            for (int i = 0; i < maxSlots; i++)
            {
                items.Add(null);
            }
        }

        // === ADD ITEM ===

        /// <summary>
        /// Tambah item ke inventory.
        /// </summary>
        public bool AddItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null || quantity <= 0) return false;

            // Coba stack ke existing item
            if (itemData.isStackable)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] != null &&
                        items[i].ItemData == itemData &&
                        items[i].CanStack())
                    {
                        int canAdd = items[i].GetRemainingStack();
                        int toAdd = Mathf.Min(quantity, canAdd);

                        items[i].AddQuantity(toAdd);
                        quantity -= toAdd;

                        if (quantity <= 0)
                        {
                            OnInventoryChanged?.Invoke();
                            Debug.Log($"[Inventory] Added {itemData.itemName} x{toAdd}");
                            return true;
                        }
                    }
                }
            }

            // Cari slot kosong
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null || items[i].IsEmpty())
                {
                    int toAdd = Mathf.Min(quantity, itemData.maxStackSize);
                    items[i] = new InventoryItem(itemData, toAdd, i);
                    quantity -= toAdd;

                    if (quantity <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        Debug.Log($"[Inventory] Added {itemData.itemName} x{toAdd}");
                        return true;
                    }
                }
            }

            // Inventory penuh
            if (quantity > 0)
            {
                Debug.LogWarning($"[Inventory] Full! Cannot add {itemData.itemName} x{quantity}");
                return false;
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        // === REMOVE ITEM ===

        /// <summary>
        /// Hapus item dari inventory.
        /// </summary>
        public bool RemoveItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null || quantity <= 0) return false;

            int remaining = quantity;

            // Cari dan hapus dari slot
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] != null && items[i].ItemData == itemData)
                {
                    int canRemove = Mathf.Min(remaining, items[i].Quantity);
                    items[i].RemoveQuantity(canRemove);
                    remaining -= canRemove;

                    if (items[i].IsEmpty())
                    {
                        items[i] = null;
                    }

                    if (remaining <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        Debug.Log($"[Inventory] Removed {itemData.itemName} x{quantity}");
                        return true;
                    }
                }
            }

            // Tidak cukup item
            if (remaining > 0)
            {
                Debug.LogWarning($"[Inventory] Not enough {itemData.itemName}!");
                return false;
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Hapus item di slot tertentu.
        /// </summary>
        public bool RemoveItemAt(int slotIndex, int quantity = 1)
        {
            if (slotIndex < 0 || slotIndex >= items.Count) return false;
            if (items[slotIndex] == null || items[slotIndex].IsEmpty()) return false;

            items[slotIndex].RemoveQuantity(quantity);

            if (items[slotIndex].IsEmpty())
            {
                items[slotIndex] = null;
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        // === USE ITEM ===

        /// <summary>
        /// Gunakan item (consumable).
        /// </summary>
        public bool UseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= items.Count) return false;
            if (items[slotIndex] == null || items[slotIndex].IsEmpty()) return false;

            ItemData item = items[slotIndex].ItemData;

            // Cek tipe item
            if (item is ConsumableData consumable)
            {
                return UseConsumable(consumable, slotIndex);
            }
            else if (item is EquipmentData equipment)
            {
                return EquipItem(equipment, slotIndex);
            }

            Debug.LogWarning($"[Inventory] Cannot use {item.itemName}");
            return false;
        }

        /// <summary>
        /// Gunakan consumable.
        /// </summary>
        private bool UseConsumable(ConsumableData consumable, int slotIndex)
        {
            // Cari player stats
            PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();
            if (playerStats == null) return false;

            bool used = false;

            switch (consumable.consumableType)
            {
                case ConsumableType.HPPotion:
                    playerStats.Heal(consumable.effectValue);
                    used = true;
                    Debug.Log($"[Inventory] Used {consumable.itemName}, restored {consumable.effectValue} HP");
                    break;

                case ConsumableType.MPPotion:
                    playerStats.RestoreMP(consumable.effectValue);
                    used = true;
                    Debug.Log($"[Inventory] Used {consumable.itemName}, restored {consumable.effectValue} MP");
                    break;

                case ConsumableType.StaminaPotion:
                    playerStats.RestoreStamina(consumable.effectValue);
                    used = true;
                    Debug.Log($"[Inventory] Used {consumable.itemName}, restored {consumable.effectValue} Stamina");
                    break;

                case ConsumableType.BuffPotion:
                    // TODO: Apply buff (need buff system)
                    used = true;
                    Debug.Log($"[Inventory] Used {consumable.itemName}, buff applied");
                    break;

                case ConsumableType.Food:
                    // Food restores HP over time (simplified: instant heal)
                    playerStats.Heal(consumable.effectValue);
                    used = true;
                    Debug.Log($"[Inventory] Used {consumable.itemName}, restored {consumable.effectValue} HP");
                    break;
            }

            if (used)
            {
                RemoveItemAt(slotIndex, 1);
            }

            return used;
        }

        /// <summary>
        /// Equip item.
        /// </summary>
        private bool EquipItem(EquipmentData equipment, int slotIndex)
        {
            EquipmentManager equipMgr = FindAnyObjectByType<EquipmentManager>();
            if (equipMgr == null) return false;

            bool equipped = equipMgr.EquipItem(equipment);

            if (equipped)
            {
                RemoveItemAt(slotIndex, 1);
                Debug.Log($"[Inventory] Equipped {equipment.itemName}");
            }

            return equipped;
        }

        // === GETTERS ===

        /// <summary>
        /// Get item di slot tertentu.
        /// </summary>
        public InventoryItem GetItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= items.Count) return null;
            return items[slotIndex];
        }

        /// <summary>
        /// Get semua items.
        /// </summary>
        public List<InventoryItem> GetAllItems()
        {
            return items;
        }

        /// <summary>
        /// Cek apakah punya item.
        /// </summary>
        public bool HasItem(ItemData itemData, int quantity = 1)
        {
            int total = 0;

            foreach (InventoryItem item in items)
            {
                if (item != null && item.ItemData == itemData)
                {
                    total += item.Quantity;
                }
            }

            return total >= quantity;
        }

        /// <summary>
        /// Get jumlah item.
        /// </summary>
        public int GetItemQuantity(ItemData itemData)
        {
            int total = 0;

            foreach (InventoryItem item in items)
            {
                if (item != null && item.ItemData == itemData)
                {
                    total += item.Quantity;
                }
            }

            return total;
        }

        /// <summary>
        /// Get jumlah slot terpakai.
        /// </summary>
        public int GetUsedSlots()
        {
            int count = 0;
            foreach (InventoryItem item in items)
            {
                if (item != null && !item.IsEmpty())
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Cek inventory penuh.
        /// </summary>
        public bool IsFull()
        {
            return GetUsedSlots() >= maxSlots;
        }

        /// <summary>
        /// Sort inventory.
        /// </summary>
        public void SortInventory()
        {
            // Remove null entries
            items.RemoveAll(i => i == null || i.IsEmpty());

            // Fill with null
            while (items.Count < maxSlots)
            {
                items.Add(null);
            }

            // Re-index
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    items[i].SetSlot(i);
                }
            }

            OnInventoryChanged?.Invoke();
            Debug.Log("[Inventory] Sorted!");
        }

        /// <summary>
        /// Clear inventory.
        /// </summary>
        public void ClearInventory()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i] = null;
            }

            OnInventoryChanged?.Invoke();
            Debug.Log("[Inventory] Cleared!");
        }

        public int MaxSlots => maxSlots;

        // === STRING ID WRAPPERS (for Shop/Quest integration) ===

        /// <summary>
        /// Add item by ID. Creates a temporary ItemData if not found in database.
        /// </summary>
        public bool AddItem(string itemID, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemID) || quantity <= 0) return false;

            // Try to find existing item in inventory
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].ItemData != null &&
                    items[i].ItemData.id == itemID)
                {
                    // Stack to existing
                    if (items[i].CanStack())
                    {
                        items[i].AddQuantity(quantity);
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }

            // Create new ItemData
            ItemData newData = ScriptableObject.CreateInstance<ItemData>();
            newData.id = itemID;
            newData.itemName = itemID;
            newData.isStackable = true;
            newData.maxStackSize = 99;

            return AddItem(newData, quantity);
        }

        /// <summary>
        /// Remove item by ID.
        /// </summary>
        public bool RemoveItem(string itemID, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemID) || quantity <= 0) return false;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].ItemData != null &&
                    items[i].ItemData.id == itemID)
                {
                    items[i].RemoveQuantity(quantity);

                    if (items[i].Quantity <= 0)
                    {
                        items[i] = null;
                    }

                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if player has item by ID.
        /// </summary>
        public bool HasItem(string itemID, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemID)) return false;

            int total = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].ItemData != null &&
                    items[i].ItemData.id == itemID)
                {
                    total += items[i].Quantity;
                }
            }

            return total >= quantity;
        }

        /// <summary>
        /// Get item count by ID.
        /// </summary>
        public int GetItemCount(string itemID)
        {
            if (string.IsNullOrEmpty(itemID)) return 0;

            int total = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].ItemData != null &&
                    items[i].ItemData.id == itemID)
                {
                    total += items[i].Quantity;
                }
            }

            return total;
        }
    }
}
