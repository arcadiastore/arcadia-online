using UnityEngine;
using System.Collections.Generic;

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
            {
                levelUpSystem = GetComponent<Player.LevelUpSystem>();
            }

            // Initialize default skills
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
        /// Initialize default skills berdasarkan level.
        /// </summary>
        private void InitializeDefaultSkills()
        {
            // Skill 1: Power Strike (Physical attack)
            SkillData powerStrike = new SkillData
            {
                skillName = "Power Strike",
                description = "Serangan fisik kuat",
                skillType = SkillType.Physical,
                damage = 20f,
                mpCost = 10f,
                cooldown = 3f,
                range = 2.5f,
                levelRequired = 1
            };

            // Skill 2: Fire Bolt (Magic attack)
            SkillData fireBolt = new SkillData
            {
                skillName = "Fire Bolt",
                description = "Serangan api",
                skillType = SkillType.Magical,
                damage = 25f,
                mpCost = 15f,
                cooldown = 5f,
                range = 5f,
                levelRequired = 3
            };

            // Skill 3: Heal (Restore HP)
            SkillData heal = new SkillData
            {
                skillName = "Heal",
                description = "Pulihkan HP",
                skillType = SkillType.Heal,
                damage = 30f, // Heal amount
                mpCost = 20f,
                cooldown = 8f,
                range = 0f, // Self
                levelRequired = 5
            };

            // Skill 4: Berserk (Buff)
            SkillData berserk = new SkillData
            {
                skillName = "Berserk",
                description = "Tingkatkan ATK",
                skillType = SkillType.Buff,
                damage = 0f,
                mpCost = 25f,
                cooldown = 15f,
                range = 0f, // Self
                levelRequired = 7
            };

            equippedSkills.Add(powerStrike);
            equippedSkills.Add(fireBolt);
            equippedSkills.Add(heal);
            equippedSkills.Add(berserk);
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

            // Check level requirement
            if (levelUpSystem != null && levelUpSystem.CurrentLevel < skill.levelRequired)
            {
                Debug.Log($"[Skill] Level too low! Need level {skill.levelRequired}");
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

            // Execute skill
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

            switch (skill.skillType)
            {
                case SkillType.Physical:
                    ExecutePhysicalSkill(skill, target);
                    break;
                case SkillType.Magical:
                    ExecuteMagicalSkill(skill, target);
                    break;
                case SkillType.Heal:
                    ExecuteHealSkill(skill);
                    break;
                case SkillType.Buff:
                    ExecuteBuffSkill(skill);
                    break;
            }

            // Play skill sound
            if (Managers.JobSFXManager.Instance != null)
            {
                Managers.JobSFXManager.Instance.PlaySkill("male");
            }

            // Spawn skill effect
            SpawnSkillEffect(skill.skillType, target);
        }

        /// <summary>
        /// Execute physical skill.
        /// </summary>
        private void ExecutePhysicalSkill(SkillData skill, Transform target)
        {
            if (target == null) return;

            // Get player ATK
            float playerATK = 10f;
            PlayerStats playerStats = GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerATK = playerStats.BaseStats.atk;
            }

            // Calculate damage
            float totalDamage = playerATK + skill.damage;

            // Apply to target
            SimpleMonsterAI monster = target.GetComponent<SimpleMonsterAI>();
            if (monster != null)
            {
                bool isCritical = Random.Range(0f, 1f) < 0.15f; // 15% crit chance for skills
                monster.TakeDamage(totalDamage, isCritical);
            }
        }

        /// <summary>
        /// Execute magical skill.
        /// </summary>
        private void ExecuteMagicalSkill(SkillData skill, Transform target)
        {
            if (target == null) return;

            // Get player MATK
            float playerMATK = 10f;
            PlayerStats playerStats = GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerMATK = playerStats.BaseStats.matk;
            }

            // Calculate damage
            float totalDamage = playerMATK + skill.damage;

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
                levelUpSystem.Heal(skill.damage);
            }
        }

        /// <summary>
        /// Execute buff skill.
        /// </summary>
        private void ExecuteBuffSkill(SkillData skill)
        {
            // Buff implementation (increase ATK temporarily)
            Debug.Log($"[Skill] {skill.skillName} activated! ATK increased for 10 seconds.");
        }

        /// <summary>
        /// Spawn visual effect for skill.
        /// </summary>
        private void SpawnSkillEffect(SkillType type, Transform target)
        {
            Vector3 spawnPos = transform.position;

            if (target != null)
            {
                spawnPos = target.position;
            }

            switch (type)
            {
                case SkillType.Physical:
                    VFX.SimpleVFXCreator.CreateHitEffect().transform.position = spawnPos + Vector3.up;
                    break;
                case SkillType.Magical:
                    VFX.SimpleVFXCreator.CreateSkillEffect().transform.position = spawnPos + Vector3.up;
                    break;
                case SkillType.Heal:
                    VFX.SimpleVFXCreator.CreateHealEffect().transform.position = spawnPos;
                    break;
                case SkillType.Buff:
                    VFX.SimpleVFXCreator.CreateLevelUpEffect().transform.position = spawnPos;
                    break;
            }
        }

        /// <summary>
        /// Check if skill is on cooldown.
        /// </summary>
        public bool IsOnCooldown(string skillName)
        {
            return cooldownTimers.ContainsKey(skillName) && cooldownTimers[skillName] > 0;
        }

        /// <summary>
        /// Get cooldown remaining for skill.
        /// </summary>
        public float GetCooldownRemaining(string skillName)
        {
            if (cooldownTimers.ContainsKey(skillName))
            {
                return Mathf.Max(0, cooldownTimers[skillName]);
            }
            return 0f;
        }

        /// <summary>
        /// Start cooldown for skill.
        /// </summary>
        private void StartCooldown(string skillName, float duration)
        {
            cooldownTimers[skillName] = duration;
        }

        /// <summary>
        /// Get equipped skills list.
        /// </summary>
        public List<SkillData> GetEquippedSkills()
        {
            return equippedSkills;
        }

        /// <summary>
        /// Get skill by index.
        /// </summary>
        public SkillData GetSkill(int index)
        {
            if (index >= 0 && index < equippedSkills.Count)
            {
                return equippedSkills[index];
            }
            return null;
        }
    }

    /// <summary>
    /// Skill data structure.
    /// </summary>
    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public string description;
        public SkillType skillType;
        public float damage;
        public float mpCost;
        public float cooldown;
        public float range;
        public int levelRequired;
    }

    /// <summary>
    /// Skill types.
    /// </summary>
    public enum SkillType
    {
        Physical,
        Magical,
        Heal,
        Buff
    }
}
