using System;

namespace ArcadiaOnline.Data
{
    /// <summary>
    /// Kumpulan stat primer karakter. Lihat docs/01_GDD/08_Stats.md.
    /// </summary>
    [Serializable]
    public struct StatBlock
    {
        public float hp;
        public float mp;
        public float atk;
        public float def;
        public float matk;
        public float mdef;
        public float spd;
        public float luk;

        public static StatBlock operator +(StatBlock a, StatBlock b)
        {
            return new StatBlock
            {
                hp = a.hp + b.hp,
                mp = a.mp + b.mp,
                atk = a.atk + b.atk,
                def = a.def + b.def,
                matk = a.matk + b.matk,
                mdef = a.mdef + b.mdef,
                spd = a.spd + b.spd,
                luk = a.luk + b.luk
            };
        }

        /// <summary>Critical Rate = Base + (LUK * 0.1) + Equipment</summary>
        public float GetCriticalRate(float baseCR = 5f, float equipmentBonus = 0f)
        {
            return UnityEngine.Mathf.Clamp(baseCR + (luk * 0.1f) + equipmentBonus, 0f, 100f);
        }

        /// <summary>Critical Damage = 150% + (LUK * 0.05)</summary>
        public float GetCriticalDamage()
        {
            return 150f + (luk * 0.05f);
        }

        /// <summary>Evasion = Base + (SPD * 0.05) + Equipment</summary>
        public float GetEvasion(float baseEva = 0f, float equipmentBonus = 0f)
        {
            return UnityEngine.Mathf.Clamp(baseEva + (spd * 0.05f) + equipmentBonus, 0f, 75f);
        }

        /// <summary>Accuracy = Base + (SPD * 0.03) + Equipment</summary>
        public float GetAccuracy(float baseAcc = 90f, float equipmentBonus = 0f)
        {
            return baseAcc + (spd * 0.03f) + equipmentBonus;
        }
    }
}
