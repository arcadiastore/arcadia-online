using UnityEngine;
using UnityEngine.UI;

namespace ArcadiaOnline.Player
{
    /// <summary>
    /// Level Up System: EXP, level, stat growth.
    /// </summary>
    public class LevelUpSystem : MonoBehaviour
    {
        public static LevelUpSystem Instance { get; private set; }

        [Header("Level Settings")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int currentEXP = 0;
        [SerializeField] private int expToNextLevel = 100;
        [SerializeField] private float expMultiplier = 1.5f; // EXP needed multiplier per level

        [Header("Stat Growth (GDD Balance)")]
        [SerializeField] private float baseHP = 150f;      // More survivable
        [SerializeField] private float hpGrowth = 15f;      // HP per level
        [SerializeField] private float baseMP = 50f;
        [SerializeField] private float mpGrowth = 5f;       // MP per level
        [SerializeField] private float baseATK = 25f;       // Faster kills (2-3 hit slime)
        [SerializeField] private float atkGrowth = 3f;      // ATK per level
        [SerializeField] private float baseDEF = 8f;
        [SerializeField] private float defGrowth = 2f;      // DEF per level

        [Header("Current Stats")]
        [SerializeField] private float currentHP;
        [SerializeField] private float maxHP;
        [SerializeField] private float currentMP;
        [SerializeField] private float maxMP;
        [SerializeField] private float atk;
        [SerializeField] private float def;

        // Properties
        public int CurrentLevel => currentLevel;
        public int CurrentEXP => currentEXP;
        public int ExpToNextLevel => expToNextLevel;
        public float CurrentHP => currentHP;
        public float MaxHP => maxHP;
        public float CurrentMP => currentMP;
        public float MaxMP => maxMP;
        public float ATK => atk;
        public float DEF => def;

        // Events
        public System.Action<int> OnLevelUp;
        public System.Action<int> OnEXPGained;

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
            // Calculate initial stats
            CalculateStats();
            currentHP = maxHP;
            currentMP = maxMP;
        }

        /// <summary>
        /// Add EXP ke player.
        /// </summary>
        public void AddEXP(int amount)
        {
            currentEXP += amount;

            Debug.Log($"[LevelUp] +{amount} EXP ({currentEXP}/{expToNextLevel})");

            // Notify listeners
            OnEXPGained?.Invoke(amount);

            // Check level up
            while (currentEXP >= expToNextLevel)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// Level up!
        /// </summary>
        private void LevelUp()
        {
            currentEXP -= expToNextLevel;
            currentLevel++;

            // Increase EXP requirement
            expToNextLevel = Mathf.RoundToInt(expToNextLevel * expMultiplier);

            // Calculate new stats
            CalculateStats();

            // Full heal on level up
            currentHP = maxHP;
            currentMP = maxMP;

            Debug.Log($"[LevelUp] LEVEL UP! Level {currentLevel} | HP: {maxHP} | MP: {maxMP} | ATK: {atk} | DEF: {def}");

            // Notify listeners
            OnLevelUp?.Invoke(currentLevel);

            // Spawn level up effect
            SpawnLevelUpEffect();
        }

        /// <summary>
        /// Calculate stats berdasarkan level.
        /// </summary>
        private void CalculateStats()
        {
            maxHP = baseHP + (hpGrowth * (currentLevel - 1));
            maxMP = baseMP + (mpGrowth * (currentLevel - 1));
            atk = baseATK + (atkGrowth * (currentLevel - 1));
            def = baseDEF + (defGrowth * (currentLevel - 1));
        }

        /// <summary>
        /// Take damage dari monster.
        /// </summary>
        public void TakeDamage(float damage)
        {
            // Reduce damage by defense
            float finalDamage = Mathf.Max(1, damage - def);
            finalDamage = Mathf.Floor(finalDamage);

            currentHP = Mathf.Max(0, currentHP - finalDamage);

            Debug.Log($"[Player] Terkena {finalDamage} damage! HP: {currentHP}/{maxHP}");

            if (currentHP <= 0)
            {
                PlayerDeath();
            }
        }

        /// <summary>
        /// Heal player.
        /// </summary>
        public void Heal(float amount)
        {
            currentHP = Mathf.Min(maxHP, currentHP + amount);
            Debug.Log($"[Player] Heal {amount}! HP: {currentHP}/{maxHP}");
        }

        /// <summary>
        /// Use MP.
        /// </summary>
        public bool UseMP(float amount)
        {
            if (currentMP >= amount)
            {
                currentMP -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Restore MP.
        /// </summary>
        public void RestoreMP(float amount)
        {
            currentMP = Mathf.Min(maxMP, currentMP + amount);
        }

        /// <summary>
        /// Player mati.
        /// </summary>
        private void PlayerDeath()
        {
            Debug.Log("[Player] MATI!");

            // Respawn after delay
            Invoke(nameof(Respawn), 3f);
        }

        /// <summary>
        /// Respawn player.
        /// </summary>
        private void Respawn()
        {
            currentHP = maxHP * 0.5f; // Respawn dengan 50% HP
            currentMP = maxMP * 0.5f;

            Debug.Log("[Player] Respawn!");
        }

        /// <summary>
        /// Spawn level up effect.
        /// </summary>
        private void SpawnLevelUpEffect()
        {
            // Create golden ring effect
            GameObject effect = new GameObject("LevelUpEffect");
            effect.transform.position = transform.position;

            ParticleSystem ps = effect.AddComponent<ParticleSystem>();
            
            // Stop first to allow property changes
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            var main = ps.main;
            main.duration = 1f;
            main.startLifetime = 1.5f;
            main.startSpeed = 2f;
            main.startSize = 0.5f;
            main.startColor = new Color(1f, 0.8f, 0f); // Gold
            main.maxParticles = 30;
            main.loop = false;
            main.playOnAwake = false;

            var emission = ps.emission;
            emission.rateOverTime = 30;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 1f;

            // Now play
            ps.Play();

            // Destroy after animation
            Destroy(effect, 2f);
        }

        /// <summary>
        /// Get EXP percentage for UI.
        /// </summary>
        public float GetEXPPercentage()
        {
            return (float)currentEXP / expToNextLevel;
        }

        /// <summary>
        /// Get HP percentage for UI.
        /// </summary>
        public float GetHPPercentage()
        {
            return currentHP / maxHP;
        }

        /// <summary>
        /// Get MP percentage for UI.
        /// </summary>
        public float GetMPPercentage()
        {
            return currentMP / maxMP;
        }
    }
}
