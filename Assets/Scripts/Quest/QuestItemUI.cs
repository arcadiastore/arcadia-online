using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.Quest
{
    /// <summary>
    /// UI untuk satu item quest di list.
    /// </summary>
    public class QuestItemUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text questNameText;
        [SerializeField] private Text questLevelText;
        [SerializeField] private Text questTypeText;
        [SerializeField] private Image statusIcon;

        [Header("Colors")]
        [SerializeField] private Color activeColor = Color.yellow;
        [SerializeField] private Color completedColor = Color.green;
        [SerializeField] private Color availableColor = Color.white;

        private QuestData questData;

        /// <summary>
        /// Setup quest item.
        /// </summary>
        public void Setup(QuestData quest)
        {
            questData = quest;

            // Set quest name
            if (questNameText != null)
            {
                questNameText.text = quest.questName;
            }

            // Set level
            if (questLevelText != null)
            {
                questLevelText.text = $"Lv.{quest.recommendedLevel}";
            }

            // Set type
            if (questTypeText != null)
            {
                questTypeText.text = quest.mainType.ToString();
            }

            // Set status color
            UpdateStatusColor();
        }

        /// <summary>
        /// Update status icon color.
        /// </summary>
        private void UpdateStatusColor()
        {
            if (statusIcon == null || QuestManager.Instance == null) return;

            QuestStatus status = QuestManager.Instance.GetQuestStatus(questData.questID);

            switch (status)
            {
                case QuestStatus.Active:
                    statusIcon.color = activeColor;
                    break;
                case QuestStatus.Completed:
                    statusIcon.color = completedColor;
                    break;
                case QuestStatus.Available:
                    statusIcon.color = availableColor;
                    break;
                default:
                    statusIcon.color = Color.gray;
                    break;
            }
        }

        /// <summary>
        /// Get quest data.
        /// </summary>
        public QuestData GetQuestData()
        {
            return questData;
        }
    }
}
