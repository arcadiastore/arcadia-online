using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ArcadiaOnline.Equipment
{
    /// <summary>
    /// UI untuk satu slot equipment.
    /// Menampilkan icon, rarity border, dan bisa diklik.
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Elements")]
        [SerializeField] private Image slotBackground;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image borderImage;
        [SerializeField] private Text slotNameText;

        [Header("Default")]
        [SerializeField] private string defaultSlotName = "Empty";
        [SerializeField] private Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        [SerializeField] private Color filledColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        private EquipmentSlot slotType;
        private EquipmentUI equipmentUI;
        private EquipmentData currentItem;

        /// <summary>
        /// Initialize slot dengan type dan callback.
        /// </summary>
        public void Initialize(EquipmentSlot type, EquipmentUI ui)
        {
            slotType = type;
            equipmentUI = ui;

            // Set slot name
            if (slotNameText != null)
            {
                slotNameText.text = GetSlotDisplayName();
            }
        }

        /// <summary>
        /// Update display dengan item baru.
        /// </summary>
        public void UpdateDisplay(EquipmentData item)
        {
            currentItem = item;

            if (item != null)
            {
                // Ada item
                if (iconImage != null)
                {
                    iconImage.sprite = item.icon;
                    iconImage.color = Color.white;
                    iconImage.enabled = true;
                }

                if (borderImage != null)
                {
                    borderImage.color = item.GetRarityColor();
                    borderImage.enabled = true;
                }

                if (slotBackground != null)
                {
                    slotBackground.color = filledColor;
                }

                if (slotNameText != null)
                {
                    slotNameText.text = item.itemName;
                    slotNameText.color = item.GetRarityColor();
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

                if (borderImage != null)
                {
                    borderImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    borderImage.enabled = true;
                }

                if (slotBackground != null)
                {
                    slotBackground.color = emptyColor;
                }

                if (slotNameText != null)
                {
                    slotNameText.text = GetSlotDisplayName();
                    slotNameText.color = Color.gray;
                }
            }
        }

        /// <summary>
        /// Get display name untuk slot.
        /// </summary>
        private string GetSlotDisplayName()
        {
            switch (slotType)
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
                    return "Weapon 1H";
                case EquipmentSlot.WeaponTwoHand:
                    return "Weapon 2H";
                case EquipmentSlot.Costume:
                    return "Costume";
                default:
                    return "Unknown";
            }
        }

        // === INPUT HANDLERS ===

        public void OnPointerClick(PointerEventData eventData)
        {
            if (equipmentUI != null)
            {
                equipmentUI.OnSlotClicked(slotType);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (equipmentUI != null)
            {
                equipmentUI.OnSlotHover(slotType);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (equipmentUI != null)
            {
                equipmentUI.OnSlotExit();
            }
        }
    }
}
