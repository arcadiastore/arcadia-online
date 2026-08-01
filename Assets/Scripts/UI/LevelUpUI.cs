using UnityEngine;
using UnityEngine.UI;
using ArcadiaOnline.Player;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// UI untuk Level Up System.
    /// Menampilkan level, EXP bar, HP, MP.
    /// </summary>
    public class LevelUpUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Text levelText;
        [SerializeField] private Image expBar;
        [SerializeField] private Image hpBar;
        [SerializeField] private Image mpBar;
        [SerializeField] private Text expText;
        [SerializeField] private Text hpText;
        [SerializeField] private Text mpText;

        [Header("Level Up Notification")]
        [SerializeField] private GameObject levelUpPanel;
        [SerializeField] private Text levelUpText;

        private LevelUpSystem levelUpSystem;

        void Start()
        {
            // Find LevelUpSystem
            levelUpSystem = LevelUpSystem.Instance;

            if (levelUpSystem != null)
            {
                // Subscribe to events
                levelUpSystem.OnLevelUp += ShowLevelUpNotification;
                levelUpSystem.OnEXPGained += OnEXPGained;

                // Initial update
                UpdateUI();
            }

            // Hide level up notification
            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(false);
            }
        }

        void Update()
        {
            if (levelUpSystem != null)
            {
                UpdateUI();
            }
        }

        /// <summary>
        /// Update semua UI elements.
        /// </summary>
        private void UpdateUI()
        {
            // Level text
            if (levelText != null)
            {
                levelText.text = $"Lv. {levelUpSystem.CurrentLevel}";
            }

            // EXP bar
            if (expBar != null)
            {
                expBar.fillAmount = levelUpSystem.GetEXPPercentage();
            }

            // EXP text
            if (expText != null)
            {
                expText.text = $"{levelUpSystem.CurrentEXP}/{levelUpSystem.ExpToNextLevel}";
            }

            // HP bar
            if (hpBar != null)
            {
                hpBar.fillAmount = levelUpSystem.GetHPPercentage();
            }

            // HP text
            if (hpText != null)
            {
                hpText.text = $"{Mathf.Ceil(levelUpSystem.CurrentHP)}/{Mathf.Ceil(levelUpSystem.MaxHP)}";
            }

            // MP bar
            if (mpBar != null)
            {
                mpBar.fillAmount = levelUpSystem.GetMPPercentage();
            }

            // MP text
            if (mpText != null)
            {
                mpText.text = $"{Mathf.Ceil(levelUpSystem.CurrentMP)}/{Mathf.Ceil(levelUpSystem.MaxMP)}";
            }
        }

        /// <summary>
        /// Show level up notification.
        /// </summary>
        private void ShowLevelUpNotification(int newLevel)
        {
            if (levelUpPanel != null && levelUpText != null)
            {
                levelUpText.text = $"LEVEL UP!\nLv. {newLevel}";
                levelUpPanel.SetActive(true);

                // Hide after 2 seconds
                Invoke(nameof(HideLevelUpNotification), 2f);
            }

            Debug.Log($"[UI] Level Up! Lv. {newLevel}");
        }

        /// <summary>
        /// Hide level up notification.
        /// </summary>
        private void HideLevelUpNotification()
        {
            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(false);
            }
        }

        /// <summary>
        /// On EXP gained.
        /// </summary>
        private void OnEXPGained(int amount)
        {
            // Could show floating EXP text here
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            if (levelUpSystem != null)
            {
                levelUpSystem.OnLevelUp -= ShowLevelUpNotification;
                levelUpSystem.OnEXPGained -= OnEXPGained;
            }
        }
    }
}
