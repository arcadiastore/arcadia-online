using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// Auto-create Skill UI saat game start.
    /// </summary>
    public class SkillUICreator : MonoBehaviour
    {
        void Awake()
        {
            CreateSkillUI();
            Destroy(this.gameObject);
        }

        private void CreateSkillUI()
        {
            // Cari Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // === SKILL BAR (Bottom Center) ===
            GameObject skillBar = new GameObject("SkillBar");
            skillBar.transform.SetParent(canvas.transform, false);

            RectTransform skillBarRect = skillBar.AddComponent<RectTransform>();
            skillBarRect.anchorMin = new Vector2(0.5f, 0);
            skillBarRect.anchorMax = new Vector2(0.5f, 0);
            skillBarRect.pivot = new Vector2(0.5f, 0);
            skillBarRect.anchoredPosition = new Vector2(0, 10);
            skillBarRect.sizeDelta = new Vector2(280, 70);

            Image skillBarBg = skillBar.AddComponent<Image>();
            skillBarBg.color = new Color(0, 0, 0, 0.5f);

            // Horizontal Layout
            HorizontalLayoutGroup layout = skillBar.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.padding = new RectOffset(5, 5, 5, 5);

            // Content Size Fitter
            ContentSizeFitter fitter = skillBar.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            // === SETUP SKILL UI COMPONENT ===
            SkillUI skillUI = canvas.gameObject.AddComponent<SkillUI>();

            // Assign references
            SetField(skillUI, "skillSlotParent", skillBar.transform);

            // Add SkillSystem to player if not exists
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                player = GameObject.Find("Player");
            }

            if (player != null)
            {
                Combat.SkillSystem skillSystem = player.GetComponent<Combat.SkillSystem>();
                if (skillSystem == null)
                {
                    skillSystem = player.AddComponent<Combat.SkillSystem>();
                }
                SetField(skillUI, "skillSystem", skillSystem);
            }

            Debug.Log("[SkillUI] Skill Bar Created!");
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
