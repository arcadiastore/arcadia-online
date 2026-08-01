using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArcadiaOnline.UI
{
    /// <summary>
    /// UI untuk Skill System.
    /// Menampilkan skill slots dengan cooldown.
    /// </summary>
    public class SkillUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Combat.SkillSystem skillSystem;

        [Header("UI Elements")]
        [SerializeField] private Transform skillSlotParent;
        [SerializeField] private GameObject skillSlotPrefab;

        [Header("Skill Info")]
        [SerializeField] private Text skillNameText;
        [SerializeField] private Text skillDescText;
        [SerializeField] private Text skillCostText;

        private List<SkillSlotUI> skillSlots = new List<SkillSlotUI>();

        void Start()
        {
            if (skillSystem == null)
            {
                skillSystem = FindFirstObjectByType<Combat.SkillSystem>();
            }

            CreateSkillSlots();
        }

        void Update()
        {
            UpdateCooldowns();
        }

        /// <summary>
        /// Create skill slots UI.
        /// </summary>
        private void CreateSkillSlots()
        {
            if (skillSystem == null || skillSlotParent == null) return;

            // Clear existing slots
            foreach (Transform child in skillSlotParent)
            {
                Destroy(child.gameObject);
            }
            skillSlots.Clear();

            // Create slots for each equipped skill
            List<Combat.SkillData> skills = skillSystem.GetEquippedSkills();

            for (int i = 0; i < skills.Count; i++)
            {
                GameObject slotObj = CreateSkillSlot(i, skills[i]);
                SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
                skillSlots.Add(slotUI);
            }
        }

        /// <summary>
        /// Create single skill slot.
        /// </summary>
        private GameObject CreateSkillSlot(int index, Combat.SkillData skill)
        {
            GameObject slot = new GameObject($"SkillSlot_{index}");
            slot.transform.SetParent(skillSlotParent, false);

            // Background
            RectTransform rect = slot.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(60, 60);

            Image bg = slot.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // Skill icon (colored square based on type)
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(slot.transform, false);

            RectTransform iconRect = icon.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(5, 5);
            iconRect.offsetMax = new Vector2(-5, -5);

            Image iconImage = icon.AddComponent<Image>();
            iconImage.color = GetSkillColor(skill.skillType);

            // Cooldown overlay
            GameObject cooldown = new GameObject("Cooldown");
            cooldown.transform.SetParent(slot.transform, false);

            RectTransform cooldownRect = cooldown.AddComponent<RectTransform>();
            cooldownRect.anchorMin = Vector2.zero;
            cooldownRect.anchorMax = Vector2.one;
            cooldownRect.offsetMin = Vector2.zero;
            cooldownRect.offsetMax = Vector2.zero;

            Image cooldownImage = cooldown.AddComponent<Image>();
            cooldownImage.color = new Color(0, 0, 0, 0.6f);
            cooldownImage.type = Image.Type.Filled;
            cooldownImage.fillMethod = Image.FillMethod.Radial360;
            cooldownImage.fillAmount = 0;

            // Key text
            GameObject keyText = new GameObject("KeyText");
            keyText.transform.SetParent(slot.transform, false);

            RectTransform keyRect = keyText.AddComponent<RectTransform>();
            keyRect.anchorMin = Vector2.zero;
            keyRect.anchorMax = Vector2.one;
            keyRect.offsetMin = Vector2.zero;
            keyRect.offsetMax = Vector2.zero;

            Text key = keyText.AddComponent<Text>();
            key.text = (index + 1).ToString();
            key.fontSize = 14;
            key.fontStyle = FontStyle.Bold;
            key.color = Color.white;
            key.alignment = TextAnchor.MiddleCenter;
            key.font = Font.CreateDynamicFontFromOSFont("Arial", 14);

            // Skill name text
            GameObject nameText = new GameObject("NameText");
            nameText.transform.SetParent(slot.transform, false);

            RectTransform nameRect = nameText.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 0);
            nameRect.pivot = new Vector2(0.5f, 0);
            nameRect.anchoredPosition = new Vector2(0, -5);
            nameRect.sizeDelta = new Vector2(0, 15);

            Text name = nameText.AddComponent<Text>();
            name.text = skill.skillName;
            name.fontSize = 8;
            name.color = Color.white;
            name.alignment = TextAnchor.MiddleCenter;
            name.font = Font.CreateDynamicFontFromOSFont("Arial", 8);

            // Add SkillSlotUI component
            SkillSlotUI slotUI = slot.AddComponent<SkillSlotUI>();
            slotUI.Initialize(index, skill, cooldownImage);

            // Add button for click
            Button button = slot.AddComponent<Button>();
            button.onClick.AddListener(() => OnSkillClicked(index));

            return slot;
        }

        /// <summary>
        /// Get color berdasarkan skill type.
        /// </summary>
        private Color GetSkillColor(Combat.SkillType type)
        {
            switch (type)
            {
                case Combat.SkillType.Physical:
                    return new Color(0.8f, 0.2f, 0.2f); // Merah
                case Combat.SkillType.Magical:
                    return new Color(0.2f, 0.2f, 0.8f); // Biru
                case Combat.SkillType.Heal:
                    return new Color(0.2f, 0.8f, 0.2f); // Hijau
                case Combat.SkillType.Buff:
                    return new Color(0.8f, 0.8f, 0.2f); // Kuning
                default:
                    return Color.gray;
            }
        }

        /// <summary>
        /// Update cooldown displays.
        /// </summary>
        private void UpdateCooldowns()
        {
            if (skillSystem == null) return;

            foreach (SkillSlotUI slot in skillSlots)
            {
                if (slot != null)
                {
                    slot.UpdateCooldown();
                }
            }
        }

        /// <summary>
        /// Handle skill click.
        /// </summary>
        private void OnSkillClicked(int index)
        {
            if (skillSystem == null) return;

            // Find target (closest monster)
            Transform target = FindClosestMonster();

            skillSystem.UseSkill(index, target);
        }

        /// <summary>
        /// Find closest monster as target.
        /// </summary>
        private Transform FindClosestMonster()
        {
            SimpleMonsterAI[] monsters = FindObjectsByType<SimpleMonsterAI>(FindObjectsSortMode.None);
            Transform closest = null;
            float closestDist = Mathf.Infinity;

            foreach (SimpleMonsterAI monster in monsters)
            {
                if (monster == null) continue;

                float dist = Vector3.Distance(transform.position, monster.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = monster.transform;
                }
            }

            return closest;
        }
    }

    /// <summary>
    /// Individual skill slot UI.
    /// </summary>
    public class SkillSlotUI : MonoBehaviour
    {
        private int slotIndex;
        private Combat.SkillData skill;
        private Image cooldownImage;

        public void Initialize(int index, Combat.SkillData skillData, Image cooldown)
        {
            slotIndex = index;
            skill = skillData;
            cooldownImage = cooldown;
        }

        public void UpdateCooldown()
        {
            if (Combat.SkillSystem.Instance == null || cooldownImage == null) return;

            float remaining = Combat.SkillSystem.Instance.GetCooldownRemaining(skill.skillName);
            float cooldownPercent = remaining / skill.cooldown;
            cooldownImage.fillAmount = cooldownPercent;
        }
    }
}
