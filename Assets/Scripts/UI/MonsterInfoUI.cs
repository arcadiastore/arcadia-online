using UnityEngine;
using UnityEngine.UI;
using ArcadiaOnline.Monster;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// UI yang menampilkan info monster saat diserang.
    /// </summary>
    public class MonsterInfoUI : MonoBehaviour
    {
        public static MonsterInfoUI Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private GameObject monsterInfoPanel;
        [SerializeField] private Text monsterNameText;
        [SerializeField] private Slider monsterHPSlider;
        [SerializeField] private Text monsterHPText;

        [Header("Settings")]
        [SerializeField] private float hideDelay = 3f;

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
            if (monsterInfoPanel != null)
            {
                monsterInfoPanel.SetActive(false);
            }
        }

        void Update()
        {
            if (!isShowing) return;

            if (currentTarget != null)
            {
                UpdateHPDisplay();

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
                    hideTimer = hideDelay;
                }
            }
        }

        public void ShowMonsterInfo(SimpleMonsterAI monster)
        {
            if (monster == null) return;

            currentTarget = monster;
            isShowing = true;
            hideTimer = hideDelay;

            if (monsterNameText != null)
            {
                monsterNameText.text = monster.MonsterName;
            }

            // Set slider range
            if (monsterHPSlider != null)
            {
                monsterHPSlider.minValue = 0;
                monsterHPSlider.maxValue = monster.MaxHP;
                monsterHPSlider.value = monster.CurrentHP;
            }

            UpdateHPDisplay();

            if (monsterInfoPanel != null)
            {
                monsterInfoPanel.SetActive(true);
            }
        }

        private void UpdateHPDisplay()
        {
            if (currentTarget == null) return;

            if (monsterHPSlider != null)
            {
                // Update value
                monsterHPSlider.value = currentTarget.CurrentHP;

                // Debug
                Debug.Log($"[MonsterInfoUI] HP: {currentTarget.CurrentHP}/{currentTarget.MaxHP} = {currentTarget.HPPercent}");
            }

            if (monsterHPText != null)
            {
                monsterHPText.text = $"{Mathf.Ceil(currentTarget.CurrentHP)}/{Mathf.Ceil(currentTarget.MaxHP)}";
            }
        }

        public void HideMonsterInfo()
        {
            isShowing = false;
            currentTarget = null;

            if (monsterInfoPanel != null)
            {
                monsterInfoPanel.SetActive(false);
            }
        }

        public void OnMonsterDamaged(SimpleMonsterAI monster)
        {
            if (monster == currentTarget)
            {
                UpdateHPDisplay();
            }
        }
    }
}
