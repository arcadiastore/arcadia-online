using UnityEngine;

namespace ArcadiaOnline.Monster
{
    /// <summary>
    /// Monster test dummy - tidak bergerak, hanya untuk test combat.
    /// Attach ke GameObject 3D (Capsule/Cube).
    /// </summary>
    public class TestDummy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float maxHP = 100f;
        [SerializeField] private float defense = 5f;

        [Header("Respawn")]
        [SerializeField] private bool respawn = true;
        [SerializeField] private float respawnTime = 5f;

        [Header("Visual")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitFlashDuration = 0.2f;

        private float currentHP;
        private Renderer meshRenderer;
        private Vector3 originalPosition;
        private bool isDead = false;
        private float hitFlashTimer = 0f;

        public float CurrentHP => currentHP;
        public float MaxHP => maxHP;
        public bool IsDead => isDead;

        void Awake()
        {
            currentHP = maxHP;
            originalPosition = transform.position;
            meshRenderer = GetComponentInChildren<Renderer>();

            if (meshRenderer != null)
            {
                try
                {
                    normalColor = meshRenderer.material.color;
                }
                catch
                {
                    // Material tidak punya _Color property, pakai default
                    normalColor = Color.white;
                }
            }
        }

        void Update()
        {
            // Hit flash effect
            if (hitFlashTimer > 0)
            {
                hitFlashTimer -= Time.deltaTime;
                if (hitFlashTimer <= 0 && meshRenderer != null)
                {
                    meshRenderer.material.color = normalColor;
                }
            }
        }

        /// <summary>
        /// Terima damage dari player.
        /// </summary>
        public void TakeDamage(float rawDamage)
        {
            if (isDead) return;

            // Hitung damage dengan defense
            float damage = Mathf.Max(1, rawDamage - defense);
            damage *= Random.Range(0.9f, 1.1f); // Random ±10%
            damage = Mathf.Floor(damage);

            currentHP = Mathf.Max(0, currentHP - damage);

            // Visual feedback
            HitFlash();
            SpawnDamagePopup(damage);

            Debug.Log($"[Dummy] Terkena {damage} damage! HP: {currentHP}/{maxHP}");

            if (currentHP <= 0)
            {
                Die();
            }
        }

        private void HitFlash()
        {
            if (meshRenderer != null)
            {
                try
                {
                    meshRenderer.material.color = hitColor;
                }
                catch
                {
                    // Material tidak support _Color
                }
                hitFlashTimer = hitFlashDuration;
            }
        }

        private void SpawnDamagePopup(float damage)
        {
            // Simple debug output - nanti di-upgrade jadi world space UI
            Debug.Log($"[DamagePopup] {damage}");

            // Shake effect saat kena damage
            StartCoroutine(HitShake());
        }

        private System.Collections.IEnumerator HitShake()
        {
            Vector3 originalPos = transform.position;
            float duration = 0.2f;
            float magnitude = 0.1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-magnitude, magnitude);
                float z = Random.Range(-magnitude, magnitude);
                transform.position = originalPos + new Vector3(x, 0, z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = originalPos;
        }

        private void Die()
        {
            isDead = true;
            Debug.Log("[Dummy] MATI!");

            // Efek mati: scale down ke 0
            StartCoroutine(DeathEffect());
        }

        private System.Collections.IEnumerator DeathEffect()
        {
            float duration = 0.5f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;
            Vector3 startPos = transform.position;

            // Turun ke bawah sambil mengecil
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Scale down
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

                // Turun sedikit
                transform.position = Vector3.Lerp(startPos, startPos + Vector3.down * 0.5f, t);

                yield return null;
            }

            // Sembunyikan (disable renderer & collider, tapi tetap aktif)
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            if (respawn)
            {
                yield return new WaitForSeconds(respawnTime);
                Respawn();
            }
        }

        private void Respawn()
        {
            currentHP = maxHP;
            isDead = false;

            // Reset position & scale
            transform.position = originalPosition;
            transform.localScale = Vector3.one * 2f;

            // Tampilkan kembali
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            Debug.Log("[Dummy] Respawn!");
        }

        // Interaksi dengan player (click/interact)
        private void OnMouseDown()
        {
            // Test: player attack dummy saat di-klik
            if (!isDead)
            {
                float playerATK = 20f; // Default ATK untuk test
                var playerStats = FindAnyObjectByType<Player.PlayerStats>();
                if (playerStats != null && playerStats.BaseStats.atk > 0)
                {
                    playerATK = playerStats.BaseStats.atk;
                }
                TakeDamage(playerATK);
            }
        }
    }
}
