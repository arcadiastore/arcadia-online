using UnityEngine;

namespace ArcadiaOnline.Inventory
{
    /// <summary>
    /// Tipe item.
    /// </summary>
    public enum ItemType
    {
        Equipment,      // Weapon, Armor, dll
        Consumable,     // Potion, Buff
        Material,       // Crafting material
        Quest           // Quest item
    }

    /// <summary>
    /// Data item dasar (ScriptableObject).
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Arcadia/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        public string id;
        public string itemName;
        [TextArea(2, 4)]
        public string description;
        public ItemType type;
        public Sprite icon;

        [Header("Stack")]
        public bool isStackable = true;
        public int maxStackSize = 99;

        [Header("Value")]
        public int buyPrice;
        public int sellPrice;

        [Header("Usage")]
        public bool isUsable = false;
        public bool isEquippable = false;
        public bool isDroppable = true;

        /// <summary>
        /// Get type display name.
        /// </summary>
        public string GetTypeName()
        {
            switch (type)
            {
                case ItemType.Equipment:
                    return "Equipment";
                case ItemType.Consumable:
                    return "Consumable";
                case ItemType.Material:
                    return "Material";
                case ItemType.Quest:
                    return "Quest Item";
                default:
                    return "Unknown";
            }
        }
    }
}
