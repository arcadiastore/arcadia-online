using UnityEngine;
using ArcadiaOnline.Player;
using ArcadiaOnline.Managers;

namespace ArcadiaOnline.Monster
{
    /// <summary>
    /// Monster AI sederhana: Patrol → Chase → Attack. Bisa di-klik untuk diserang.
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
            currentState = AIState.Patrol;
            SetNewPatrolTarget();
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
                    break;
                case AIState.Chase:
                    UpdateChase();
                    break;
                case AIState.Attack:
                    UpdateAttack();
                    break;
            }

            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
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

            // Cari player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                player = GameObject.Find("Player");
            }

            if (player == null) return;

            // Ambil damage dari player
            float rawDamage = 15f; // Default
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                rawDamage = playerStats.ATK;
            }

            // Hitung critical (10% chance)
            bool isCritical = Random.Range(0f, 1f) < 0.1f;

            // Terima damage
            TakeDamage(rawDamage, isCritical);

            // Play attack sound
            if (JobSFXManager.Instance != null)
            {
                JobSFXManager.Instance.PlayAttack("male");
            }
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
