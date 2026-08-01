using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// Auto-create Level Up UI saat game start.
    /// Tidak perlu setup manual di Unity Editor.
    /// </summary>
    public class LevelUpUICreator : MonoBehaviour
    {
        void Awake()
        {
            CreateLevelUpUI();
            Destroy(this.gameObject); // Hapus creator setelah UI dibuat
        }

        /// <summary>
        /// Buat semua UI elements secara programmatic.
        /// </summary>
        private void CreateLevelUpUI()
        {
            // Cari atau buat Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // === LEVEL UP PANEL (Container) ===
            GameObject levelUpPanel = CreatePanel(canvas.transform, "LevelUpPanel",
                new Vector2(10, -10), new Vector2(200, 120), new Color(0, 0, 0, 0.7f));

            // === LEVEL TEXT ===
            GameObject levelText = CreateText(levelUpPanel.transform, "LevelText",
                "Lv. 1", 18, FontStyle.Bold, Color.white,
                new Vector2(0, 0), new Vector2(200, 25), TextAnchor.MiddleCenter);

            // Set anchor ke Top-Stretch
            RectTransform levelTextRect = levelText.GetComponent<RectTransform>();
            SetAnchor(levelTextRect, AnchorPreset.TopStretch);

            // === HP BAR ===
            GameObject hpBarBG = CreatePanel(levelUpPanel.transform, "HPBarBG",
                new Vector2(10, -30), new Vector2(180, 20), new Color(0.2f, 0.2f, 0.2f, 1f));
            SetAnchorTopLeft(hpBarBG.GetComponent<RectTransform>());

            GameObject hpBar = CreateFilledBar(hpBarBG.transform, "HPBar",
                new Color(0.8f, 0.1f, 0.1f, 1f)); // Merah

            GameObject hpText = CreateText(levelUpPanel.transform, "HPText",
                "100/100", 12, FontStyle.Normal, Color.white,
                new Vector2(10, -30), new Vector2(180, 20), TextAnchor.MiddleCenter);
            SetAnchorTopLeft(hpText.GetComponent<RectTransform>());

            // === MP BAR ===
            GameObject mpBarBG = CreatePanel(levelUpPanel.transform, "MPBarBG",
                new Vector2(10, -55), new Vector2(180, 20), new Color(0.2f, 0.2f, 0.2f, 1f));
            SetAnchorTopLeft(mpBarBG.GetComponent<RectTransform>());

            GameObject mpBar = CreateFilledBar(mpBarBG.transform, "MPBar",
                new Color(0.1f, 0.3f, 0.8f, 1f)); // Biru

            GameObject mpText = CreateText(levelUpPanel.transform, "MPText",
                "50/50", 12, FontStyle.Normal, Color.white,
                new Vector2(10, -55), new Vector2(180, 20), TextAnchor.MiddleCenter);
            SetAnchorTopLeft(mpText.GetComponent<RectTransform>());

            // === EXP BAR ===
            GameObject expBarBG = CreatePanel(levelUpPanel.transform, "EXPBarBG",
                new Vector2(10, -80), new Vector2(180, 15), new Color(0.2f, 0.2f, 0.2f, 1f));
            SetAnchorTopLeft(expBarBG.GetComponent<RectTransform>());

            GameObject expBar = CreateFilledBar(expBarBG.transform, "EXPBar",
                new Color(0.2f, 0.8f, 0.2f, 1f)); // Hijau

            GameObject expText = CreateText(levelUpPanel.transform, "EXPText",
                "0/100", 10, FontStyle.Normal, Color.white,
                new Vector2(10, -80), new Vector2(180, 15), TextAnchor.MiddleCenter);
            SetAnchorTopLeft(expText.GetComponent<RectTransform>());

            // === LEVEL UP NOTIFICATION ===
            GameObject notification = CreatePanel(canvas.transform, "LevelUpNotification",
                new Vector2(0, 100), new Vector2(300, 80), new Color(1f, 0.8f, 0f, 0.9f));
            SetAnchorCenter(notification.GetComponent<RectTransform>());
            notification.SetActive(false); // Hidden by default

            GameObject notificationText = CreateText(notification.transform, "LevelUpText",
                "LEVEL UP!", 24, FontStyle.Bold, Color.black,
                Vector2.zero, new Vector2(300, 80), TextAnchor.MiddleCenter);
            SetStretch(notificationText.GetComponent<RectTransform>());

            // === SETUP LEVEL UP UI COMPONENT ===
            LevelUpUI levelUpUI = canvas.gameObject.AddComponent<LevelUpUI>();

            // Assign references via reflection (karena field private)
            SetField(levelUpUI, "levelText", levelText.GetComponent<Text>());
            SetField(levelUpUI, "expBar", expBar.GetComponent<Image>());
            SetField(levelUpUI, "hpBar", hpBar.GetComponent<Image>());
            SetField(levelUpUI, "mpBar", mpBar.GetComponent<Image>());
            SetField(levelUpUI, "expText", expText.GetComponent<Text>());
            SetField(levelUpUI, "hpText", hpText.GetComponent<Text>());
            SetField(levelUpUI, "mpText", mpText.GetComponent<Text>());
            SetField(levelUpUI, "levelUpPanel", notification);
            SetField(levelUpUI, "levelUpText", notificationText.GetComponent<Text>());

            Debug.Log("[LevelUpUI] UI Created Successfully!");
        }

        // === HELPER METHODS ===

        private GameObject CreatePanel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = panel.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            return panel;
        }

        private GameObject CreateText(Transform parent, string name, string text, int fontSize,
            FontStyle fontStyle, Color color, Vector2 position, Vector2 size, TextAnchor alignment)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = fontStyle;
            textComponent.color = color;
            textComponent.alignment = alignment;
            textComponent.font = Font.CreateDynamicFontFromOSFont("Arial", fontSize);
            textComponent.raycastTarget = false;

            return textObj;
        }

        private GameObject CreateFilledBar(Transform parent, string name, Color color)
        {
            GameObject bar = new GameObject(name);
            bar.transform.SetParent(parent, false);

            RectTransform rect = bar.AddComponent<RectTransform>();
            SetStretch(rect); // Fill parent

            Image image = bar.AddComponent<Image>();
            image.color = color;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = 0; // Left
            image.fillAmount = 1f;

            return bar;
        }

        // === ANCHOR HELPERS ===

        private enum AnchorPreset
        {
            TopLeft, TopCenter, TopRight,
            MiddleLeft, MiddleCenter, MiddleRight,
            BottomLeft, BottomCenter, BottomRight,
            TopStretch, MiddleStretch, BottomStretch,
            StretchLeft, StretchRight, StretchCenter,
            StretchAll
        }

        private void SetAnchor(RectTransform rect, AnchorPreset preset)
        {
            switch (preset)
            {
                case AnchorPreset.TopStretch:
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = new Vector2(1, 1);
                    break;
            }
        }

        private void SetAnchorTopLeft(RectTransform rect)
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
        }

        private void SetAnchorCenter(RectTransform rect)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        private void SetStretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void SetField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}
