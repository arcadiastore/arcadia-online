using UnityEngine;

namespace ArcadiaOnline.Inventory
{
    /// <summary>
    /// Tipe consumable.
    /// </summary>
    public enum ConsumableType
    {
        HPPotion,       // Pulihkan HP
        MPPotion,       // Pulihkan MP
        StaminaPotion,  // Pulihkan Stamina
        BuffPotion,     // Buff stats
        Food            // Regen over time
    }

    /// <summary>
    /// Data consumable (potion, food, dll).
    /// </summary>
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Arcadia/Consumable")]
    public class ConsumableData : ItemData
    {
        [Header("Consumable")]
        public ConsumableType consumableType;
        public float effectValue;       // HP/MP/Stamina yang dipulihkan
        public float effectDuration;    // Durasi effect (0 = instant)
        public float cooldown = 1f;     // Cooldown penggunaan

        [Header("Buff (jika BuffPotion)")]
        public float atkBuff;
        public float defBuff;
        public float spdBuff;
        public float critRateBuff;
        public float critDmgBuff;

        void OnValidate()
        {
            type = ItemType.Consumable;
            isUsable = true;
            isEquippable = false;
        }

        /// <summary>
        /// Get effect description untuk UI.
        /// </summary>
        public string GetEffectDescription()
        {
            string desc = "";

            switch (consumableType)
            {
                case ConsumableType.HPPotion:
                    desc = $"Restore {effectValue} HP";
                    break;
                case ConsumableType.MPPotion:
                    desc = $"Restore {effectValue} MP";
                    break;
                case ConsumableType.StaminaPotion:
                    desc = $"Restore {effectValue} Stamina";
                    break;
                case ConsumableType.BuffPotion:
                    desc = "Buff:";
                    if (atkBuff > 0) desc += $" ATK+{atkBuff}";
                    if (defBuff > 0) desc += $" DEF+{defBuff}";
                    if (spdBuff > 0) desc += $" SPD+{spdBuff}";
                    if (critRateBuff > 0) desc += $" Crit+{critRateBuff}%";
                    if (critDmgBuff > 0) desc += $" CritDMG+{critDmgBuff}%";
                    break;
                case ConsumableType.Food:
                    desc = $"Regen {effectValue} HP over {effectDuration}s";
                    break;
            }

            return desc;
        }
    }
}
