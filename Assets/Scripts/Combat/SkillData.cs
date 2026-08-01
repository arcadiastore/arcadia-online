using UnityEngine;

namespace ArcadiaOnline.Combat
{
    /// <summary>
    /// Definisi 1 skill. Lihat docs/02_TDD/CombatArchitecture.md dan
    /// docs/01_GDD/07_Skills.md.
    /// Buat asset via: Assets > Create > Arcadia > Skill Data
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Arcadia/Skill Data")]
    public class SkillData : ScriptableObject
    {
        public string id;
        public string skillName;
        [TextArea] public string description;

        public SkillType type;
        public DamageType damageType;

        public int tier = 1;
        public float mpCost;
        public float cooldown;
        public float damageMultiplier = 1f;
        public SkillEffect effect;

        [Header("Requirement")]
        public int levelRequirement = 1;
        public SkillData prerequisite;
    }

    public enum SkillType
    {
        Active,
        Passive,
        Ultimate
    }

    public enum DamageType
    {
        Physical,
        Magic,
        None
    }

    public enum SkillEffect
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        CrowdControl,
        AoEDamage
    }
}
