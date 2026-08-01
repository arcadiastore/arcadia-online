using UnityEngine;
using ArcadiaOnline.Managers;

namespace ArcadiaOnline.Monster
{
    /// <summary>
    /// Monster AI sederhana: Patrol → Chase → Attack.
    /// Tidak pakai NavMeshAgent, langsung transform.position.
    /// </summary>
    public class SimpleMonsterAI : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float maxHP = 100f;
        [SerializeField] private float attack = 10f;
        [SerializeField] private float defense = 2f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float detectRange = 8f;
        [SerializeField] private float moveSpeed = 3f;

        [Header("Patrol")]
        [SerializeField] private float patrolRadius = 5f;
        [SerializeField] private float patrolWaitTime = 2f;

        [Header("Attack")]
        [SerializeField] private float attackCooldown = 1.5f;

        [Header("EXP")]
        [SerializeField] private int expReward = 25;

        // State
        private enum AIState { Idle, Patrol, Chase, Attack, Dead }
        private AIState currentState = AIState.Idle;
        private float currentHP;
        private bool isDead = false;

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
            // Mulai patrol
            currentState = AIState.Patrol;
            SetNewPatrolTarget();
        }

        void Update()
        {
            if (isDead) return;

            // Cek player di sekitar
            DetectPlayer();

            // Update state
            switch (currentState)
            {
                case AIState.Idle:
                    UpdateIdle();
                    break;
                case AIState.Patrol:
                    UpdatePatrol();
                    break;
                case AIState.Chase:
                    UpdateChase();
                    break;
                case AIState.Attack:
                    UpdateAttack();
                    break;
            }

            // Update attack timer
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Deteksi player di sekitar monster.
        /// </summary>
        private void DetectPlayer()
        {
            // Cari player dengan tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                // Jika tidak ada player tag, cari dengan nama
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

                // Jika dalam range attack, attack
                if (distance <= attackRange)
                {
                    currentState = AIState.Attack;
                }
                // Jika dalam range detect tapi belum attack range, chase
                else if (currentState != AIState.Attack)
                {
                    currentState = AIState.Chase;
                }
            }
            else
            {
                playerTarget = null;

                // Kembali ke patrol jika player keluar range
                if (currentState == AIState.Chase || currentState == AIState.Attack)
                {
                    currentState = AIState.Patrol;
                    SetNewPatrolTarget();
                }
            }
        }

        /// <summary>
        /// Update state Idle.
        /// </summary>
        private void UpdateIdle()
        {
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0)
            {
                currentState = AIState.Patrol;
                SetNewPatrolTarget();
            }
        }

        /// <summary>
        /// Update state Patrol.
        /// </summary>
        private void UpdatePatrol()
        {
            // Bergerak ke patrol target
            Vector3 direction = (patrolTarget - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Face direction
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // Cek sudah sampai
            float distance = Vector3.Distance(transform.position, patrolTarget);
            if (distance < 0.5f)
            {
                // Tunggu sebelum patrol berikutnya
                currentState = AIState.Idle;
                patrolTimer = patrolWaitTime;
            }
        }

        /// <summary>
        /// Update state Chase.
        /// </summary>
        private void UpdateChase()
        {
            if (playerTarget == null) return;

            // Bergerak ke player
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Face player
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        /// <summary>
        /// Update state Attack.
        /// </summary>
        private void UpdateAttack()
        {
            if (playerTarget == null) return;

            // Face player
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // Attack jika cooldown selesai
            if (attackTimer <= 0)
            {
                PerformAttack();
                attackTimer = attackCooldown;
            }
        }

        /// <summary>
        /// Eksekusi serangan ke player.
        /// </summary>
        private void PerformAttack()
        {
            if (playerTarget == null) return;

            // Hitung damage
            float damage = attack * Random.Range(0.9f, 1.1f);
            damage = Mathf.Floor(damage);

            // Kirim damage ke player (jika ada script HP)
            // Untuk sekarang, spawn damage popup ke player
            if (DamagePopupSpawner.Instance != null)
            {
                DamagePopupSpawner.Instance.SpawnDamagePopup(
                    playerTarget.position,
                    damage,
                    false,
                    playerTarget
                );
            }

            // Play attack sound
            if (JobSFXManager.Instance != null)
            {
                JobSFXManager.Instance.PlayAttack("male");
            }

            Debug.Log($"[Monster] Attack player! Damage: {damage}");
        }

        /// <summary>
        /// Set patrol target baru di sekitar spawn point.
        /// </summary>
        private void SetNewPatrolTarget()
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            patrolTarget = spawnPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
        }

        /// <summary>
        /// Terima damage dari player.
        /// </summary>
        public void TakeDamage(float rawDamage, bool isCritical = false)
        {
            if (isDead) return;

            // Trigger battle BGM
            if (BattleBGMManager.Instance != null)
            {
                BattleBGMManager.Instance.EnterBattle();
            }

            // Hitung damage dengan defense
            float damage = Mathf.Max(1, rawDamage - defense);
            damage *= Random.Range(0.9f, 1.1f);

            // Critical multiplier
            if (isCritical)
            {
                damage *= 1.5f;
            }

            damage = Mathf.Floor(damage);
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

        /// <summary>
        /// Efek flash saat kena hit.
        /// </summary>
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

        /// <summary>
        /// Monster mati.
        /// </summary>
        private void Die()
        {
            isDead = true;
            currentState = AIState.Dead;

            // Spawn death effect
            SimpleVFXCreator.CreateDeathEffect().transform.position = transform.position + Vector3.up * 0.5f;

            // Beri EXP ke player
            GiveEXPToPlayer();

            // Exit battle BGM
            if (BattleBGMManager.Instance != null)
            {
                BattleBGMManager.Instance.ExitBattle();
            }

            Debug.Log($"[Monster] MATI! EXP: {expReward}");

            // Disable collider dan renderer
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Destroy setelah delay
            Destroy(gameObject, 2f);
        }

        /// <summary>
        /// Beri EXP ke player.
        /// </summary>
        private void GiveEXPToPlayer()
        {
            // Cari player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                player = GameObject.Find("Player");
            }

            if (player != null)
            {
                // Cek apakah player punya script EXP
                // Untuk sekarang, spawn popup EXP di player
                if (DamagePopupSpawner.Instance != null)
                {
                    DamagePopupSpawner.Instance.SpawnDamagePopup(
                        player.transform.position,
                        expReward,
                        false,
                        player.transform
                    );
                }

                Debug.Log($"[EXP] Player mendapat {expReward} EXP");
            }
        }

        /// <summary>
        /// Draw gizmos untuk debug.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Detect range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRange);

            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Patrol radius
            Gizmos.color = Color.blue;
            Vector3 center = Application.isPlaying ? spawnPosition : transform.position;
            Gizmos.DrawWireSphere(center, patrolRadius);
        }
    }
}
