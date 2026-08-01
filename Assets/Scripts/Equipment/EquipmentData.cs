using UnityEngine;

namespace ArcadiaOnline.Equipment
{
    /// <summary>
    /// Tipe equipment slot.
    /// </summary>
    public enum EquipmentSlot
    {
        Helm,           // Kepala
        TShirt,         // Armor atas (T-Shirt/Armor)
        Pants,          // Armor bawah
        Shoes,          // Sepatu
        WingsCape,      // Sayap / Cape
        RingLeft,       // Cincin kiri
        RingRight,      // Cincin kanan
        Necklace,       // Kalung
        WeaponOneHand,  // Senjata satu tangan
        WeaponTwoHand,  // Senjata dua tangan
        Costume         // Costume (cosmetic only)
    }

    /// <summary>
    /// Tipe weapon.
    /// </summary>
    public enum WeaponType
    {
        None,
        Sword,      // Pedang
        Staff,      // Tongkat sihir
        Bow,        // Busur
        Dagger,     // Belati
        Axe,        // Kapak
        Hammer,     // Palu
        Shield      // Perisai (untuk one-hand)
    }

    /// <summary>
    /// Rarity equipment.
    /// </summary>
    public enum EquipmentRarity
    {
        Common,     // Abu-abu - 60%
        Uncommon,   // Hijau - 25%
        Rare,       // Biru - 10%
        Epic,       // Ungu - 4%
        Legendary   // Emas - 1%
    }

    /// <summary>
    /// Data equipment (ScriptableObject).
    /// </summary>
    [CreateAssetMenu(fileName = "New Equipment", menuName = "Arcadia/Equipment")]
    public class EquipmentData : ScriptableObject
    {
        [Header("Basic Info")]
        public string id;
        public string itemName;
        [TextArea(2, 4)]
        public string description;
        public EquipmentSlot slot;
        public EquipmentRarity rarity = EquipmentRarity.Common;
        public int levelRequirement = 1;
        public int sellPrice = 10;

        [Header("Weapon Only")]
        public WeaponType weaponType = WeaponType.None;
        public bool isTwoHanded = false;

        [Header("Stats Bonus")]
        [Tooltip("Physical Attack")]
        public float atkBonus;
        [Tooltip("Magic Attack")]
        public float matkBbonus;
        [Tooltip("Physical Defense")]
        public float defBonus;
        [Tooltip("Magic Defense")]
        public float mdefBonus;
        [Tooltip("Max HP")]
        public float hpBonus;
        [Tooltip("Max MP")]
        public float mpBonus;
        [Tooltip("Speed")]
        public float spdBonus;
        [Tooltip("Critical Rate %")]
        public float critRateBonus;
        [Tooltip("Critical Damage %")]
        public float critDmgBonus;
        [Tooltip("Attack Speed %")]
        public float atkSpdBonus;

        [Header("Visual")]
        public Sprite icon;
        public Color glowColor = Color.white;

        // === HELPER METHODS ===

        /// <summary>
        /// Get rarity color untuk UI.
        /// </summary>
        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case EquipmentRarity.Common:
                    return new Color(0.7f, 0.7f, 0.7f);
                case EquipmentRarity.Uncommon:
                    return new Color(0.2f, 0.8f, 0.2f);
                case EquipmentRarity.Rare:
                    return new Color(0.2f, 0.4f, 0.9f);
                case EquipmentRarity.Epic:
                    return new Color(0.6f, 0.2f, 0.8f);
                case EquipmentRarity.Legendary:
                    return new Color(0.9f, 0.7f, 0.1f);
                default:
                    return Color.white;
            }
        }

        /// <summary>
        /// Get rarity display name.
        /// </summary>
        public string GetRarityName()
        {
            switch (rarity)
            {
                case EquipmentRarity.Common:
                    return "Common";
                case EquipmentRarity.Uncommon:
                    return "Uncommon";
                case EquipmentRarity.Rare:
                    return "Rare";
                case EquipmentRarity.Epic:
                    return "Epic";
                case EquipmentRarity.Legendary:
                    return "Legendary";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Get slot display name.
        /// </summary>
        public string GetSlotName()
        {
            switch (slot)
            {
                case EquipmentSlot.Helm:
                    return "Helm";
                case EquipmentSlot.TShirt:
                    return "T-Shirt";
                case EquipmentSlot.Pants:
                    return "Pants";
                case EquipmentSlot.Shoes:
                    return "Shoes";
                case EquipmentSlot.WingsCape:
                    return "Wings/Cape";
                case EquipmentSlot.RingLeft:
                    return "Ring L";
                case EquipmentSlot.RingRight:
                    return "Ring R";
                case EquipmentSlot.Necklace:
                    return "Necklace";
                case EquipmentSlot.WeaponOneHand:
                    return "Weapon (1H)";
                case EquipmentSlot.WeaponTwoHand:
                    return "Weapon (2H)";
                case EquipmentSlot.Costume:
                    return "Costume";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Cek apakah equipment ini weapon.
        /// </summary>
        public bool IsWeapon()
        {
            return slot == EquipmentSlot.WeaponOneHand || slot == EquipmentSlot.WeaponTwoHand;
        }

        /// <summary>
        /// Cek apakah equipment ini costume (no stats).
        /// </summary>
        public bool IsCostume()
        {
            return slot == EquipmentSlot.Costume;
        }

        /// <summary>
        /// Get total stats bonus untuk display.
        /// </summary>
        public string GetStatsDescription()
        {
            string desc = "";

            if (atkBonus > 0) desc += $"ATK +{atkBonus}\n";
            if (matkBbonus > 0) desc += $"MATK +{matkBbonus}\n";
            if (defBonus > 0) desc += $"DEF +{defBonus}\n";
            if (mdefBonus > 0) desc += $"MDEF +{mdefBonus}\n";
            if (hpBonus > 0) desc += $"HP +{hpBonus}\n";
            if (mpBonus > 0) desc += $"MP +{mpBonus}\n";
            if (spdBonus > 0) desc += $"SPD +{spdBonus}\n";
            if (critRateBonus > 0) desc += $"Crit Rate +{critRateBonus}%\n";
            if (critDmgBonus > 0) desc += $"Crit DMG +{critDmgBonus}%\n";
            if (atkSpdBonus > 0) desc += $"ATK SPD +{atkSpdBonus}%\n";

            if (string.IsNullOrEmpty(desc))
                desc = "No bonus stats";

            return desc.TrimEnd('\n');
        }
    }
}
