using UnityEngine;
using UnityEngine.UI;
using ArcadiaOnline.Monster;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// UI yang menampilkan info monster saat diserang.
    /// Muncul saat klik monster, hilang saat monster kembali patrol.
    /// </summary>
    public class MonsterInfoUI : MonoBehaviour
    {
        public static MonsterInfoUI Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private GameObject monsterInfoPanel;
        [SerializeField] private Text monsterNameText;
        [SerializeField] private Image monsterHPBar;
        [SerializeField] private Text monsterHPText;

        [Header("Settings")]
        [SerializeField] private float hideDelay = 3f; // Delay sebelum hide setelah monster stop chase

        private SimpleMonsterAI currentTarget;
        private float hideTimer;
        private bool isShowing = false;

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
            // Hide awal
            if (monsterInfoPanel != null)
            {
                monsterInfoPanel.SetActive(false);
            }
        }

        void Update()
        {
            if (!isShowing) return;

            // Update HP bar
            if (currentTarget != null)
            {
                UpdateHPDisplay();

                // Cek apakah monster masih chase/attack
                // Jika kembali patrol, start hide timer
                if (!currentTarget.IsChasing)
                {
                    hideTimer -= Time.deltaTime;
                    if (hideTimer <= 0)
                    {
                        HideMonsterInfo();
                    }
                }
                else
                {
                    hideTimer = hideDelay; // Reset timer saat masih chase
                }
            }
        }

        /// <summary>
        /// Tampilkan info monster.
        /// </summary>
        public void ShowMonsterInfo(SimpleMonsterAI monster)
        {
            if (monster == null) return;

            currentTarget = monster;
            isShowing = true;
            hideTimer = hideDelay;

            // Update UI
            if (monsterNameText != null)
            {
                monsterNameText.text = monster.MonsterName;
            }

            UpdateHPDisplay();

            // Show panel
            if (monsterInfoPanel != null)
            {
                monsterInfoPanel.SetActive(true);
            }

            Debug.Log($"[MonsterInfoUI] Show: {monster.MonsterName}");
        }

        /// <summary>
        /// Update tampilan HP.
        /// </summary>
        private void UpdateHPDisplay()
        {
            if (currentTarget == null) return;

            // Update HP bar
            if (monsterHPBar != null)
            {
                monsterHPBar.fillAmount = currentTarget.HPPercent;
            }

            // Update HP text
            if (monsterHPText != null)
            {
                monsterHPText.text = $"{Mathf.Ceil(currentTarget.CurrentHP)}/{Mathf.Ceil(currentTarget.MaxHP)}";
            }
        }

        /// <summary>
        /// Sembunyikan info monster.
        /// </summary>
        public void HideMonsterInfo()
        {
            isShowing = false;
            currentTarget = null;

            if (monsterInfoPanel != null)
            {
                monsterInfoPanel.SetActive(false);
            }

            Debug.Log("[MonsterInfoUI] Hide");
        }

        /// <summary>
        /// Update saat monster terkena damage.
        /// </summary>
        public void OnMonsterDamaged(SimpleMonsterAI monster)
        {
            if (monster == currentTarget)
            {
                UpdateHPDisplay();
            }
        }
    }
}
