using UnityEngine;
using UnityEngine.UI;
using ArcadiaOnline.Player;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// HUD sederhana untuk HP, MP, dan Stamina bar.
    /// Attach ke GameObject Canvas.
    /// </summary>
    public class SimpleHUD : MonoBehaviour
    {
        [Header("Bars")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private Slider mpBar;
        [SerializeField] private Slider staminaBar;

        [Header("Labels")]
        [SerializeField] private Text hpLabel;
        [SerializeField] private Text mpLabel;
        [SerializeField] private Text staminaLabel;

        [Header("Player Reference")]
        [SerializeField] private SimplePlayerController playerController;
        [SerializeField] private PlayerStats playerStats;

        void Start()
        {
            // Auto find player jika tidak di-assign
            if (playerController == null)
            {
                playerController = FindAnyObjectByType<SimplePlayerController>();
            }
            if (playerStats == null)
            {
                playerStats = FindAnyObjectByType<PlayerStats>();
            }

            // Set bar awal
            if (staminaBar != null && playerController != null)
            {
                staminaBar.maxValue = playerController.MaxStamina;
                staminaBar.value = playerController.CurrentStamina;
            }

            if (hpBar != null && playerStats != null)
            {
                hpBar.maxValue = playerStats.MaxHP;
                hpBar.value = playerStats.CurrentHP;
            }

            if (mpBar != null && playerStats != null)
            {
                mpBar.maxValue = playerStats.MaxMP;
                mpBar.value = playerStats.CurrentMP;
            }
        }

        void Update()
        {
            UpdateStaminaBar();
            UpdateHPBar();
            UpdateMPBar();
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
    }
}
