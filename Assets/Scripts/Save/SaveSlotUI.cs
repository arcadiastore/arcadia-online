using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.Save
{
    /// <summary>
    /// UI untuk individual save slot.
    /// </summary>
    public class SaveSlotUI : MonoBehaviour
    {
        [Header("Slot Info")]
        [SerializeField] private SaveSlotInfo slotInfo;

        [Header("Visual")]
        [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        [SerializeField] private Color savedColor = new Color(0.2f, 0.3f, 0.2f, 0.9f);
        [SerializeField] private Color hoverColor = new Color(0.3f, 0.4f, 0.3f, 0.9f);

        // Components
        private Image backgroundImage;
        private Button button;

        void Awake()
        {
            backgroundImage = GetComponent<Image>();
            button = GetComponent<Button>();
        }

        void Start()
        {
            // Setup hover effect
            if (button != null)
            {
                ColorBlock colors = button.colors;
                colors.highlightedColor = hoverColor;
                colors.pressedColor = new Color(0.4f, 0.5f, 0.4f, 0.9f);
                button.colors = colors;
            }
        }

        /// <summary>
        /// Initialize slot dengan info.
        /// </summary>
        public void Initialize(SaveSlotInfo info)
        {
            slotInfo = info;

            // Update visual
            if (backgroundImage != null)
            {
                backgroundImage.color = info.exists ? savedColor : emptyColor;
            }
        }

        /// <summary>
        /// Get slot info.
        /// </summary>
        public SaveSlotInfo GetSlotInfo()
        {
            return slotInfo;
        }

        /// <summary>
        /// Update visual state.
        /// </summary>
        public void UpdateVisual(bool isHighlighted)
        {
            if (backgroundImage != null)
            {
                if (isHighlighted)
                {
                    backgroundImage.color = hoverColor;
                }
                else
                {
                    backgroundImage.color = slotInfo.exists ? savedColor : emptyColor;
                }
            }
        }
    }
}
