using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.Dialogue
{
    /// <summary>
    /// UI untuk dialogue system.
    /// Auto-create UI jika tidak ada.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        public static DialogueUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private Text speakerNameText;
        [SerializeField] private Text dialogueText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private GameObject portraitPanel;
        [SerializeField] private Button continueButton;
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private Transform choiceButtonParent;

        [Header("Auto-Create UI")]
        [SerializeField] private bool autoCreateUI = true;

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
            if (autoCreateUI && dialoguePanel == null)
            {
                CreateDialogueUI();
            }
        }

        /// <summary>
        /// Auto-create dialogue UI.
        /// </summary>
        private void CreateDialogueUI()
        {
            // Find Canvas
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[DialogueUI] Canvas not found!");
                return;
            }

            // Create Dialogue Panel
            dialoguePanel = CreatePanel(canvas.transform, "DialoguePanel",
                new Vector2(0, 0), new Vector2(1, 0.3f), new Vector2(0, 0));

            // Background
            Image bg = dialoguePanel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);

            // Create Portrait Panel
            portraitPanel = CreatePanel(dialoguePanel.transform, "PortraitPanel",
                new Vector2(0, 0), new Vector2(0.1f, 1), new Vector2(0, 0));
            Image portraitBg = portraitPanel.AddComponent<Image>();
            portraitBg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            // Portrait Image
            GameObject portraitObj = CreateImage(portraitPanel.transform, "Portrait",
                Color.white);
            portraitImage = portraitObj.GetComponent<Image>();

            // Create Text Panel
            GameObject textPanel = CreatePanel(dialoguePanel.transform, "TextPanel",
                new Vector2(0.1f, 0), new Vector2(1, 1), new Vector2(0, 0));

            // Speaker Name Text
            GameObject nameObj = CreateText(textPanel.transform, "SpeakerName",
                "NPC Name", 18, TextAnchor.UpperLeft, Color.yellow);
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.7f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-10, -5);
            speakerNameText = nameObj.GetComponent<Text>();

            // Dialogue Text
            GameObject dialogueObj = CreateText(textPanel.transform, "DialogueText",
                "Dialogue text goes here...", 16, TextAnchor.UpperLeft, Color.white);
            RectTransform dialogueRect = dialogueObj.GetComponent<RectTransform>();
            dialogueRect.anchorMin = new Vector2(0, 0);
            dialogueRect.anchorMax = new Vector2(1, 0.7f);
            dialogueRect.offsetMin = new Vector2(10, 10);
            dialogueRect.offsetMax = new Vector2(-10, 0);
            dialogueText = dialogueObj.GetComponent<Text>();

            // Continue Button
            GameObject continueObj = CreateButton(dialoguePanel.transform, "ContinueButton",
                "Continue", new Vector2(0.9f, 0.1f), new Vector2(1, 0.3f));
            continueButton = continueObj.GetComponent<Button>();

            // Choice Panel
            choicePanel = CreatePanel(dialoguePanel.transform, "ChoicePanel",
                new Vector2(0.3f, 0.3f), new Vector2(0.7f, 0.9f), new Vector2(0.5f, 0.5f));
            Image choiceBg = choicePanel.AddComponent<Image>();
            choiceBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            choicePanel.SetActive(false);

            // Choice Button Parent
            choiceButtonParent = choicePanel.transform;

            // Setup DialogueManager references
            SetupDialogueManager();

            Debug.Log("[DialogueUI] Dialogue UI created!");
        }

        private void SetupDialogueManager()
        {
            DialogueManager manager = FindAnyObjectByType<DialogueManager>();
            if (manager != null)
            {
                // Set references via reflection or public setter
                Debug.Log("[DialogueUI] DialogueManager found, setup references.");
            }
        }

        // Helper methods
        private GameObject CreatePanel(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return panel;
        }

        private GameObject CreateText(Transform parent, string name, string text,
            int fontSize, TextAnchor alignment, Color color)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Text textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = fontSize;
            textComp.alignment = alignment;
            textComp.color = color;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return textObj;
        }

        private GameObject CreateImage(Transform parent, string name, Color color)
        {
            GameObject imageObj = new GameObject(name);
            imageObj.transform.SetParent(parent, false);

            RectTransform rect = imageObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(5, 5);
            rect.offsetMax = new Vector2(-5, -5);

            Image image = imageObj.AddComponent<Image>();
            image.color = color;

            return imageObj;
        }

        private GameObject CreateButton(Transform parent, string name, string text,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(10, 5);
            rect.offsetMax = new Vector2(-10, -5);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            Button button = buttonObj.AddComponent<Button>();

            GameObject textObj = CreateText(buttonObj.transform, "Text", text,
                14, TextAnchor.MiddleCenter, Color.white);

            return buttonObj;
        }
    }
}
