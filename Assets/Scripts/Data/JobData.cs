using UnityEngine;
using ArcadiaOnline.Combat;

namespace ArcadiaOnline.Data
{
    /// <summary>
    /// Definisi Job (Warrior/Mage/Archer, dst). Lihat docs/01_GDD/04_Player.md
    /// dan docs/01_GDD/06_Jobs.md.
    /// Buat asset via: Assets > Create > Arcadia > Job Data
    /// </summary>
    [CreateAssetMenu(fileName = "NewJob", menuName = "Arcadia/Job Data")]
    public class JobData : ScriptableObject
    {
        public string jobId;
        public string jobName;
        public JobRole role;
        public int tier = 1;
        public JobData previousTierJob;

        [Header("Starting Stats (Lv 1)")]
        public StatBlock startingStats;

        [Header("Stat Gain per Level")]
        public StatBlock statGainPerLevel;

        [Header("Skill Tree")]
        public SkillData[] skillTree;
    }

    public enum JobRole
    {
        Tank,
        MeleeDPS,
        RangedPhysicalDPS,
        RangedMagicDPS,
        Support
    }
}
