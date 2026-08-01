using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArcadiaOnline.Save
{
    /// <summary>
    /// UI untuk save/load system.
    /// </summary>
    public class SaveUI : MonoBehaviour
    {
        public static SaveUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject savePanel;

        [Header("Auto-Create UI")]
        [SerializeField] private bool autoCreateUI = true;

        [Header("Settings")]
        [SerializeField] private KeyCode toggleKey = KeyCode.F6;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        // Internal references
        private Transform slotListParent;
        private Text titleText;
        private Button closeButton;
        private List<SaveSlotUI> slotUIs = new List<SaveSlotUI>();

        // State
        private bool isOpen = false;
        private SaveMode currentMode = SaveMode.Save;

        public enum SaveMode
        {
            Save,
            Load
        }

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
            if (autoCreateUI && savePanel == null)
            {
                CreateSaveUI();
            }

            // Setup buttons
            SetupButtons();

            // Hide panel
            HidePanel();

            if (showDebug)
            {
                Debug.Log("[SaveUI] Initialized");
            }
        }

        void Update()
        {
            // Toggle save/load UI
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleSaveUI();
            }

            // Close dengan ESC
            if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            {
                HidePanel();
            }
        }

        /// <summary>
        /// Setup button listeners.
        /// </summary>
        private void SetupButtons()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HidePanel);
            }
        }

        /// <summary>
        /// Toggle save/load UI.
        /// </summary>
        public void ToggleSaveUI()
        {
            if (isOpen)
            {
                HidePanel();
            }
            else
            {
                ShowSaveUI();
            }
        }

        /// <summary>
        /// Show save UI.
        /// </summary>
        public void ShowSaveUI()
        {
            currentMode = SaveMode.Save;
            ShowPanel();
            RefreshSlots();
        }

        /// <summary>
        /// Show load UI.
        /// </summary>
        public void ShowLoadUI()
        {
            currentMode = SaveMode.Load;
            ShowPanel();
            RefreshSlots();
        }

        /// <summary>
        /// Show panel.
        /// </summary>
        private void ShowPanel()
        {
            if (savePanel != null)
            {
                savePanel.SetActive(true);
                isOpen = true;

                // Update title
                if (titleText != null)
                {
                    titleText.text = currentMode == SaveMode.Save ? "SAVE GAME" : "LOAD GAME";
                }
            }
        }

        /// <summary>
        /// Hide panel.
        /// </summary>
        private void HidePanel()
        {
            if (savePanel != null)
            {
                savePanel.SetActive(false);
                isOpen = false;
            }
        }

        /// <summary>
        /// Refresh save slots.
        /// </summary>
        private void RefreshSlots()
        {
            if (SaveManager.Instance == null) return;

            // Clear existing slots
            foreach (SaveSlotUI slotUI in slotUIs)
            {
                if (slotUI != null)
                {
                    Destroy(slotUI.gameObject);
                }
            }
            slotUIs.Clear();

            // Get save slots info
            List<SaveSlotInfo> slots = SaveManager.Instance.GetAllSaveSlots();

            // Create slot UIs
            foreach (SaveSlotInfo slotInfo in slots)
            {
                CreateSlotUI(slotInfo);
            }
        }

        /// <summary>
        /// Create slot UI.
        /// </summary>
        private void CreateSlotUI(SaveSlotInfo slotInfo)
        {
            if (slotListParent == null) return;

            // Create slot object
            GameObject slotObj = new GameObject($"SaveSlot_{slotInfo.slotIndex}");
            slotObj.transform.SetParent(slotListParent, false);

            RectTransform rect = slotObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 100);

            LayoutElement layoutElement = slotObj.AddComponent<LayoutElement>();
            layoutElement.minHeight = 100;
            layoutElement.preferredHeight = 100;

            Image bg = slotObj.AddComponent<Image>();
            bg.color = slotInfo.exists ? new Color(0.2f, 0.3f, 0.2f, 0.9f) : new Color(0.2f, 0.2f, 0.2f, 0.9f);

            Button button = slotObj.AddComponent<Button>();

            // Slot number
            Text slotNumber = CreateTextElement("SlotNumber", $"Slot {slotInfo.slotIndex + 1}", 16, TextAnchor.UpperLeft, Color.yellow);
            slotNumber.rectTransform.SetParent(slotObj.transform, false);
            slotNumber.rectTransform.anchorMin = new Vector2(0, 0.7f);
            slotNumber.rectTransform.anchorMax = new Vector2(0.3f, 1);
            slotNumber.rectTransform.offsetMin = new Vector2(10, 0);
            slotNumber.rectTransform.offsetMax = Vector2.zero;

            if (slotInfo.exists)
            {
                // Player name
                Text playerName = CreateTextElement("PlayerName", slotInfo.playerName, 14, TextAnchor.UpperLeft, Color.white);
                playerName.rectTransform.SetParent(slotObj.transform, false);
                playerName.rectTransform.anchorMin = new Vector2(0, 0.4f);
                playerName.rectTransform.anchorMax = new Vector2(0.5f, 0.7f);
                playerName.rectTransform.offsetMin = new Vector2(10, 0);
                playerName.rectTransform.offsetMax = Vector2.zero;

                // Level
                Text levelText = CreateTextElement("Level", $"Lv.{slotInfo.playerLevel}", 14, TextAnchor.UpperLeft, Color.white);
                levelText.rectTransform.SetParent(slotObj.transform, false);
                levelText.rectTransform.anchorMin = new Vector2(0.5f, 0.4f);
                levelText.rectTransform.anchorMax = new Vector2(1, 0.7f);
                levelText.rectTransform.offsetMin = new Vector2(10, 0);
                levelText.rectTransform.offsetMax = Vector2.zero;

                // Map
                Text mapText = CreateTextElement("Map", slotInfo.currentMap, 12, TextAnchor.MiddleLeft, new Color(0.7f, 0.7f, 0.7f));
                mapText.rectTransform.SetParent(slotObj.transform, false);
                mapText.rectTransform.anchorMin = new Vector2(0, 0.2f);
                mapText.rectTransform.anchorMax = new Vector2(0.5f, 0.4f);
                mapText.rectTransform.offsetMin = new Vector2(10, 0);
                mapText.rectTransform.offsetMax = Vector2.zero;

                // Save date
                Text dateText = CreateTextElement("Date", slotInfo.saveDate, 12, TextAnchor.MiddleRight, new Color(0.7f, 0.7f, 0.7f));
                dateText.rectTransform.SetParent(slotObj.transform, false);
                dateText.rectTransform.anchorMin = new Vector2(0.5f, 0.2f);
                dateText.rectTransform.anchorMax = new Vector2(1, 0.4f);
                dateText.rectTransform.offsetMin = Vector2.zero;
                dateText.rectTransform.offsetMax = new Vector2(-10, 0);

                // Play time
                Text timeText = CreateTextElement("Time", slotInfo.playTime, 12, TextAnchor.LowerRight, new Color(0.7f, 0.7f, 0.7f));
                timeText.rectTransform.SetParent(slotObj.transform, false);
                timeText.rectTransform.anchorMin = new Vector2(0.5f, 0);
                timeText.rectTransform.anchorMax = new Vector2(1, 0.2f);
                timeText.rectTransform.offsetMin = Vector2.zero;
                timeText.rectTransform.offsetMax = new Vector2(-10, 5);
            }
            else
            {
                // Empty slot text
                Text emptyText = CreateTextElement("Empty", "Empty Slot", 16, TextAnchor.MiddleCenter, new Color(0.5f, 0.5f, 0.5f));
                emptyText.rectTransform.SetParent(slotObj.transform, false);
                emptyText.rectTransform.anchorMin = Vector2.zero;
                emptyText.rectTransform.anchorMax = Vector2.one;
                emptyText.rectTransform.offsetMin = Vector2.zero;
                emptyText.rectTransform.offsetMax = Vector2.zero;
            }

            // Add click listener
            int slotIndex = slotInfo.slotIndex;
            button.onClick.AddListener(() => OnSlotClicked(slotIndex));

            // Add to list
            SaveSlotUI slotUI = slotObj.AddComponent<SaveSlotUI>();
            slotUI.Initialize(slotInfo);
            slotUIs.Add(slotUI);
        }

        /// <summary>
        /// On slot clicked.
        /// </summary>
        private void OnSlotClicked(int slotIndex)
        {
            if (SaveManager.Instance == null) return;

            if (currentMode == SaveMode.Save)
            {
                // Save game
                if (SaveManager.Instance.SaveGame(slotIndex))
                {
                    Debug.Log($"[SaveUI] Game saved to slot {slotIndex}");
                    RefreshSlots();
                }
            }
            else
            {
                // Load game
                if (SaveManager.Instance.LoadGame(slotIndex))
                {
                    Debug.Log($"[SaveUI] Game loaded from slot {slotIndex}");
                    HidePanel();
                }
            }
        }

        /// <summary>
        /// Create save UI.
        /// </summary>
        private void CreateSaveUI()
        {
            // Find Canvas
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[SaveUI] Canvas not found!");
                return;
            }

            // Create Save Panel
            savePanel = new GameObject("SavePanel");
            savePanel.transform.SetParent(canvas.transform, false);

            RectTransform panelRect = savePanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.2f, 0.15f);
            panelRect.anchorMax = new Vector2(0.8f, 0.85f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelBg = savePanel.AddComponent<Image>();
            panelBg.color = new Color(0, 0, 0, 0.9f);

            // Title
            titleText = CreateTextElement("Title", "SAVE GAME", 24, TextAnchor.MiddleCenter, Color.yellow);
            titleText.rectTransform.SetParent(savePanel.transform, false);
            titleText.rectTransform.anchorMin = new Vector2(0, 0.9f);
            titleText.rectTransform.anchorMax = new Vector2(1, 1);
            titleText.rectTransform.offsetMin = new Vector2(10, 0);
            titleText.rectTransform.offsetMax = new Vector2(-10, -5);

            // Slot list parent
            slotListParent = CreatePanel("SlotList", savePanel.transform,
                new Vector2(0, 0.1f), new Vector2(1, 0.9f),
                new Vector2(10, 10), new Vector2(-10, -10));

            VerticalLayoutGroup layout = slotListParent.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Close button
            closeButton = CreateButton("CloseButton", "X", new Vector2(0.9f, 0.9f), new Vector2(1, 1));

            // Hide panel
            savePanel.SetActive(false);

            if (showDebug)
            {
                Debug.Log("[SaveUI] Save UI created!");
            }
        }

        // Helper methods
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
            buttonObj.transform.SetParent(savePanel.transform, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(5, 5);
            rect.offsetMax = new Vector2(-5, -5);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.5f, 0.2f, 0.2f);

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
            textComp.fontSize = 16;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = Color.white;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return button;
        }
    }
}
