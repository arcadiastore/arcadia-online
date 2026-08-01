using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArcadiaOnline.Quest
{
    /// <summary>
    /// UI untuk quest system.
    /// </summary>
    public class QuestUI : MonoBehaviour
    {
        public static QuestUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject questPanel;

        [Header("Auto-Create UI")]
        [SerializeField] private bool autoCreateUI = true;

        // Internal references (auto-created)
        private Transform questListParent;
        private GameObject detailPanel;
        private Text questNameText;
        private Text questDescText;
        private Text objectivesText;
        private Text rewardsText;
        private Button acceptButton;
        private Button abandonButton;
        private Button claimButton;
        private Button activeTab;
        private Button availableTab;
        private Button completedTab;

        // State
        private QuestData selectedQuest;
        private int currentTab = 0;
        private bool isPanelActive = false;

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
            if (autoCreateUI && questPanel == null)
            {
                CreateQuestUI();
            }

            // Register events
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnQuestAccepted += OnQuestAccepted;
                QuestManager.Instance.OnQuestCompleted += OnQuestCompleted;
                QuestManager.Instance.OnQuestClaimed += OnQuestClaimed;
                QuestManager.Instance.OnObjectiveUpdated += OnObjectiveUpdated;
            }

            // Hide panel at end of frame
            StartCoroutine(HidePanelAtEndOfFrame());
        }

        private System.Collections.IEnumerator HidePanelAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            HidePanel();
        }

        void Update()
        {
            // Toggle quest panel dengan J
            if (Input.GetKeyDown(KeyCode.J))
            {
                ToggleQuestPanel();
            }
        }

        /// <summary>
        /// Toggle quest panel.
        /// </summary>
        public void ToggleQuestPanel()
        {
            if (isPanelActive)
            {
                HidePanel();
            }
            else
            {
                ShowPanel();
            }
        }

        /// <summary>
        /// Show quest panel.
        /// </summary>
        private void ShowPanel()
        {
            if (questPanel != null)
            {
                questPanel.SetActive(true);
                isPanelActive = true;
                RefreshQuestList();
                Debug.Log("[QuestUI] Panel shown");
            }
        }

        /// <summary>
        /// Hide quest panel.
        /// </summary>
        private void HidePanel()
        {
            if (questPanel != null)
            {
                questPanel.SetActive(false);
                isPanelActive = false;
                Debug.Log("[QuestUI] Panel hidden");
            }
        }

        /// <summary>
        /// Refresh quest list.
        /// </summary>
        private void RefreshQuestList()
        {
            Debug.Log($"[QuestUI] RefreshQuestList called. Tab: {currentTab}");

            if (QuestManager.Instance == null)
            {
                Debug.LogWarning("[QuestUI] QuestManager.Instance is null!");
                return;
            }

            // Clear existing items
            if (questListParent != null)
            {
                foreach (Transform child in questListParent)
                {
                    Destroy(child.gameObject);
                }
            }

            // Get quests based on tab
            List<QuestData> quests = new List<QuestData>();
            switch (currentTab)
            {
                case 0:
                    quests = QuestManager.Instance.GetActiveQuests();
                    break;
                case 1:
                    quests = QuestManager.Instance.GetAvailableQuests();
                    break;
                case 2:
                    quests = QuestManager.Instance.GetCompletedQuests();
                    break;
            }

            Debug.Log($"[QuestUI] Quests to display: {quests.Count}");

            // Create quest items directly
            foreach (QuestData quest in quests)
            {
                CreateQuestItem(quest);
            }

            // Update tab button colors
            UpdateTabColors();
        }

        /// <summary>
        /// Create quest item directly.
        /// </summary>
        private void CreateQuestItem(QuestData quest)
        {
            if (questListParent == null) return;

            // Create item
            GameObject item = new GameObject("QuestItem_" + quest.questName);
            item.transform.SetParent(questListParent, false);

            RectTransform rect = item.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 60);

            // Add LayoutElement for VerticalLayoutGroup
            LayoutElement layoutElement = item.AddComponent<LayoutElement>();
            layoutElement.minHeight = 60;
            layoutElement.preferredHeight = 60;

            Image bg = item.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            Button button = item.AddComponent<Button>();

            // Status Icon (left side)
            GameObject iconObj = new GameObject("StatusIcon");
            iconObj.transform.SetParent(item.transform, false);

            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.2f);
            iconRect.anchorMax = new Vector2(0, 0.8f);
            iconRect.offsetMin = new Vector2(8, 0);
            iconRect.offsetMax = new Vector2(25, 0);

            Image icon = iconObj.AddComponent<Image>();
            icon.color = GetQuestStatusColor(quest);

            // Quest Name Text (center-left)
            GameObject nameObj = new GameObject("QuestName");
            nameObj.transform.SetParent(item.transform, false);

            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.05f, 0.5f);
            nameRect.anchorMax = new Vector2(0.75f, 1);
            nameRect.offsetMin = new Vector2(5, 0);
            nameRect.offsetMax = new Vector2(-5, -5);

            Text nameText = nameObj.AddComponent<Text>();
            nameText.text = quest.questName;
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyle.Bold;
            nameText.alignment = TextAnchor.MiddleLeft;
            nameText.color = Color.white;
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Quest Type Text (below name)
            GameObject typeObj = new GameObject("QuestType");
            typeObj.transform.SetParent(item.transform, false);

            RectTransform typeRect = typeObj.AddComponent<RectTransform>();
            typeRect.anchorMin = new Vector2(0.05f, 0);
            typeRect.anchorMax = new Vector2(0.75f, 0.5f);
            typeRect.offsetMin = new Vector2(5, 5);
            typeRect.offsetMax = new Vector2(-5, 0);

            Text typeText = typeObj.AddComponent<Text>();
            typeText.text = quest.mainType.ToString();
            typeText.fontSize = 12;
            typeText.alignment = TextAnchor.MiddleLeft;
            typeText.color = new Color(0.7f, 0.7f, 0.7f);
            typeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Level Text (right side)
            GameObject levelObj = new GameObject("LevelText");
            levelObj.transform.SetParent(item.transform, false);

            RectTransform levelRect = levelObj.AddComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0.75f, 0);
            levelRect.anchorMax = new Vector2(1, 1);
            levelRect.offsetMin = Vector2.zero;
            levelRect.offsetMax = new Vector2(-10, 0);

            Text levelText = levelObj.AddComponent<Text>();
            levelText.text = $"Lv.{quest.recommendedLevel}";
            levelText.fontSize = 14;
            levelText.fontStyle = FontStyle.Bold;
            levelText.alignment = TextAnchor.MiddleRight;
            levelText.color = Color.yellow;
            levelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Add click listener
            QuestData questRef = quest;
            button.onClick.AddListener(() => SelectQuest(questRef));

            Debug.Log($"[QuestUI] Created quest item: {quest.questName}");
        }

        /// <summary>
        /// Get quest status color.
        /// </summary>
        private Color GetQuestStatusColor(QuestData quest)
        {
            if (QuestManager.Instance == null) return Color.gray;

            QuestStatus status = QuestManager.Instance.GetQuestStatus(quest.questID);

            switch (status)
            {
                case QuestStatus.Active:
                    return Color.yellow;
                case QuestStatus.Completed:
                    return Color.green;
                case QuestStatus.Available:
                    return Color.white;
                default:
                    return Color.gray;
            }
        }

        /// <summary>
        /// Update tab button colors.
        /// </summary>
        private void UpdateTabColors()
        {
            Color activeColor = new Color(0.3f, 0.6f, 1f);
            Color inactiveColor = new Color(0.5f, 0.5f, 0.5f);

            if (activeTab != null)
            {
                Image img = activeTab.GetComponent<Image>();
                if (img != null) img.color = currentTab == 0 ? activeColor : inactiveColor;
            }

            if (availableTab != null)
            {
                Image img = availableTab.GetComponent<Image>();
                if (img != null) img.color = currentTab == 1 ? activeColor : inactiveColor;
            }

            if (completedTab != null)
            {
                Image img = completedTab.GetComponent<Image>();
                if (img != null) img.color = currentTab == 2 ? activeColor : inactiveColor;
            }
        }

        /// <summary>
        /// Select quest to show details.
        /// </summary>
        private void SelectQuest(QuestData quest)
        {
            selectedQuest = quest;
            ShowQuestDetails(quest);
        }

        /// <summary>
        /// Show quest details.
        /// </summary>
        private void ShowQuestDetails(QuestData quest)
        {
            if (detailPanel == null || quest == null) return;

            detailPanel.SetActive(true);

            if (questNameText != null)
            {
                questNameText.text = quest.questName;
            }

            if (questDescText != null)
            {
                questDescText.text = quest.description;
            }

            if (objectivesText != null)
            {
                string objText = "";
                foreach (var objective in quest.objectives)
                {
                    string status = objective.IsComplete() ? "<color=green>[Done]</color>" : "[ ]";
                    objText += $"{status} {objective.description} ({objective.GetProgressString()})\n";
                }
                objectivesText.text = objText;
            }

            if (rewardsText != null)
            {
                string rewardText = "";
                if (quest.rewards.expReward > 0)
                    rewardText += $"EXP: +{quest.rewards.expReward}\n";
                if (quest.rewards.goldReward > 0)
                    rewardText += $"Gold: +{quest.rewards.goldReward}\n";
                rewardsText.text = rewardText;
            }

            UpdateButtons(quest);
        }

        /// <summary>
        /// Update button visibility.
        /// </summary>
        private void UpdateButtons(QuestData quest)
        {
            if (QuestManager.Instance == null) return;

            QuestStatus status = QuestManager.Instance.GetQuestStatus(quest.questID);

            if (acceptButton != null)
                acceptButton.gameObject.SetActive(status == QuestStatus.Available);

            if (abandonButton != null)
                abandonButton.gameObject.SetActive(status == QuestStatus.Active);

            if (claimButton != null)
                claimButton.gameObject.SetActive(status == QuestStatus.Completed);
        }

        /// <summary>
        /// On accept button clicked.
        /// </summary>
        private void OnAcceptClicked()
        {
            if (selectedQuest == null || QuestManager.Instance == null) return;

            QuestManager.Instance.AcceptQuest(selectedQuest.questID);
            RefreshQuestList();
            ShowQuestDetails(selectedQuest);
        }

        /// <summary>
        /// On abandon button clicked.
        /// </summary>
        private void OnAbandonClicked()
        {
            if (selectedQuest == null || QuestManager.Instance == null) return;

            QuestManager.Instance.AbandonQuest(selectedQuest.questID);
            RefreshQuestList();
            detailPanel.SetActive(false);
        }

        /// <summary>
        /// On claim button clicked.
        /// </summary>
        private void OnClaimClicked()
        {
            if (selectedQuest == null || QuestManager.Instance == null) return;

            QuestManager.Instance.ClaimReward(selectedQuest.questID);
            RefreshQuestList();
            detailPanel.SetActive(false);
        }

        // Event handlers
        private void OnQuestAccepted(QuestData quest)
        {
            Debug.Log($"[QuestUI] Quest accepted: {quest.questName}");
        }

        private void OnQuestCompleted(QuestData quest)
        {
            Debug.Log($"[QuestUI] Quest completed: {quest.questName}");
        }

        private void OnQuestClaimed(QuestData quest)
        {
            Debug.Log($"[QuestUI] Quest claimed: {quest.questName}");
        }

        private void OnObjectiveUpdated(QuestData quest, QuestObjective objective)
        {
            Debug.Log($"[QuestUI] Objective updated: {objective.description}");
        }

        /// <summary>
        /// Auto-create quest UI.
        /// </summary>
        private void CreateQuestUI()
        {
            // Find Canvas
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[QuestUI] Canvas not found!");
                return;
            }

            // Create Quest Panel
            questPanel = new GameObject("QuestPanel");
            questPanel.transform.SetParent(canvas.transform, false);

            RectTransform panelRect = questPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.1f, 0.1f);
            panelRect.anchorMax = new Vector2(0.9f, 0.9f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelBg = questPanel.AddComponent<Image>();
            panelBg.color = new Color(0, 0, 0, 0.9f);

            // Create tabs
            CreateTabs();

            // Create quest list parent
            questListParent = CreatePanel("QuestList", questPanel.transform,
                new Vector2(0, 0), new Vector2(0.4f, 0.9f),
                new Vector2(10, 10), new Vector2(-5, -40));

            // Add VerticalLayoutGroup
            VerticalLayoutGroup layout = questListParent.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5;
            layout.padding = new RectOffset(5, 5, 5, 5);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Create detail panel
            CreateDetailPanel();

            // Hide panel initially
            questPanel.SetActive(false);
            isPanelActive = false;

            Debug.Log("[QuestUI] Quest UI created!");
        }

        private Transform CreatePanel(string name, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            return panel.transform;
        }

        private void CreateTabs()
        {
            activeTab = CreateTabButton("ActiveTab", "Active", new Vector2(0, 0.9f), new Vector2(0.15f, 1));
            availableTab = CreateTabButton("AvailableTab", "Available", new Vector2(0.15f, 0.9f), new Vector2(0.3f, 1));
            completedTab = CreateTabButton("CompletedTab", "Completed", new Vector2(0.3f, 0.9f), new Vector2(0.45f, 1));
        }

        private Button CreateTabButton(string name, string text, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(questPanel.transform, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(5, 0);
            rect.offsetMax = new Vector2(-5, -5);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f);

            Button button = buttonObj.AddComponent<Button>();

            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = 12;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = Color.white;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return button;
        }

        private void CreateDetailPanel()
        {
            detailPanel = new GameObject("DetailPanel");
            detailPanel.transform.SetParent(questPanel.transform, false);

            RectTransform detailRect = detailPanel.AddComponent<RectTransform>();
            detailRect.anchorMin = new Vector2(0.4f, 0);
            detailRect.anchorMax = new Vector2(1, 0.9f);
            detailRect.offsetMin = new Vector2(5, 10);
            detailRect.offsetMax = new Vector2(-10, -40);

            Image detailBg = detailPanel.AddComponent<Image>();
            detailBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Quest name
            questNameText = CreateTextElement("QuestName", "Quest Name", 18, TextAnchor.UpperLeft, Color.yellow);
            questNameText.rectTransform.SetParent(detailPanel.transform, false);
            questNameText.rectTransform.anchorMin = new Vector2(0, 0.85f);
            questNameText.rectTransform.anchorMax = new Vector2(1, 1);
            questNameText.rectTransform.offsetMin = new Vector2(10, 0);
            questNameText.rectTransform.offsetMax = new Vector2(-10, -5);

            // Quest description
            questDescText = CreateTextElement("QuestDesc", "Description", 14, TextAnchor.UpperLeft, Color.white);
            questDescText.rectTransform.SetParent(detailPanel.transform, false);
            questDescText.rectTransform.anchorMin = new Vector2(0, 0.6f);
            questDescText.rectTransform.anchorMax = new Vector2(1, 0.85f);
            questDescText.rectTransform.offsetMin = new Vector2(10, 0);
            questDescText.rectTransform.offsetMax = new Vector2(-10, 0);

            // Objectives
            objectivesText = CreateTextElement("Objectives", "Objectives", 14, TextAnchor.UpperLeft, Color.white);
            objectivesText.rectTransform.SetParent(detailPanel.transform, false);
            objectivesText.rectTransform.anchorMin = new Vector2(0, 0.3f);
            objectivesText.rectTransform.anchorMax = new Vector2(1, 0.6f);
            objectivesText.rectTransform.offsetMin = new Vector2(10, 0);
            objectivesText.rectTransform.offsetMax = new Vector2(-10, 0);

            // Rewards
            rewardsText = CreateTextElement("Rewards", "Rewards", 14, TextAnchor.UpperLeft, Color.green);
            rewardsText.rectTransform.SetParent(detailPanel.transform, false);
            rewardsText.rectTransform.anchorMin = new Vector2(0, 0.1f);
            rewardsText.rectTransform.anchorMax = new Vector2(1, 0.3f);
            rewardsText.rectTransform.offsetMin = new Vector2(10, 0);
            rewardsText.rectTransform.offsetMax = new Vector2(-10, 0);

            // Buttons
            acceptButton = CreateButton("AcceptButton", "Accept", new Vector2(0.6f, 0), new Vector2(0.8f, 0.1f));
            abandonButton = CreateButton("AbandonButton", "Abandon", new Vector2(0.8f, 0), new Vector2(1, 0.1f));
            claimButton = CreateButton("ClaimButton", "Claim", new Vector2(0.6f, 0), new Vector2(1, 0.1f));
        }

        private Text CreateTextElement(string name, string text, int fontSize, TextAnchor alignment, Color color)
        {
            GameObject textObj = new GameObject(name);
            Text textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = fontSize;
            textComp.alignment = alignment;
            textComp.color = color;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return textComp;
        }

        private Button CreateButton(string name, string text, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(detailPanel.transform, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(5, 5);
            rect.offsetMax = new Vector2(-5, -5);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.3f, 0.5f, 0.3f);

            Button button = buttonObj.AddComponent<Button>();

            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = 14;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = Color.white;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return button;
        }
    }
}
