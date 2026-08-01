using UnityEngine;
using System.Collections.Generic;

namespace ArcadiaOnline.Equipment
{
    /// <summary>
    /// Manager untuk equip/unequip item.
    /// Attach ke player.
    /// </summary>
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance { get; private set; }

        [Header("Equipped Items")]
        [SerializeField] private EquipmentData helm;
        [SerializeField] private EquipmentData tshirt;
        [SerializeField] private EquipmentData pants;
        [SerializeField] private EquipmentData shoes;
        [SerializeField] private EquipmentData wingsCape;
        [SerializeField] private EquipmentData ringLeft;
        [SerializeField] private EquipmentData ringRight;
        [SerializeField] private EquipmentData necklace;
        [SerializeField] private EquipmentData weaponOneHand;
        [SerializeField] private EquipmentData weaponTwoHand;
        [SerializeField] private EquipmentData costume;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent OnEquipmentChanged;

        // Total stats bonus dari semua equipment
        private float totalATK;
        private float totalMATK;
        private float totalDEF;
        private float totalMDEF;
        private float totalHP;
        private float totalMP;
        private float totalSPD;
        private float totalCritRate;
        private float totalCritDmg;
        private float totalAtkSpd;

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
            RecalculateStats();
        }

        // === EQUIP / UNEQUIP ===

        /// <summary>
        /// Equip item ke slot yang sesuai.
        /// </summary>
        public bool EquipItem(EquipmentData item)
        {
            if (item == null) return false;

            // Cek level requirement
            Player.LevelUpSystem levelUp = GetComponent<Player.LevelUpSystem>();
            if (levelUp != null && levelUp.CurrentLevel < item.levelRequirement)
            {
                Debug.Log($"[Equipment] Level too low! Need {item.levelRequirement}");
                return false;
            }

            // Equip ke slot yang sesuai
            EquipmentData oldItem = null;

            switch (item.slot)
            {
                case EquipmentSlot.Helm:
                    oldItem = helm;
                    helm = item;
                    break;
                case EquipmentSlot.TShirt:
                    oldItem = tshirt;
                    tshirt = item;
                    break;
                case EquipmentSlot.Pants:
                    oldItem = pants;
                    pants = item;
                    break;
                case EquipmentSlot.Shoes:
                    oldItem = shoes;
                    shoes = item;
                    break;
                case EquipmentSlot.WingsCape:
                    oldItem = wingsCape;
                    wingsCape = item;
                    break;
                case EquipmentSlot.RingLeft:
                    oldItem = ringLeft;
                    ringLeft = item;
                    break;
                case EquipmentSlot.RingRight:
                    oldItem = ringRight;
                    ringRight = item;
                    break;
                case EquipmentSlot.Necklace:
                    oldItem = necklace;
                    necklace = item;
                    break;
                case EquipmentSlot.WeaponOneHand:
                    oldItem = weaponOneHand;
                    weaponOneHand = item;
                    break;
                case EquipmentSlot.WeaponTwoHand:
                    oldItem = weaponTwoHand;
                    weaponTwoHand = item;
                    break;
                case EquipmentSlot.Costume:
                    oldItem = costume;
                    costume = item;
                    break;
                default:
                    Debug.LogWarning($"[Equipment] Unknown slot: {item.slot}");
                    return false;
            }

            // Recalculate stats
            RecalculateStats();

            // Trigger event
            OnEquipmentChanged?.Invoke();

            Debug.Log($"[Equipment] Equipped: {item.itemName} ({item.GetSlotName()})");

            // Return old item (bisa ditambahkan ke inventory)
            if (oldItem != null)
            {
                Debug.Log($"[Equipment] Unequipped: {oldItem.itemName}");
                // TODO: Add oldItem to inventory
            }

            return true;
        }

        /// <summary>
        /// Unequip item dari slot.
        /// </summary>
        public EquipmentData UnequipItem(EquipmentSlot slot)
        {
            EquipmentData removedItem = null;

            switch (slot)
            {
                case EquipmentSlot.Helm:
                    removedItem = helm;
                    helm = null;
                    break;
                case EquipmentSlot.TShirt:
                    removedItem = tshirt;
                    tshirt = null;
                    break;
                case EquipmentSlot.Pants:
                    removedItem = pants;
                    pants = null;
                    break;
                case EquipmentSlot.Shoes:
                    removedItem = shoes;
                    shoes = null;
                    break;
                case EquipmentSlot.WingsCape:
                    removedItem = wingsCape;
                    wingsCape = null;
                    break;
                case EquipmentSlot.RingLeft:
                    removedItem = ringLeft;
                    ringLeft = null;
                    break;
                case EquipmentSlot.RingRight:
                    removedItem = ringRight;
                    ringRight = null;
                    break;
                case EquipmentSlot.Necklace:
                    removedItem = necklace;
                    necklace = null;
                    break;
                case EquipmentSlot.WeaponOneHand:
                    removedItem = weaponOneHand;
                    weaponOneHand = null;
                    break;
                case EquipmentSlot.WeaponTwoHand:
                    removedItem = weaponTwoHand;
                    weaponTwoHand = null;
                    break;
                case EquipmentSlot.Costume:
                    removedItem = costume;
                    costume = null;
                    break;
            }

            if (removedItem != null)
            {
                RecalculateStats();
                OnEquipmentChanged?.Invoke();
                Debug.Log($"[Equipment] Unequipped: {removedItem.itemName} from {slot}");
            }

            return removedItem;
        }

        /// <summary>
        /// Get equipped item di slot tertentu.
        /// </summary>
        public EquipmentData GetEquippedItem(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Helm:
                    return helm;
                case EquipmentSlot.TShirt:
                    return tshirt;
                case EquipmentSlot.Pants:
                    return pants;
                case EquipmentSlot.Shoes:
                    return shoes;
                case EquipmentSlot.WingsCape:
                    return wingsCape;
                case EquipmentSlot.RingLeft:
                    return ringLeft;
                case EquipmentSlot.RingRight:
                    return ringRight;
                case EquipmentSlot.Necklace:
                    return necklace;
                case EquipmentSlot.WeaponOneHand:
                    return weaponOneHand;
                case EquipmentSlot.WeaponTwoHand:
                    return weaponTwoHand;
                case EquipmentSlot.Costume:
                    return costume;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Cek apakah slot kosong.
        /// </summary>
        public bool IsSlotEmpty(EquipmentSlot slot)
        {
            return GetEquippedItem(slot) == null;
        }

        /// <summary>
        /// Get semua equipped items.
        /// </summary>
        public Dictionary<EquipmentSlot, EquipmentData> GetAllEquipped()
        {
            Dictionary<EquipmentSlot, EquipmentData> equipped = new Dictionary<EquipmentSlot, EquipmentData>();

            if (helm != null) equipped[EquipmentSlot.Helm] = helm;
            if (tshirt != null) equipped[EquipmentSlot.TShirt] = tshirt;
            if (pants != null) equipped[EquipmentSlot.Pants] = pants;
            if (shoes != null) equipped[EquipmentSlot.Shoes] = shoes;
            if (wingsCape != null) equipped[EquipmentSlot.WingsCape] = wingsCape;
            if (ringLeft != null) equipped[EquipmentSlot.RingLeft] = ringLeft;
            if (ringRight != null) equipped[EquipmentSlot.RingRight] = ringRight;
            if (necklace != null) equipped[EquipmentSlot.Necklace] = necklace;
            if (weaponOneHand != null) equipped[EquipmentSlot.WeaponOneHand] = weaponOneHand;
            if (weaponTwoHand != null) equipped[EquipmentSlot.WeaponTwoHand] = weaponTwoHand;
            if (costume != null) equipped[EquipmentSlot.Costume] = costume;

            return equipped;
        }

        // === STATS CALCULATION ===

        /// <summary>
        /// Recalculate total stats dari semua equipment.
        /// </summary>
        public void RecalculateStats()
        {
            totalATK = 0;
            totalMATK = 0;
            totalDEF = 0;
            totalMDEF = 0;
            totalHP = 0;
            totalMP = 0;
            totalSPD = 0;
            totalCritRate = 0;
            totalCritDmg = 0;
            totalAtkSpd = 0;

            // Hitung dari semua equipped items
            EquipmentData[] allEquipped = { helm, tshirt, pants, shoes, wingsCape,
                                            ringLeft, ringRight, necklace,
                                            weaponOneHand, weaponTwoHand, costume };

            foreach (EquipmentData item in allEquipped)
            {
                if (item != null)
                {
                    totalATK += item.atkBonus;
                    totalMATK += item.matkBbonus;
                    totalDEF += item.defBonus;
                    totalMDEF += item.mdefBonus;
                    totalHP += item.hpBonus;
                    totalMP += item.mpBonus;
                    totalSPD += item.spdBonus;
                    totalCritRate += item.critRateBonus;
                    totalCritDmg += item.critDmgBonus;
                    totalAtkSpd += item.atkSpdBonus;
                }
            }

            Debug.Log($"[Equipment] Stats recalculated - ATK+{totalATK}, DEF+{totalDEF}, HP+{totalHP}");
        }

        // === GETTERS ===

        public float TotalATK => totalATK;
        public float TotalMATK => totalMATK;
        public float TotalDEF => totalDEF;
        public float TotalMDEF => totalMDEF;
        public float TotalHP => totalHP;
        public float TotalMP => totalMP;
        public float TotalSPD => totalSPD;
        public float TotalCritRate => totalCritRate;
        public float TotalCritDmg => totalCritDmg;
        public float TotalAtkSpd => totalAtkSpd;
    }
}
