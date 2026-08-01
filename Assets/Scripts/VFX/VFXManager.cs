using UnityEngine;

namespace ArcadiaOnline.VFX
{
    /// <summary>
    /// Manager untuk particle effects (hit, death, skill, dll).
    /// Attach ke GameObject "VFXManager".
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        [Header("Hit Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject criticalHitEffectPrefab;

        [Header("Death Effects")]
        [SerializeField] private GameObject deathEffectPrefab;

        [Header("Skill Effects")]
        [SerializeField] private GameObject skillEffectPrefab;
        [SerializeField] private GameObject healEffectPrefab;

        [Header("Level Up Effect")]
        [SerializeField] private GameObject levelUpEffectPrefab;

        [Header("Settings")]
        [SerializeField] private float effectLifetime = 2f;

        public static VFXManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// Spawn hit effect di posisi target.
        /// </summary>
        public void SpawnHitEffect(Vector3 position)
        {
            SpawnEffect(hitEffectPrefab, position);
        }

        /// <summary>
        /// Spawn critical hit effect (lebih besar/mencolok).
        /// </summary>
        public void SpawnCriticalHitEffect(Vector3 position)
        {
            SpawnEffect(criticalHitEffectPrefab, position);
        }

        /// <summary>
        /// Spawn death effect di posisi target.
        /// </summary>
        public void SpawnDeathEffect(Vector3 position)
        {
            SpawnEffect(deathEffectPrefab, position);
        }

        /// <summary>
        /// Spawn skill effect di posisi target.
        /// </summary>
        public void SpawnSkillEffect(Vector3 position)
        {
            SpawnEffect(skillEffectPrefab, position);
        }

        /// <summary>
        /// Spawn heal effect di posisi target.
        /// </summary>
        public void SpawnHealEffect(Vector3 position)
        {
            SpawnEffect(healEffectPrefab, position);
        }

        /// <summary>
        /// Spawn level up effect di posisi target.
        /// </summary>
        public void SpawnLevelUpEffect(Vector3 position)
        {
            SpawnEffect(levelUpEffectPrefab, position);
        }

        private void SpawnEffect(GameObject prefab, Vector3 position)
        {
            if (prefab == null)
            {
                Debug.LogWarning("[VFX] Effect prefab belum di-assign!");
                return;
            }

            GameObject effect = Instantiate(prefab, position, Quaternion.identity);

            // Auto destroy setelah lifetime
            Destroy(effect, effectLifetime);
        }
    }
}
