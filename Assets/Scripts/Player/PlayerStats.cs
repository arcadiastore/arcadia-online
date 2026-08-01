using UnityEngine;
using ArcadiaOnline.Core;
using ArcadiaOnline.Data;
using ArcadiaOnline.Combat;

namespace ArcadiaOnline.Player
{
    /// <summary>
    /// Stat & progresi pemain. Lihat docs/01_GDD/04_Player.md dan
    /// docs/01_GDD/08_Stats.md.
    /// </summary>
    public class PlayerStats : MonoBehaviour, IDamageable
    {
        [SerializeField] private JobData _job;

        public int Level { get; private set; } = 1;
        public float Exp { get; private set; }
        public StatBlock BaseStats { get; private set; }

        private float _currentHP;
        private float _currentMP;
        private float _stamina = 100f;

        public const int LEVEL_CAP = 100;
        public const float MAX_STAMINA = 100f;

        public float CurrentHP => _currentHP;
        public float MaxHP => BaseStats.hp;
        public bool IsDead => _currentHP <= 0f;
        public float CurrentMP => _currentMP;
        public float MaxMP => BaseStats.mp;
        public float Stamina => _stamina;

        private void Awake()
        {
            if (_job != null)
            {
                BaseStats = _job.startingStats;
            }
            _currentHP = MaxHP;
            _currentMP = MaxMP;
        }

        private void Update()
        {
            // Regenerasi stamina: 1/menit online, 1/5menit offline (implementasikan
            // pengecekan mode online/offline sesuai World/OfflineToOnline.md).
            _stamina = Mathf.Min(MAX_STAMINA, _stamina + (Time.deltaTime / 60f));
        }

        /// <summary>EXP_needed = Base * (Level ^ 1.5)</summary>
        public float GetExpNeededForLevel(int level, float baseExp = 100f)
        {
            return baseExp * Mathf.Pow(level, 1.5f);
        }

        public void GainExp(float amount)
        {
            if (Level >= LEVEL_CAP) return;

            Exp += amount;
            float needed = GetExpNeededForLevel(Level);
            while (Exp >= needed && Level < LEVEL_CAP)
            {
                Exp -= needed;
                LevelUp();
                needed = GetExpNeededForLevel(Level);
            }
        }

        private void LevelUp()
        {
            Level++;
            if (_job != null)
            {
                BaseStats += _job.statGainPerLevel;
            }
            _currentHP = MaxHP;
            _currentMP = MaxMP;
            Events.PlayerLevelUp(Level);
        }

        public void TakeDamage(DamageResult damage)
        {
            if (IsDead) return;
            if (CombatManager.Instance != null && CombatManager.Instance.IsInvincible) return;

            _currentHP = Mathf.Max(0f, _currentHP - damage.Damage);
            Events.HPChanged(_currentHP, MaxHP);

            if (_currentHP <= 0f)
            {
                Events.PlayerDied();
            }
        }

        public void Heal(float amount)
        {
            _currentHP = Mathf.Min(MaxHP, _currentHP + amount);
            Events.HPChanged(_currentHP, MaxHP);
        }

        public bool TrySpendMP(float amount)
        {
            if (_currentMP < amount) return false;
            _currentMP -= amount;
            Events.MPChanged(_currentMP, MaxMP);
            return true;
        }

        public bool TrySpendStamina(float amount)
        {
            if (_stamina < amount) return false;
            _stamina -= amount;
            return true;
        }
    }
}
