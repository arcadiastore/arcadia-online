using UnityEngine;
using ArcadiaOnline.Player;
using ArcadiaOnline.Managers;
using ArcadiaOnline.VFX;

namespace ArcadiaOnline.Monster
{
    /// <summary>
    /// Monster AI: Patrol → Chase → Attack. Bisa di-klik untuk diserang.
    /// HP regen saat kembali patrol.
    /// </summary>
    public class SimpleMonsterAI : MonoBehaviour
    {
        [Header("Stats (GDD Balance)")]
        [SerializeField] private string monsterName = "Monster";
        [SerializeField] private float maxHP = 50f;      // Reduced for faster kills
        [SerializeField] private float attack = 8f;       // Reduced for balance
        [SerializeField] private float defense = 2f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float detectRange = 8f;
        [SerializeField] private float moveSpeed = 3f;

        [Header("Patrol")]
        [SerializeField] private float patrolRadius = 5f;
        [SerializeField] private float patrolWaitTime = 2f;

        [Header("Attack")]
        [SerializeField] private float attackCooldown = 1.5f;

        [Header("HP Regen")]
        [SerializeField] private float regenRate = 5f; // HP per detik saat patrol
        [SerializeField] private float regenDelay = 3f; // Delay sebelum regen mulai

        [Header("EXP")]
        [SerializeField] private int expReward = 25;

        // State
        private enum AIState { Idle, Patrol, Chase, Attack, Dead }
        private AIState currentState = AIState.Idle;
        private float currentHP;
        private bool isDead = false;
        private float regenTimer;

        // Patrol
        private Vector3 spawnPosition;
        private Vector3 patrolTarget;
        private float patrolTimer;

        // Attack
        private float attackTimer;
        private Transform playerTarget;

        // References
        private Renderer meshRenderer;
        private Color originalColor;

        // === PUBLIC PROPERTIES ===
        public string MonsterName => monsterName;
        public float CurrentHP => currentHP;
        public float MaxHP => maxHP;
        public float HPPercent => currentHP / maxHP;
        public bool IsChasing => currentState == AIState.Chase || currentState == AIState.Attack;
        public bool IsDead => isDead;

        void Awake()
        {
            currentHP = maxHP;
            spawnPosition = transform.position;
            meshRenderer = GetComponentInChildren<Renderer>();
            if (meshRenderer != null)
            {
                originalColor = meshRenderer.material.color;
            }
        }

        void Start()
        {
            currentState = AIState.Patrol;
            SetNewPatrolTarget();
            regenTimer = regenDelay;
        }

        void Update()
        {
            if (isDead) return;

            DetectPlayer();

            switch (currentState)
            {
                case AIState.Idle:
                    UpdateIdle();
                    break;
                case AIState.Patrol:
                    UpdatePatrol();
                    UpdateHPRegen();
                    break;
                case AIState.Chase:
                    UpdateChase();
                    regenTimer = regenDelay; // Reset regen timer saat chase
                    break;
                case AIState.Attack:
                    UpdateAttack();
                    regenTimer = regenDelay; // Reset regen timer saat attack
                    break;
            }

            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// HP Regeneration saat patrol/idle.
        /// </summary>
        private void UpdateHPRegen()
        {
            if (currentHP >= maxHP) return;

            regenTimer -= Time.deltaTime;
            if (regenTimer <= 0)
            {
                // Regen HP
                currentHP = Mathf.Min(maxHP, currentHP + regenRate * Time.deltaTime);

                // Update UI jika ada
                if (UI.MonsterInfoUI.Instance != null)
                {
                    UI.MonsterInfoUI.Instance.OnMonsterDamaged(this);
                }
            }
        }

        private void DetectPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                player = GameObject.Find("Player");
            }

            if (player == null)
            {
                playerTarget = null;
                return;
            }

            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= detectRange)
            {
                playerTarget = player.transform;

                if (distance <= attackRange)
                {
                    currentState = AIState.Attack;
                }
                else if (currentState != AIState.Attack)
                {
                    currentState = AIState.Chase;
                }
            }
            else
            {
                playerTarget = null;

                if (currentState == AIState.Chase || currentState == AIState.Attack)
                {
                    currentState = AIState.Patrol;
                    SetNewPatrolTarget();
                    regenTimer = regenDelay; // Mulai hitung regen
                }
            }
        }

        private void UpdateIdle()
        {
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0)
            {
                currentState = AIState.Patrol;
                SetNewPatrolTarget();
            }
        }

        private void UpdatePatrol()
        {
            Vector3 direction = (patrolTarget - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            float distance = Vector3.Distance(transform.position, patrolTarget);
            if (distance < 0.5f)
            {
                currentState = AIState.Idle;
                patrolTimer = patrolWaitTime;
            }
        }

        private void UpdateChase()
        {
            if (playerTarget == null) return;

            Vector3 direction = (playerTarget.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private void UpdateAttack()
        {
            if (playerTarget == null) return;

            Vector3 direction = (playerTarget.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (attackTimer <= 0)
            {
                PerformAttack();
                attackTimer = attackCooldown;
            }
        }

        private void PerformAttack()
        {
            if (playerTarget == null) return;

            float damage = attack * Random.Range(0.9f, 1.1f);
            damage = Mathf.Floor(damage);

            Debug.Log($"[Monster] Attack player! Damage: {damage}");
        }

        private void SetNewPatrolTarget()
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            patrolTarget = spawnPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
        }

        /// <summary>
        /// Klik untuk menyerang monster.
        /// </summary>
        private void OnMouseDown()
        {
            if (isDead) return;

            // Show monster info UI
            if (UI.MonsterInfoUI.Instance != null)
            {
                UI.MonsterInfoUI.Instance.ShowMonsterInfo(this);
            }

            // Cari player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                player = GameObject.Find("Player");
            }

            if (player == null) return;

            // Ambil damage dari player (GDD: Lv1 Warrior base ATK = 25)
            float rawDamage = 25f;  // Default fallback
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                rawDamage = playerStats.BaseStats.atk;
                Debug.Log($"[Combat] Player ATK from PlayerStats: {rawDamage}");
            }
            else
            {
                Debug.Log("[Combat] PlayerStats not found, using default damage: 25");
            }

            // Minimum damage guarantee
            if (rawDamage < 10f)
            {
                rawDamage = 25f;  // Force minimum damage
                Debug.Log("[Combat] Damage too low, forcing to 25");
            }

            // Hitung critical (10% chance)
            bool isCritical = Random.Range(0f, 1f) < 0.1f;

            // Terima damage
            TakeDamage(rawDamage, isCritical);

            // Play hit sound
            if (JobSFXManager.Instance != null)
            {
                JobSFXManager.Instance.PlayHit("male");
            }
        }

        /// <summary>
        /// Terima damage dari player.
        /// </summary>
        public void TakeDamage(float rawDamage, bool isCritical = false)
        {
            if (isDead) return;

            // Reset regen timer saat terkena damage
            regenTimer = regenDelay;

            // Trigger battle BGM
            if (BattleBGMManager.Instance != null)
            {
                BattleBGMManager.Instance.EnterBattle();
            }

            // Hitung damage - minimal 1 damage
            float damage = rawDamage * Random.Range(0.9f, 1.1f);

            if (isCritical)
            {
                damage *= 1.5f;
            }

            damage = Mathf.Max(1, Mathf.Floor(damage)); // Minimal 1 damage
            currentHP = Mathf.Max(0, currentHP - damage);

            // Visual feedback
            HitFlash();

            // Spawn damage popup
            if (DamagePopupSpawner.Instance != null)
            {
                DamagePopupSpawner.Instance.SpawnDamagePopup(transform.position, damage, isCritical, transform);
            }

            // Spawn hit effect
            SimpleVFXCreator.CreateHitEffect().transform.position = transform.position + Vector3.up;

            // Play hit sound
            if (JobSFXManager.Instance != null)
            {
                JobSFXManager.Instance.PlayHit("male");
            }

            // Update monster info UI
            if (UI.MonsterInfoUI.Instance != null)
            {
                UI.MonsterInfoUI.Instance.OnMonsterDamaged(this);
            }

            // Chase player setelah diserang
            if (playerTarget != null)
            {
                currentState = AIState.Chase;
            }

            Debug.Log($"[Monster] Terkena {damage} damage! (Critical: {isCritical}) HP: {currentHP}/{maxHP}");

            if (currentHP <= 0)
            {
                Die();
            }
        }

        private void HitFlash()
        {
            if (meshRenderer != null)
            {
                StartCoroutine(FlashCoroutine());
            }
        }

        private System.Collections.IEnumerator FlashCoroutine()
        {
            meshRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            meshRenderer.material.color = originalColor;
        }

        private void Die()
        {
            isDead = true;
            currentState = AIState.Dead;

            // Hide monster info UI
            if (UI.MonsterInfoUI.Instance != null)
            {
                UI.MonsterInfoUI.Instance.HideMonsterInfo();
            }

            // Beri EXP ke player
            GiveEXPToPlayer();

            // Spawn death effect
            SimpleVFXCreator.CreateDeathEffect().transform.position = transform.position + Vector3.up * 0.5f;

            // Exit battle BGM
            if (BattleBGMManager.Instance != null)
            {
                BattleBGMManager.Instance.ExitBattle();
            }

            Debug.Log($"[Monster] MATI! EXP: {expReward}");

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Destroy(gameObject, 2f);
        }

        private void GiveEXPToPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                player = GameObject.Find("Player");
            }

            if (player != null)
            {
                LevelUpSystem levelUp = player.GetComponent<LevelUpSystem>();
                if (levelUp != null)
                {
                    levelUp.AddEXP(expReward);
                    Debug.Log($"[EXP] Player mendapat {expReward} EXP");
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.blue;
            Vector3 center = Application.isPlaying ? spawnPosition : transform.position;
            Gizmos.DrawWireSphere(center, patrolRadius);
        }
    }
}
