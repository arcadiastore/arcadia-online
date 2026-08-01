using UnityEngine;
using UnityEngine.UI;
using ArcadiaOnline.Player;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// Simple HUD untuk Player: HP, MP, Stamina, EXP, Level.
    /// Auto-create UI jika tidak ada.
    /// </summary>
    public class SimpleHUD : MonoBehaviour
    {
        public static SimpleHUD Instance { get; private set; }

        [Header("Auto Create")]
        [SerializeField] private bool autoCreateUI = true;

        [Header("References")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private Slider mpBar;
        [SerializeField] private Slider staminaBar;
        [SerializeField] private Slider expBar;

        [Header("Labels")]
        [SerializeField] private Text hpText;
        [SerializeField] private Text mpText;
        [SerializeField] private Text staminaText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text expText;

        [Header("Player")]
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private LevelUpSystem levelUpSystem;

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
            // Auto find references
            if (playerStats == null)
                playerStats = FindAnyObjectByType<PlayerStats>();
            if (levelUpSystem == null)
                levelUpSystem = FindAnyObjectByType<LevelUpSystem>();

            // Auto create UI jika tidak ada
            if (autoCreateUI && hpBar == null)
            {
                CreateHUD();
            }
        }

        void Update()
        {
            UpdateHUD();
        }

        /// <summary>
        /// Create HUD UI otomatis.
        /// </summary>
        private void CreateHUD()
        {
            // Find Canvas
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[SimpleHUD] Canvas not found!");
                return;
            }

            // Create HUD Panel
            GameObject hudPanel = new GameObject("SimpleHUD");
            hudPanel.transform.SetParent(canvas.transform, false);

            RectTransform panelRect = hudPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(10, -10);
            panelRect.sizeDelta = new Vector2(200, 100);

            // Create HP Bar
            hpBar = CreateBar(hudPanel.transform, "HPBar", new Color(0.8f, 0.2f, 0.2f), 0);
            hpText = CreateText(hudPanel.transform, "HPText", "HP: 100/100", -0);

            // Create MP Bar
            mpBar = CreateBar(hudPanel.transform, "MPBar", new Color(0.2f, 0.2f, 0.8f), -25);
            mpText = CreateText(hudPanel.transform, "MPText", "MP: 50/50", -25);

            // Create Stamina Bar
            staminaBar = CreateBar(hudPanel.transform, "StaminaBar", new Color(0.2f, 0.8f, 0.2f), -50);
            staminaText = CreateText(hudPanel.transform, "StaminaText", "SP: 100/100", -50);

            // Create EXP Bar
            expBar = CreateBar(hudPanel.transform, "EXPBar", new Color(0.8f, 0.8f, 0.2f), -75);
            expText = CreateText(hudPanel.transform, "EXPText", "EXP: 0/100", -75);

            // Create Level Text
            levelText = CreateText(hudPanel.transform, "LevelText", "Lv. 1", -95);

            Debug.Log("[SimpleHUD] HUD created!");
        }

        private Slider CreateBar(Transform parent, string name, Color color, float yOffset)
        {
            // Background
            GameObject bg = new GameObject(name + "BG");
            bg.transform.SetParent(parent, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 1);
            bgRect.anchorMax = new Vector2(1, 1);
            bgRect.pivot = new Vector2(0.5f, 1);
            bgRect.anchoredPosition = new Vector2(0, yOffset);
            bgRect.sizeDelta = new Vector2(0, 15);
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // Slider
            GameObject slider = new GameObject(name);
            slider.transform.SetParent(bg.transform, false);
            RectTransform sliderRect = slider.AddComponent<RectTransform>();
            sliderRect.anchorMin = Vector2.zero;
            sliderRect.anchorMax = Vector2.one;
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            Slider sliderComp = slider.AddComponent<Slider>();
            sliderComp.interactable = false;
            sliderComp.transition = Selectable.Transition.None;
            sliderComp.minValue = 0;
            sliderComp.maxValue = 1;
            sliderComp.value = 1;

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(slider.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;

            // Fill
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = color;

            sliderComp.fillRect = fillRect;

            return sliderComp;
        }

        private Text CreateText(Transform parent, string name, string text, float yOffset)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 1);
            textRect.anchoredPosition = new Vector2(0, yOffset + 2);
            textRect.sizeDelta = new Vector2(0, 12);

            Text textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = 10;
            textComp.color = Color.white;
            textComp.alignment = TextAnchor.MiddleCenter;

            return textComp;
        }

        /// <summary>
        /// Update HUD display.
        /// </summary>
        private void UpdateHUD()
        {
            if (playerStats == null) return;

            // Update HP
            if (hpBar != null)
            {
                hpBar.value = playerStats.HPPercent;
            }
            if (hpText != null)
            {
                hpText.text = $"HP: {Mathf.Ceil(playerStats.CurrentHP)}/{Mathf.Ceil(playerStats.MaxHP)}";
            }

            // Update MP
            if (mpBar != null)
            {
                mpBar.value = playerStats.MPPercent;
            }
            if (mpText != null)
            {
                mpText.text = $"MP: {Mathf.Ceil(playerStats.CurrentMP)}/{Mathf.Ceil(playerStats.MaxMP)}";
            }

            // Update Stamina
            if (staminaBar != null)
            {
                staminaBar.value = playerStats.StaminaPercent;
            }
            if (staminaText != null)
            {
                staminaText.text = $"SP: {Mathf.Ceil(playerStats.CurrentStamina)}/{Mathf.Ceil(playerStats.MaxStamina)}";
            }

            // Update Level & EXP
            if (levelUpSystem != null)
            {
                if (levelText != null)
                {
                    levelText.text = $"Lv. {levelUpSystem.CurrentLevel}";
                }
                if (expBar != null)
                {
                    expBar.value = levelUpSystem.EXPPercent;
                }
                if (expText != null)
                {
                    expText.text = $"EXP: {levelUpSystem.CurrentEXP}/{levelUpSystem.EXPToNextLevel}";
                }
            }
        }
    }
}
