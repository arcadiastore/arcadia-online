using UnityEngine;
using ArcadiaOnline.Data;
using ArcadiaOnline.Combat;

namespace ArcadiaOnline.Monster
{
    /// <summary>Lihat docs/01_GDD/11_Monsters.md.</summary>
    public class MonsterStats : MonoBehaviour, IDamageable
    {
        [SerializeField] private StatBlock _stats;
        private float _currentHP;

        public StatBlock Stats => _stats;
        public float CurrentHP => _currentHP;
        public float MaxHP => _stats.hp;
        public bool IsDead => _currentHP <= 0f;

        private void Awake()
        {
            _currentHP = MaxHP;
        }

        public void TakeDamage(DamageResult damage)
        {
            if (IsDead) return;
            _currentHP = Mathf.Max(0f, _currentHP - damage.Damage);

            if (_currentHP <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            _currentHP = Mathf.Min(MaxHP, _currentHP + amount);
        }

        private void Die()
        {
            // TODO: trigger drop table (lihat docs/03_DDD/DropTableDB.md),
            // beri EXP ke player, kembalikan ke ObjectPool (bukan Destroy).
            var ai = GetComponent<MonsterAI>();
            if (ai != null) ai.enabled = false;

            Destroy(gameObject, 2f); // sementara: ganti dengan ObjectPool.Return di implementasi final
        }
    }
}
