using System.Collections.Generic;
using UnityEngine;

namespace ArcadiaOnline.Player
{
    /// <summary>
    /// Inventory dasar. Lihat docs/01_GDD/09_Items.md untuk detail item
    /// (rarity, stack, tipe) yang perlu diimplementasikan lebih lanjut.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int _capacity = 50;
        private readonly List<InventorySlot> _slots = new List<InventorySlot>();

        public bool IsFull => _slots.Count >= _capacity;

        public bool AddItem(string itemId, int quantity = 1)
        {
            if (IsFull) return false;

            var existing = _slots.Find(s => s.itemId == itemId);
            if (existing != null)
            {
                existing.quantity += quantity;
            }
            else
            {
                _slots.Add(new InventorySlot { itemId = itemId, quantity = quantity });
            }
            return true;
        }

        public bool RemoveItem(string itemId, int quantity = 1)
        {
            var existing = _slots.Find(s => s.itemId == itemId);
            if (existing == null || existing.quantity < quantity) return false;

            existing.quantity -= quantity;
            if (existing.quantity <= 0) _slots.Remove(existing);
            return true;
        }

        public IReadOnlyList<InventorySlot> GetSlots() => _slots;
    }

    [System.Serializable]
    public class InventorySlot
    {
        public string itemId;
        public int quantity;
    }
}
