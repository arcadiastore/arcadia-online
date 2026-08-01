using UnityEngine;
using UnityEngine.UI;
using ArcadiaOnline.Player;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// HUD terintegrasi: HP, MP, Stamina, Level, EXP.
    /// Attach ke GameObject Canvas.
    /// </summary>
    public class SimpleHUD : MonoBehaviour
    {
        [Header("Bars")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private Slider mpBar;
        [SerializeField] private Slider staminaBar;
        [SerializeField] private Slider expBar;

        [Header("Labels")]
        [SerializeField] private Text hpLabel;
        [SerializeField] private Text mpLabel;
        [SerializeField] private Text staminaLabel;
        [SerializeField] private Text levelLabel;
        [SerializeField] private Text expLabel;

        [Header("Player Reference")]
        [SerializeField] private SimplePlayerController playerController;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private LevelUpSystem levelUpSystem;

        [Header("Level Up Notification")]
        [SerializeField] private GameObject levelUpNotification;
        [SerializeField] private Text levelUpText;

        void Start()
        {
            // Auto find references
            if (playerController == null)
                playerController = FindAnyObjectByType<SimplePlayerController>();
            if (playerStats == null)
                playerStats = FindAnyObjectByType<PlayerStats>();
            if (levelUpSystem == null)
                levelUpSystem = FindAnyObjectByType<LevelUpSystem>();

            // Initialize bars
            InitializeBars();

            // Subscribe to level up events
            if (levelUpSystem != null)
            {
                levelUpSystem.OnLevelUp += OnLevelUp;
            }

            // Hide notification
            if (levelUpNotification != null)
                levelUpNotification.SetActive(false);
        }

        private void InitializeBars()
        {
            // HP Bar
            if (hpBar != null && playerStats != null)
            {
                hpBar.maxValue = playerStats.MaxHP;
                hpBar.value = playerStats.CurrentHP;
            }

            // MP Bar
            if (mpBar != null && playerStats != null)
            {
                mpBar.maxValue = playerStats.MaxMP;
                mpBar.value = playerStats.CurrentMP;
            }

            // Stamina Bar
            if (staminaBar != null && playerController != null)
            {
                staminaBar.maxValue = playerController.MaxStamina;
                staminaBar.value = playerController.CurrentStamina;
            }

            // EXP Bar
            if (expBar != null && levelUpSystem != null)
            {
                expBar.maxValue = levelUpSystem.ExpToNextLevel;
                expBar.value = levelUpSystem.CurrentEXP;
            }

            // Level Label
            if (levelLabel != null && levelUpSystem != null)
            {
                levelLabel.text = $"Lv. {levelUpSystem.CurrentLevel}";
            }
        }

        void Update()
        {
            UpdateHPBar();
            UpdateMPBar();
            UpdateStaminaBar();
            UpdateEXPBar();
        }

        private void UpdateHPBar()
        {
            if (playerStats == null || hpBar == null) return;

            hpBar.maxValue = playerStats.MaxHP;
            hpBar.value = playerStats.CurrentHP;

            if (hpLabel != null)
            {
                hpLabel.text = $"HP: {Mathf.CeilToInt(playerStats.CurrentHP)}/{Mathf.CeilToInt(playerStats.MaxHP)}";
            }
        }

        private void UpdateMPBar()
        {
            if (playerStats == null || mpBar == null) return;

            mpBar.maxValue = playerStats.MaxMP;
            mpBar.value = playerStats.CurrentMP;

            if (mpLabel != null)
            {
                mpLabel.text = $"MP: {Mathf.CeilToInt(playerStats.CurrentMP)}/{Mathf.CeilToInt(playerStats.MaxMP)}";
            }
        }

        private void UpdateStaminaBar()
        {
            if (playerController == null || staminaBar == null) return;

            staminaBar.maxValue = playerController.MaxStamina;
            staminaBar.value = playerController.CurrentStamina;

            if (staminaLabel != null)
            {
                staminaLabel.text = $"Stamina: {Mathf.CeilToInt(playerController.CurrentStamina)}/{Mathf.CeilToInt(playerController.MaxStamina)}";
            }
        }

        private void UpdateEXPBar()
        {
            if (levelUpSystem == null || expBar == null) return;

            expBar.maxValue = levelUpSystem.ExpToNextLevel;
            expBar.value = levelUpSystem.CurrentEXP;

            if (expLabel != null)
            {
                expLabel.text = $"EXP: {levelUpSystem.CurrentEXP}/{levelUpSystem.ExpToNextLevel}";
            }

            if (levelLabel != null)
            {
                levelLabel.text = $"Lv. {levelUpSystem.CurrentLevel}";
            }
        }

        /// <summary>
        /// Callback saat level up.
        /// </summary>
        private void OnLevelUp(int newLevel)
        {
            if (levelUpNotification != null && levelUpText != null)
            {
                levelUpText.text = $"LEVEL UP!\nLv. {newLevel}";
                levelUpNotification.SetActive(true);
                Invoke(nameof(HideLevelUpNotification), 2f);
            }
        }

        private void HideLevelUpNotification()
        {
            if (levelUpNotification != null)
                levelUpNotification.SetActive(false);
        }

        void OnDestroy()
        {
            if (levelUpSystem != null)
                levelUpSystem.OnLevelUp -= OnLevelUp;
        }
    }
}
