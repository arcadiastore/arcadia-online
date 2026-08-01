using UnityEngine;
using System;

namespace ArcadiaOnline.Inventory
{
    /// <summary>
    /// Item di inventory (runtime data).
    /// </summary>
    [Serializable]
    public class InventoryItem
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private int quantity;
        [SerializeField] private int slotIndex;

        public ItemData ItemData => itemData;
        public int Quantity => quantity;
        public int SlotIndex => slotIndex;

        public InventoryItem(ItemData data, int qty, int slot)
        {
            itemData = data;
            quantity = qty;
            slotIndex = slot;
        }

        /// <summary>
        /// Tambah quantity.
        /// </summary>
        public void AddQuantity(int amount)
        {
            quantity += amount;
            if (itemData != null && itemData.isStackable)
            {
                quantity = Mathf.Min(quantity, itemData.maxStackSize);
            }
        }

        /// <summary>
        /// Kurangi quantity.
        /// </summary>
        public void RemoveQuantity(int amount)
        {
            quantity -= amount;
            if (quantity < 0) quantity = 0;
        }

        /// <summary>
        /// Set slot index.
        /// </summary>
        public void SetSlot(int index)
        {
            slotIndex = index;
        }

        /// <summary>
        /// Cek apakah item kosong.
        /// </summary>
        public bool IsEmpty()
        {
            return itemData == null || quantity <= 0;
        }

        /// <summary>
        /// Cek apakah bisa di-stack.
        /// </summary>
        public bool CanStack()
        {
            if (itemData == null || !itemData.isStackable) return false;
            return quantity < itemData.maxStackSize;
        }

        /// <summary>
        /// Get sisa stack space.
        /// </summary>
        public int GetRemainingStack()
        {
            if (itemData == null || !itemData.isStackable) return 0;
            return itemData.maxStackSize - quantity;
        }
    }
}
