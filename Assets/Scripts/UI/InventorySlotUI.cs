using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ArcadiaOnline.Equipment;

namespace ArcadiaOnline.Inventory
{
    /// <summary>
    /// UI untuk satu slot inventory.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Elements")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private Text quantityText;
        [SerializeField] private Image borderImage;

        [Header("Colors")]
        [SerializeField] private Color normalColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        [SerializeField] private Color selectedColor = new Color(0.5f, 0.5f, 0.8f, 1f);
        [SerializeField] private Color hoverColor = new Color(0.4f, 0.4f, 0.4f, 1f);

        private int slotIndex;
        private InventoryUI inventoryUI;
        private InventoryItem currentItem;
        private bool isSelected = false;
        private bool isHovered = false;

        /// <summary>
        /// Initialize slot.
        /// </summary>
        public void Initialize(int index, InventoryUI ui)
        {
            slotIndex = index;
            inventoryUI = ui;

            // Get or create components
            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
            }

            // Create icon if needed
            if (iconImage == null)
            {
                GameObject iconObj = new GameObject("Icon");
                iconObj.transform.SetParent(transform, false);

                RectTransform iconRect = iconObj.AddComponent<RectTransform>();
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.offsetMin = new Vector2(5, 5);
                iconRect.offsetMax = new Vector2(-5, -5);

                iconImage = iconObj.AddComponent<Image>();
                iconImage.color = new Color(1, 1, 1, 0.3f);
                iconImage.enabled = false;
            }

            // Create quantity text if needed
            if (quantityText == null)
            {
                GameObject textObj = new GameObject("Quantity");
                textObj.transform.SetParent(transform, false);

                RectTransform textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = new Vector2(1, 0);
                textRect.anchorMax = new Vector2(1, 0);
                textRect.pivot = new Vector2(1, 0);
                textRect.anchoredPosition = new Vector2(-2, 2);
                textRect.sizeDelta = new Vector2(30, 15);

                quantityText = textObj.AddComponent<Text>();
                quantityText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                quantityText.fontSize = 10;
                quantityText.color = Color.white;
                quantityText.alignment = TextAnchor.LowerRight;
                quantityText.text = "";
            }

            // Create border if needed
            if (borderImage == null)
            {
                GameObject borderObj = new GameObject("Border");
                borderObj.transform.SetParent(transform, false);

                RectTransform borderRect = borderObj.AddComponent<RectTransform>();
                borderRect.anchorMin = Vector2.zero;
                borderRect.anchorMax = Vector2.one;
                borderRect.offsetMin = Vector2.zero;
                borderRect.offsetMax = Vector2.zero;

                borderImage = borderObj.AddComponent<Image>();
                borderImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                borderImage.raycastTarget = false;
            }
        }

        /// <summary>
        /// Update display dengan item.
        /// </summary>
        public void UpdateDisplay(InventoryItem item)
        {
            currentItem = item;

            if (item != null && !item.IsEmpty())
            {
                // Ada item
                if (iconImage != null)
                {
                    iconImage.sprite = item.ItemData.icon;
                    iconImage.color = Color.white;
                    iconImage.enabled = true;
                }

                if (quantityText != null)
                {
                    if (item.Quantity > 1)
                    {
                        quantityText.text = item.Quantity.ToString();
                    }
                    else
                    {
                        quantityText.text = "";
                    }
                }

                // Set border color berdasarkan rarity
                if (borderImage != null && item.ItemData is EquipmentData equipment)
                {
                    borderImage.color = equipment.GetRarityColor();
                }
            }
            else
            {
                // Slot kosong
                if (iconImage != null)
                {
                    iconImage.sprite = null;
                    iconImage.color = new Color(1, 1, 1, 0.3f);
                    iconImage.enabled = false;
                }

                if (quantityText != null)
                {
                    quantityText.text = "";
                }

                if (borderImage != null)
                {
                    borderImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }
            }

            // Update background color
            UpdateBackgroundColor();
        }

        /// <summary>
        /// Set selected state.
        /// </summary>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateBackgroundColor();
        }

        /// <summary>
        /// Update background color berdasarkan state.
        /// </summary>
        private void UpdateBackgroundColor()
        {
            if (backgroundImage == null) return;

            if (isSelected)
            {
                backgroundImage.color = selectedColor;
            }
            else if (isHovered)
            {
                backgroundImage.color = hoverColor;
            }
            else
            {
                backgroundImage.color = normalColor;
            }
        }

        // === INPUT HANDLERS ===

        public void OnPointerClick(PointerEventData eventData)
        {
            if (inventoryUI != null)
            {
                inventoryUI.SelectSlot(slotIndex);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            UpdateBackgroundColor();

            // Show tooltip on hover
            if (currentItem != null && !currentItem.IsEmpty())
            {
                if (inventoryUI != null)
                {
                    inventoryUI.ShowTooltip(currentItem);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            UpdateBackgroundColor();

            // Hide tooltip
            if (inventoryUI != null)
            {
                inventoryUI.HideTooltip();
            }
        }
    }
}
