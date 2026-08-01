using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArcadiaOnline.Equipment
{
    /// <summary>
    /// UI untuk Equipment System.
    /// Menampilkan slot equipment dan stats.
    /// </summary>
    public class EquipmentUI : MonoBehaviour
    {
        public static EquipmentUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject equipmentPanel;
        [SerializeField] private Transform slotsParent;

        [Header("Slot References")]
        [SerializeField] private EquipmentSlotUI helmSlot;
        [SerializeField] private EquipmentSlotUI tshirtSlot;
        [SerializeField] private EquipmentSlotUI pantsSlot;
        [SerializeField] private EquipmentSlotUI shoesSlot;
        [SerializeField] private EquipmentSlotUI wingsCapeSlot;
        [SerializeField] private EquipmentSlotUI ringLeftSlot;
        [SerializeField] private EquipmentSlotUI ringRightSlot;
        [SerializeField] private EquipmentSlotUI necklaceSlot;
        [SerializeField] private EquipmentSlotUI weaponOneHandSlot;
        [SerializeField] private EquipmentSlotUI weaponTwoHandSlot;
        [SerializeField] private EquipmentSlotUI costumeSlot;

        [Header("Stats Display")]
        [SerializeField] private Text statsText;
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text levelText;

        [Header("Tooltip")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private Text tooltipNameText;
        [SerializeField] private Text tooltipTypeText;
        [SerializeField] private Text tooltipStatsText;
        [SerializeField] private Text tooltipDescText;
        [SerializeField] private Text tooltipLevelText;

        private bool isOpen = false;

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
            // Hide panel awal
            if (equipmentPanel != null)
            {
                equipmentPanel.SetActive(false);
            }

            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }

            // Setup slot callbacks
            SetupSlots();
        }

        void Update()
        {
            // Toggle dengan tombol E
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleEquipment();
            }

            // Close dengan ESC
            if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            {
                CloseEquipment();
            }
        }

        /// <summary>
        /// Setup slot callbacks.
        /// </summary>
        private void SetupSlots()
        {
            SetupSlot(helmSlot, EquipmentSlot.Helm);
            SetupSlot(tshirtSlot, EquipmentSlot.TShirt);
            SetupSlot(pantsSlot, EquipmentSlot.Pants);
            SetupSlot(shoesSlot, EquipmentSlot.Shoes);
            SetupSlot(wingsCapeSlot, EquipmentSlot.WingsCape);
            SetupSlot(ringLeftSlot, EquipmentSlot.RingLeft);
            SetupSlot(ringRightSlot, EquipmentSlot.RingRight);
            SetupSlot(necklaceSlot, EquipmentSlot.Necklace);
            SetupSlot(weaponOneHandSlot, EquipmentSlot.WeaponOneHand);
            SetupSlot(weaponTwoHandSlot, EquipmentSlot.WeaponTwoHand);
            SetupSlot(costumeSlot, EquipmentSlot.Costume);
        }

        private void SetupSlot(EquipmentSlotUI slotUI, EquipmentSlot slotType)
        {
            if (slotUI != null)
            {
                slotUI.Initialize(slotType, this);
            }
        }

        // === OPEN / CLOSE ===

        public void ToggleEquipment()
        {
            if (isOpen)
                CloseEquipment();
            else
                OpenEquipment();
        }

        public void OpenEquipment()
        {
            isOpen = true;

            if (equipmentPanel != null)
            {
                equipmentPanel.SetActive(true);
            }

            RefreshUI();

            Debug.Log("[EquipmentUI] Opened");
        }

        public void CloseEquipment()
        {
            isOpen = false;

            if (equipmentPanel != null)
            {
                equipmentPanel.SetActive(false);
            }

            HideTooltip();

            Debug.Log("[EquipmentUI] Closed");
        }

        // === REFRESH UI ===

        public void RefreshUI()
        {
            if (EquipmentManager.Instance == null) return;

            // Update setiap slot
            UpdateSlot(helmSlot, EquipmentSlot.Helm);
            UpdateSlot(tshirtSlot, EquipmentSlot.TShirt);
            UpdateSlot(pantsSlot, EquipmentSlot.Pants);
            UpdateSlot(shoesSlot, EquipmentSlot.Shoes);
            UpdateSlot(wingsCapeSlot, EquipmentSlot.WingsCape);
            UpdateSlot(ringLeftSlot, EquipmentSlot.RingLeft);
            UpdateSlot(ringRightSlot, EquipmentSlot.RingRight);
            UpdateSlot(necklaceSlot, EquipmentSlot.Necklace);
            UpdateSlot(weaponOneHandSlot, EquipmentSlot.WeaponOneHand);
            UpdateSlot(weaponTwoHandSlot, EquipmentSlot.WeaponTwoHand);
            UpdateSlot(costumeSlot, EquipmentSlot.Costume);

            // Update stats display
            UpdateStatsDisplay();
        }

        private void UpdateSlot(EquipmentSlotUI slotUI, EquipmentSlot slotType)
        {
            if (slotUI == null) return;

            EquipmentData item = EquipmentManager.Instance.GetEquippedItem(slotType);
            slotUI.UpdateDisplay(item);
        }

        private void UpdateStatsDisplay()
        {
            if (statsText == null) return;

            EquipmentManager mgr = EquipmentManager.Instance;

            string stats = "=== Equipment Bonus ===\n";
            stats += $"ATK: +{mgr.TotalATK}\n";
            stats += $"MATK: +{mgr.TotalMATK}\n";
            stats += $"DEF: +{mgr.TotalDEF}\n";
            stats += $"MDEF: +{mgr.TotalMDEF}\n";
            stats += $"HP: +{mgr.TotalHP}\n";
            stats += $"MP: +{mgr.TotalMP}\n";
            stats += $"SPD: +{mgr.TotalSPD}\n";

            if (mgr.TotalCritRate > 0)
                stats += $"Crit Rate: +{mgr.TotalCritRate}%\n";
            if (mgr.TotalCritDmg > 0)
                stats += $"Crit DMG: +{mgr.TotalCritDmg}%\n";
            if (mgr.TotalAtkSpd > 0)
                stats += $"ATK SPD: +{mgr.TotalAtkSpd}%\n";

            statsText.text = stats;

            // Update player name & level
            Player.LevelUpSystem levelUp = FindObjectOfType<Player.LevelUpSystem>();
            if (levelUp != null)
            {
                if (playerNameText != null)
                    playerNameText.text = "Player";
                if (levelText != null)
                    levelText.text = $"Lv. {levelUp.CurrentLevel}";
            }
        }

        // === TOOLTIP ===

        public void ShowTooltip(EquipmentData item)
        {
            if (tooltipPanel == null || item == null) return;

            tooltipPanel.SetActive(true);

            if (tooltipNameText != null)
            {
                tooltipNameText.text = item.itemName;
                tooltipNameText.color = item.GetRarityColor();
            }

            if (tooltipTypeText != null)
            {
                tooltipTypeText.text = $"[{item.GetRarityName()}] {item.GetSlotName()}";
            }

            if (tooltipStatsText != null)
            {
                tooltipStatsText.text = item.GetStatsDescription();
            }

            if (tooltipDescText != null)
            {
                tooltipDescText.text = item.description;
            }

            if (tooltipLevelText != null)
            {
                tooltipLevelText.text = $"Req. Level: {item.levelRequirement}";
            }
        }

        public void HideTooltip()
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }

        // === SLOT CALLBACK ===

        public void OnSlotClicked(EquipmentSlot slotType)
        {
            if (EquipmentManager.Instance == null) return;

            EquipmentData item = EquipmentManager.Instance.GetEquippedItem(slotType);

            if (item != null)
            {
                // Unequip
                EquipmentData removed = EquipmentManager.Instance.UnequipItem(slotType);
                if (removed != null)
                {
                    // TODO: Add to inventory
                    Debug.Log($"[EquipmentUI] Unequipped: {removed.itemName}");
                }

                RefreshUI();
            }
        }

        public void OnSlotHover(EquipmentSlot slotType)
        {
            if (EquipmentManager.Instance == null) return;

            EquipmentData item = EquipmentManager.Instance.GetEquippedItem(slotType);
            ShowTooltip(item);
        }

        public void OnSlotExit()
        {
            HideTooltip();
        }

        public bool IsOpen => isOpen;
    }
}
