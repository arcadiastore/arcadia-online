using UnityEngine;
using ArcadiaOnline.Data;

namespace ArcadiaOnline.Combat
{
    public struct DamageResult
    {
        public float Damage;
        public bool IsCritical;
    }

    /// <summary>
    /// Formula damage. Lihat docs/01_GDD/08_Stats.md dan
    /// docs/02_TDD/CombatArchitecture.md.
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>Damage = (ATK * Skill_Multiplier) - (Target_DEF * 0.5), minimum 1</summary>
        public static DamageResult CalculatePhysical(
            float atk, float targetDef, float skillMultiplier,
            float critRatePercent, float critDamagePercent)
        {
            return Calculate(atk, targetDef, skillMultiplier, critRatePercent, critDamagePercent);
        }

        /// <summary>Damage = (MATK * Skill_Multiplier) - (Target_MDEF * 0.5), minimum 1</summary>
        public static DamageResult CalculateMagic(
            float matk, float targetMdef, float skillMultiplier,
            float critRatePercent, float critDamagePercent)
        {
            return Calculate(matk, targetMdef, skillMultiplier, critRatePercent, critDamagePercent);
        }

        private static DamageResult Calculate(
            float offense, float defense, float skillMultiplier,
            float critRatePercent, float critDamagePercent)
        {
            float baseDamage = (offense * skillMultiplier) - (defense * 0.5f);
            bool isCrit = Random.value * 100f < critRatePercent;
            float finalDamage = isCrit ? baseDamage * (critDamagePercent / 100f) : baseDamage;

            return new DamageResult
            {
                Damage = Mathf.Max(1f, finalDamage),
                IsCritical = isCrit
            };
        }

        public static DamageResult CalculateFromStats(
            StatBlock attacker, StatBlock target, SkillData skill)
        {
            float multiplier = skill != null ? skill.damageMultiplier : 1f;

            if (skill != null && skill.damageType == DamageType.Magic)
            {
                return CalculateMagic(attacker.matk, target.mdef, multiplier,
                    attacker.GetCriticalRate(), attacker.GetCriticalDamage());
            }

            return CalculatePhysical(attacker.atk, target.def, multiplier,
                attacker.GetCriticalRate(), attacker.GetCriticalDamage());
        }
    }
}
