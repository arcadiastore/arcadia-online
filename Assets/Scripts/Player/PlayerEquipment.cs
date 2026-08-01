using UnityEngine;
using ArcadiaOnline.Data;

namespace ArcadiaOnline.Player
{
    /// <summary>
    /// Equipment slot dasar. Lihat docs/01_GDD/10_Equipment.md untuk detail
    /// tipe equipment per job (Heavy/Cloth/Light armor, weapon type, dll).
    /// </summary>
    public class PlayerEquipment : MonoBehaviour
    {
        public EquipmentSlot weapon;
        public EquipmentSlot armor;
        public EquipmentSlot accessory1;
        public EquipmentSlot accessory2;

        public StatBlock GetTotalEquipmentBonus()
        {
            StatBlock total = default;
            if (weapon.isEquipped) total += weapon.statBonus;
            if (armor.isEquipped) total += armor.statBonus;
            if (accessory1.isEquipped) total += accessory1.statBonus;
            if (accessory2.isEquipped) total += accessory2.statBonus;
            return total;
        }
    }

    [System.Serializable]
    public struct EquipmentSlot
    {
        public string itemId;
        public bool isEquipped;
        public StatBlock statBonus;
    }
}
