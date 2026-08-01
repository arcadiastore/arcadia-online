using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ArcadiaOnline.Equipment;

namespace ArcadiaOnline.Inventory
{
    /// <summary>
    /// UI untuk Inventory System.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private GridLayoutGroup gridLayout;

        [Header("Tooltip")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private Text tooltipNameText;
        [SerializeField] private Text tooltipTypeText;
        [SerializeField] private Text tooltipDescText;
        [SerializeField] private Text tooltipStatsText;
        [SerializeField] private Text tooltipValueText;

        [Header("Buttons")]
        [SerializeField] private Button useButton;
        [SerializeField] private Button dropButton;
        [SerializeField] private Button sortButton;

        [Header("Info")]
        [SerializeField] private Text slotCountText;

        [Header("Settings")]
        [SerializeField] private int columns = 6;
        [SerializeField] private float slotSize = 60f;
        [SerializeField] private float slotSpacing = 5f;

        private bool isOpen = false;
        private int selectedSlot = -1;
        private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

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
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }

            // Setup grid layout
            SetupGridLayout();

            // Create slots
            CreateSlots();

            // Setup buttons
            SetupButtons();

            // Subscribe to inventory changes
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged.AddListener(RefreshUI);
            }
        }

        void Update()
        {
            // Toggle dengan tombol I
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }

            // Close dengan ESC
            if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            {
                CloseInventory();
            }
        }

        // === SETUP ===

        private void SetupGridLayout()
        {
            if (gridLayout == null && slotsParent != null)
            {
                gridLayout = slotsParent.GetComponent<GridLayoutGroup>();
                if (gridLayout == null)
                {
                    gridLayout = slotsParent.gameObject.AddComponent<GridLayoutGroup>();
                }
            }

            if (gridLayout != null)
            {
                gridLayout.cellSize = new Vector2(slotSize, slotSize);
                gridLayout.spacing = new Vector2(slotSpacing, slotSpacing);
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = columns;
            }
        }

        private void CreateSlots()
        {
            if (slotsParent == null || InventoryManager.Instance == null) return;

            // Clear existing slots
            foreach (Transform child in slotsParent)
            {
                Destroy(child.gameObject);
            }
            slotUIs.Clear();

            // Create new slots
            int maxSlots = InventoryManager.Instance.MaxSlots;
            for (int i = 0; i < maxSlots; i++)
            {
                GameObject slotObj = new GameObject($"Slot_{i}");
                slotObj.transform.SetParent(slotsParent, false);

                // Add RectTransform
                RectTransform slotRect = slotObj.AddComponent<RectTransform>();
                slotRect.sizeDelta = new Vector2(slotSize, slotSize);

                // Add Image (background)
                Image slotImage = slotObj.AddComponent<Image>();
                slotImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

                // Add InventorySlotUI
                InventorySlotUI slotUI = slotObj.AddComponent<InventorySlotUI>();
                slotUI.Initialize(i, this);
                slotUIs.Add(slotUI);
            }
        }

        private void SetupButtons()
        {
            if (useButton != null)
            {
                useButton.onClick.AddListener(OnUseButtonClicked);
            }

            if (dropButton != null)
            {
                dropButton.onClick.AddListener(OnDropButtonClicked);
            }

            if (sortButton != null)
            {
                sortButton.onClick.AddListener(OnSortButtonClicked);
            }
        }

        // === OPEN / CLOSE ===

        public void ToggleInventory()
        {
            if (isOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        public void OpenInventory()
        {
            isOpen = true;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }

            RefreshUI();

            Debug.Log("[InventoryUI] Opened");
        }

        public void CloseInventory()
        {
            isOpen = false;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            HideTooltip();
            DeselectSlot();

            Debug.Log("[InventoryUI] Closed");
        }

        // === REFRESH UI ===

        public void RefreshUI()
        {
            if (InventoryManager.Instance == null) return;

            // Update setiap slot
            for (int i = 0; i < slotUIs.Count; i++)
            {
                InventoryItem item = InventoryManager.Instance.GetItem(i);
                slotUIs[i].UpdateDisplay(item);
            }

            // Update slot count
            if (slotCountText != null)
            {
                int used = InventoryManager.Instance.GetUsedSlots();
                int max = InventoryManager.Instance.MaxSlots;
                slotCountText.text = $"{used}/{max}";
            }
        }

        // === SLOT SELECTION ===

        public void SelectSlot(int slotIndex)
        {
            // Deselect previous
            if (selectedSlot >= 0 && selectedSlot < slotUIs.Count)
            {
                slotUIs[selectedSlot].SetSelected(false);
            }

            selectedSlot = slotIndex;

            // Select new
            if (selectedSlot >= 0 && selectedSlot < slotUIs.Count)
            {
                slotUIs[selectedSlot].SetSelected(true);
            }

            // Show tooltip
            InventoryItem item = InventoryManager.Instance.GetItem(selectedSlot);
            ShowTooltip(item);

            // Enable/disable buttons
            UpdateButtons(item);
        }

        public void DeselectSlot()
        {
            if (selectedSlot >= 0 && selectedSlot < slotUIs.Count)
            {
                slotUIs[selectedSlot].SetSelected(false);
            }

            selectedSlot = -1;
            HideTooltip();
            UpdateButtons(null);
        }

        // === TOOLTIP ===

        public void ShowTooltip(InventoryItem item)
        {
            if (tooltipPanel == null || item == null || item.IsEmpty())
            {
                HideTooltip();
                return;
            }

            tooltipPanel.SetActive(true);

            if (tooltipNameText != null)
            {
                tooltipNameText.text = item.ItemData.itemName;
            }

            if (tooltipTypeText != null)
            {
                tooltipTypeText.text = $"[{item.ItemData.GetTypeName()}]";
            }

            if (tooltipDescText != null)
            {
                tooltipDescText.text = item.ItemData.description;
            }

            if (tooltipStatsText != null)
            {
                if (item.ItemData is EquipmentData equipment)
                {
                    tooltipStatsText.text = equipment.GetStatsDescription();
                }
                else if (item.ItemData is ConsumableData consumable)
                {
                    tooltipStatsText.text = consumable.GetEffectDescription();
                }
                else
                {
                    tooltipStatsText.text = "";
                }
            }

            if (tooltipValueText != null)
            {
                tooltipValueText.text = $"Sell: {item.ItemData.sellPrice}G";
            }
        }

        public void HideTooltip()
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }

        // === BUTTONS ===

        private void UpdateButtons(InventoryItem item)
        {
            bool hasItem = item != null && !item.IsEmpty();

            if (useButton != null)
            {
                useButton.interactable = hasItem && (item.ItemData.isUsable || item.ItemData.isEquippable);
            }

            if (dropButton != null)
            {
                dropButton.interactable = hasItem && item.ItemData.isDroppable;
            }
        }

        private void OnUseButtonClicked()
        {
            if (selectedSlot < 0) return;

            InventoryManager.Instance.UseItem(selectedSlot);
            RefreshUI();
        }

        private void OnDropButtonClicked()
        {
            if (selectedSlot < 0) return;

            InventoryItem item = InventoryManager.Instance.GetItem(selectedSlot);
            if (item != null && !item.IsEmpty())
            {
                Debug.Log($"[InventoryUI] Dropped {item.ItemData.itemName}");
                InventoryManager.Instance.RemoveItemAt(selectedSlot, item.Quantity);
                DeselectSlot();
                RefreshUI();
            }
        }

        private void OnSortButtonClicked()
        {
            InventoryManager.Instance.SortInventory();
            RefreshUI();
        }

        // === GETTERS ===

        public bool IsOpen => isOpen;
        public int SelectedSlot => selectedSlot;
    }
}
