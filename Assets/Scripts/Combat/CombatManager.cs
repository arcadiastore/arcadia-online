using UnityEngine;
using ArcadiaOnline.Core;

namespace ArcadiaOnline.Combat
{
    /// <summary>
    /// Orkestrator pertarungan real-time. Lihat docs/01_GDD/05_Combat.md
    /// dan docs/02_TDD/CombatArchitecture.md.
    ///
    /// Alur:
    /// Player mendekati Enemy -> TargetLockSystem mendeteksi target ->
    /// Player menekan Attack -> CombatManager.ExecuteAttack() ->
    /// DamageCalculator menghitung damage -> Enemy menerima damage ->
    /// Enemy AI merespons -> ulangi sampai salah satu HP = 0
    /// </summary>
    public class CombatManager : Singleton<CombatManager>
    {
        public bool IsInCombat { get; private set; }
        public Transform CurrentTarget { get; private set; }

        [SerializeField] private TargetLockSystem _targetLock;
        [SerializeField] private SkillSystem _skillSystem;

        // Combo: 3 serangan beruntun = bonus damage
        private int _comboCount;
        [SerializeField] private float _comboWindow = 1.2f;
        [SerializeField] private float _comboBonusMultiplier = 1.25f;
        private float _comboTimer;

        [SerializeField] private float _dodgeInvincibilityDuration = 0.3f;
        [SerializeField] private float _dodgeCooldown = 1f;
        private float _dodgeTimer;
        private bool _isDodging;

        private void Update()
        {
            if (_comboTimer > 0f)
            {
                _comboTimer -= Time.deltaTime;
                if (_comboTimer <= 0f) _comboCount = 0;
            }

            if (_dodgeTimer > 0f) _dodgeTimer -= Time.deltaTime;
        }

        public void EnterCombat(Transform enemy)
        {
            IsInCombat = true;
            CurrentTarget = enemy;
            Events.CombatEntered(enemy);
        }

        public void ExitCombat()
        {
            IsInCombat = false;
            CurrentTarget = null;
            _comboCount = 0;
            Events.CombatExited();
        }

        public void SwitchTarget()
        {
            _targetLock?.SwitchToNextTarget();
            CurrentTarget = _targetLock?.CurrentTarget;
        }

        /// <summary>Serangan dasar - tanpa cooldown, tidak menghabiskan MP.</summary>
        public void ExecuteAttack(IDamageable attackerStats, IDamageable targetStats,
            float atk, float targetDef)
        {
            if (targetStats == null || targetStats.IsDead) return;

            _comboCount = Mathf.Min(_comboCount + 1, 3);
            _comboTimer = _comboWindow;

            float multiplier = _comboCount >= 3 ? _comboBonusMultiplier : 1f;

            DamageResult result = DamageCalculator.CalculatePhysical(
                atk, targetDef, multiplier, 5f, 150f);

            targetStats.TakeDamage(result);
        }

        public void ExecuteSkill(int skillIndex, IDamageable targetStats, StatData attackerStats)
        {
            // Implementasikan sesuai kebutuhan: ambil SkillData dari SkillSystem,
            // hitung damage/heal via DamageCalculator, terapkan efek (buff/debuff/CC).
        }

        public void ExecuteDefend()
        {
            // Block: mengurangi damage 50%, tidak ada cooldown.
            // Set flag "isBlocking" yang dibaca oleh IDamageable.TakeDamage implementasi Player.
        }

        public bool TryExecuteDodge()
        {
            if (_dodgeTimer > 0f) return false;
            _dodgeTimer = _dodgeCooldown;
            _isDodging = true;
            Invoke(nameof(EndDodge), _dodgeInvincibilityDuration);
            return true;
        }

        private void EndDodge() => _isDodging = false;

        public bool IsInvincible => _isDodging;

        public void ExecuteFlee()
        {
            ExitCombat();
        }
    }

    // Placeholder struct - ganti dengan referensi ArcadiaOnline.Data.StatBlock
    // di project sebenarnya. Disediakan agar file ini tetap self-contained.
    public struct StatData { }
}
