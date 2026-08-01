using UnityEngine;
using System.Collections.Generic;
using ArcadiaOnline.VFX;

namespace ArcadiaOnline.Combat
{
    /// <summary>
    /// Skill System: Active skills dengan cooldown dan MP cost.
    /// </summary>
    public class SkillSystem : MonoBehaviour
    {
        public static SkillSystem Instance { get; private set; }

        [Header("Skill Slots")]
        [SerializeField] private List<SkillData> equippedSkills = new List<SkillData>();
        [SerializeField] private int maxSkillSlots = 4;

        [Header("References")]
        [SerializeField] private Player.LevelUpSystem levelUpSystem;
        [SerializeField] private Player.PlayerStats playerStats;

        // Cooldown tracking
        private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>();

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
            if (levelUpSystem == null)
                levelUpSystem = GetComponent<Player.LevelUpSystem>();
            if (playerStats == null)
                playerStats = GetComponent<Player.PlayerStats>();

            InitializeDefaultSkills();
        }

        void Update()
        {
            // Update cooldown timers
            List<string> keys = new List<string>(cooldownTimers.Keys);
            foreach (string key in keys)
            {
                if (cooldownTimers[key] > 0)
                {
                    cooldownTimers[key] -= Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// Initialize default skills.
        /// </summary>
        private void InitializeDefaultSkills()
        {
            // Skill 1: Power Strike
            SkillData powerStrike = CreateSkill("Power Strike", "Serangan fisik kuat",
                SkillType.Active, DamageType.Physical, 2f, 10f, 3f, 1, SkillEffect.Damage);

            // Skill 2: Fire Bolt
            SkillData fireBolt = CreateSkill("Fire Bolt", "Serangan api",
                SkillType.Active, DamageType.Magic, 2.5f, 15f, 5f, 3, SkillEffect.Damage);

            // Skill 3: Heal
            SkillData heal = CreateSkill("Heal", "Pulihkan HP",
                SkillType.Active, DamageType.None, 0f, 20f, 8f, 5, SkillEffect.Heal);

            // Skill 4: Berserk
            SkillData berserk = CreateSkill("Berserk", "Tingkatkan ATK",
                SkillType.Active, DamageType.None, 0f, 25f, 15f, 7, SkillEffect.Buff);

            equippedSkills.Add(powerStrike);
            equippedSkills.Add(fireBolt);
            equippedSkills.Add(heal);
            equippedSkills.Add(berserk);
        }

        /// <summary>
        /// Helper buat buat skill.
        /// </summary>
        private SkillData CreateSkill(string name, string desc, SkillType type, DamageType dmgType,
            float dmgMult, float mp, float cd, int lvlReq, SkillEffect effect)
        {
            SkillData skill = ScriptableObject.CreateInstance<SkillData>();
            skill.id = name.ToLower().Replace(" ", "_");
            skill.skillName = name;
            skill.description = desc;
            skill.type = type;
            skill.damageType = dmgType;
            skill.damageMultiplier = dmgMult;
            skill.mpCost = mp;
            skill.cooldown = cd;
            skill.levelRequirement = lvlReq;
            skill.effect = effect;
            return skill;
        }

        /// <summary>
        /// Use skill by index.
        /// </summary>
        public bool UseSkill(int skillIndex, Transform target = null)
        {
            if (skillIndex < 0 || skillIndex >= equippedSkills.Count)
            {
                Debug.LogWarning("[Skill] Invalid skill index!");
                return false;
            }

            SkillData skill = equippedSkills[skillIndex];

            // Check level
            if (levelUpSystem != null && levelUpSystem.CurrentLevel < skill.levelRequirement)
            {
                Debug.Log($"[Skill] Level too low! Need level {skill.levelRequirement}");
                return false;
            }

            // Check cooldown
            if (IsOnCooldown(skill.skillName))
            {
                Debug.Log($"[Skill] {skill.skillName} is on cooldown!");
                return false;
            }

            // Check MP
            if (levelUpSystem != null && !levelUpSystem.UseMP(skill.mpCost))
            {
                Debug.Log("[Skill] Not enough MP!");
                return false;
            }

            // Execute
            ExecuteSkill(skill, target);

            // Start cooldown
            StartCooldown(skill.skillName, skill.cooldown);

            return true;
        }

        /// <summary>
        /// Execute skill effect.
        /// </summary>
        private void ExecuteSkill(SkillData skill, Transform target)
        {
            Debug.Log($"[Skill] Using {skill.skillName}!");

            switch (skill.effect)
            {
                case SkillEffect.Damage:
                    ExecuteDamageSkill(skill, target);
                    break;
                case SkillEffect.Heal:
                    ExecuteHealSkill(skill);
                    break;
                case SkillEffect.Buff:
                    ExecuteBuffSkill(skill);
                    break;
            }

            // Play sound
            if (Managers.JobSFXManager.Instance != null)
            {
                Managers.JobSFXManager.Instance.PlaySkill("male");
            }

            // Spawn effect
            SpawnSkillEffect(skill.effect, target);
        }

        /// <summary>
        /// Execute damage skill.
        /// </summary>
        private void ExecuteDamageSkill(SkillData skill, Transform target)
        {
            if (target == null) return;

            // Get base ATK or MATK
            float baseDamage = 10f;
            if (playerStats != null)
            {
                baseDamage = skill.damageType == DamageType.Physical
                    ? playerStats.BaseStats.atk
                    : playerStats.BaseStats.matk;
            }

            float totalDamage = baseDamage * skill.damageMultiplier;

            // Apply to target
            SimpleMonsterAI monster = target.GetComponent<SimpleMonsterAI>();
            if (monster != null)
            {
                bool isCritical = Random.Range(0f, 1f) < 0.15f;
                monster.TakeDamage(totalDamage, isCritical);
            }
        }

        /// <summary>
        /// Execute heal skill.
        /// </summary>
        private void ExecuteHealSkill(SkillData skill)
        {
            if (levelUpSystem != null)
            {
                float healAmount = levelUpSystem.MaxHP * 0.3f; // Heal 30% HP
                levelUpSystem.Heal(healAmount);
            }
        }

        /// <summary>
        /// Execute buff skill.
        /// </summary>
        private void ExecuteBuffSkill(SkillData skill)
        {
            Debug.Log($"[Skill] {skill.skillName} activated! ATK increased for 10 seconds.");
        }

        /// <summary>
        /// Spawn visual effect.
        /// </summary>
        private void SpawnSkillEffect(SkillEffect effect, Transform target)
        {
            Vector3 spawnPos = target != null ? target.position : transform.position;

            switch (effect)
            {
                case SkillEffect.Damage:
                    SimpleVFXCreator.CreateHitEffect().transform.position = spawnPos + Vector3.up;
                    break;
                case SkillEffect.Heal:
                    SimpleVFXCreator.CreateHealEffect().transform.position = spawnPos;
                    break;
                case SkillEffect.Buff:
                    SimpleVFXCreator.CreateLevelUpEffect().transform.position = spawnPos;
                    break;
            }
        }

        public bool IsOnCooldown(string skillName)
        {
            return cooldownTimers.ContainsKey(skillName) && cooldownTimers[skillName] > 0;
        }

        public float GetCooldownRemaining(string skillName)
        {
            if (cooldownTimers.ContainsKey(skillName))
                return Mathf.Max(0, cooldownTimers[skillName]);
            return 0f;
        }

        private void StartCooldown(string skillName, float duration)
        {
            cooldownTimers[skillName] = duration;
        }

        public List<SkillData> GetEquippedSkills()
        {
            return equippedSkills;
        }

        public SkillData GetSkill(int index)
        {
            if (index >= 0 && index < equippedSkills.Count)
                return equippedSkills[index];
            return null;
        }
    }
}
