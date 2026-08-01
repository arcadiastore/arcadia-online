using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// Auto-create Level Up UI dengan layout yang benar.
    /// </summary>
    public class LevelUpUICreator : MonoBehaviour
    {
        void Awake()
        {
            CreateLevelUpUI();
            Destroy(this.gameObject);
        }

        private void CreateLevelUpUI()
        {
            // Cari Canvas yang sudah ada
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // === MAIN PANEL (Kiri Atas) ===
            GameObject mainPanel = new GameObject("LevelUpPanel");
            mainPanel.transform.SetParent(canvas.transform, false);

            RectTransform mainRect = mainPanel.AddComponent<RectTransform>();
            // Anchor ke kiri atas
            mainRect.anchorMin = new Vector2(0, 1);
            mainRect.anchorMax = new Vector2(0, 1);
            mainRect.pivot = new Vector2(0, 1);
            mainRect.anchoredPosition = new Vector2(10, -10);
            mainRect.sizeDelta = new Vector2(220, 100);

            Image mainImage = mainPanel.AddComponent<Image>();
            mainImage.color = new Color(0, 0, 0, 0.6f);

            // === LEVEL TEXT ===
            GameObject levelTextObj = CreateTextElement(mainPanel.transform, "LevelText",
                "Lv. 1", 20, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            RectTransform levelRect = levelTextObj.GetComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0, 1);
            levelRect.anchorMax = new Vector2(1, 1);
            levelRect.pivot = new Vector2(0.5f, 1);
            levelRect.anchoredPosition = new Vector2(0, -5);
            levelRect.sizeDelta = new Vector2(0, 25);

            // === HP BAR ROW ===
            CreateBarRow(mainPanel.transform, "HP", new Color(0.8f, 0.1f, 0.1f), -35);

            // === MP BAR ROW ===
            CreateBarRow(mainPanel.transform, "MP", new Color(0.1f, 0.3f, 0.8f), -60);

            // === EXP BAR ROW ===
            CreateBarRow(mainPanel.transform, "EXP", new Color(0.2f, 0.8f, 0.2f), -85, true);

            // === LEVEL UP NOTIFICATION (Tengah) ===
            GameObject notif = new GameObject("LevelUpNotification");
            notif.transform.SetParent(canvas.transform, false);

            RectTransform notifRect = notif.AddComponent<RectTransform>();
            notifRect.anchorMin = new Vector2(0.5f, 0.5f);
            notifRect.anchorMax = new Vector2(0.5f, 0.5f);
            notifRect.pivot = new Vector2(0.5f, 0.5f);
            notifRect.anchoredPosition = new Vector2(0, 100);
            notifRect.sizeDelta = new Vector2(300, 60);

            Image notifImage = notif.AddComponent<Image>();
            notifImage.color = new Color(1f, 0.8f, 0f, 0.9f);

            // Text di notifikasi
            GameObject notifText = CreateTextElement(notif.transform, "LevelUpText",
                "LEVEL UP!", 28, FontStyle.Bold, Color.black, TextAnchor.MiddleCenter);
            RectTransform notifTextRect = notifText.GetComponent<RectTransform>();
            notifTextRect.anchorMin = Vector2.zero;
            notifTextRect.anchorMax = Vector2.one;
            notifTextRect.offsetMin = Vector2.zero;
            notifTextRect.offsetMax = Vector2.zero;

            notif.SetActive(false);

            // === SETUP COMPONENT ===
            SetupLevelUpUI(canvas.gameObject, mainPanel, notif);
        }

        /// <summary>
        /// Buat satu bar row (Label + Background + Fill + Text).
        /// </summary>
        private void CreateBarRow(Transform parent, string label, Color barColor, float yOffset, bool isEXP = false)
        {
            // Label (HP:, MP:, EXP:)
            GameObject labelObj = CreateTextElement(parent, label + "Label",
                label + ":", 12, FontStyle.Bold, Color.white, TextAnchor.MiddleLeft);
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 1);
            labelRect.anchorMax = new Vector2(0, 1);
            labelRect.pivot = new Vector2(0, 1);
            labelRect.anchoredPosition = new Vector2(5, yOffset);
            labelRect.sizeDelta = new Vector2(35, 15);

            // Bar Background
            float barWidth = isEXP ? 150f : 160f;
            float barHeight = isEXP ? 12f : 18f;

            GameObject barBG = new GameObject(label + "BarBG");
            barBG.transform.SetParent(parent, false);

            RectTransform bgRect = barBG.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 1);
            bgRect.anchorMax = new Vector2(0, 1);
            bgRect.pivot = new Vector2(0, 1);
            bgRect.anchoredPosition = new Vector2(40, yOffset);
            bgRect.sizeDelta = new Vector2(barWidth, barHeight);

            Image bgImage = barBG.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);

            // Bar Fill
            GameObject barFill = new GameObject(label + "Bar");
            barFill.transform.SetParent(barBG.transform, false);

            RectTransform fillRect = barFill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            Image fillImage = barFill.AddComponent<Image>();
            fillImage.color = barColor;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0;
            fillImage.fillAmount = 1f;

            // Text (100/100)
            GameObject textObj = CreateTextElement(barBG.transform, label + "Text",
                "100/100", 10, FontStyle.Normal, Color.white, TextAnchor.MiddleCenter);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// Buat text element.
        /// </summary>
        private GameObject CreateTextElement(Transform parent, string name, string text,
            int fontSize, FontStyle style, Color color, TextAnchor alignment)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            obj.AddComponent<RectTransform>();

            Text textComp = obj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = fontSize;
            textComp.fontStyle = style;
            textComp.color = color;
            textComp.alignment = alignment;
            textComp.font = Font.CreateDynamicFontFromOSFont("Arial", fontSize);
            textComp.raycastTarget = false;

            return obj;
        }

        /// <summary>
        /// Setup LevelUpUI component dengan references.
        /// </summary>
        private void SetupLevelUpUI(GameObject canvasObj, GameObject mainPanel, GameObject notif)
        {
            LevelUpUI ui = canvasObj.AddComponent<LevelUpUI>();

            // Cari elements
            Text levelText = mainPanel.transform.Find("LevelText")?.GetComponent<Text>();
            Image hpBar = mainPanel.transform.Find("HPBarBG/HPBar")?.GetComponent<Image>();
            Image mpBar = mainPanel.transform.Find("MPBarBG/MPBar")?.GetComponent<Image>();
            Image expBar = mainPanel.transform.Find("EXPBarBG/EXPBar")?.GetComponent<Image>();
            Text hpText = mainPanel.transform.Find("HPBarBG/HPText")?.GetComponent<Text>();
            Text mpText = mainPanel.transform.Find("MPBarBG/MPText")?.GetComponent<Text>();
            Text expText = mainPanel.transform.Find("EXPBarBG/EXPText")?.GetComponent<Text>();
            Text notifText = notif.transform.Find("LevelUpText")?.GetComponent<Text>();

            // Assign via reflection
            SetField(ui, "levelText", levelText);
            SetField(ui, "hpBar", hpBar);
            SetField(ui, "mpBar", mpBar);
            SetField(ui, "expBar", expBar);
            SetField(ui, "hpText", hpText);
            SetField(ui, "mpText", mpText);
            SetField(ui, "expText", expText);
            SetField(ui, "levelUpPanel", notif);
            SetField(ui, "levelUpText", notifText);
        }

        private void SetField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (field != null && value != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}
