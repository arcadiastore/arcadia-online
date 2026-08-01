using UnityEngine;

namespace ArcadiaOnline.Monster
{
    /// <summary>
    /// Spawn monster di sekitar area untuk testing.
    /// Attach ke empty GameObject di scene.
    /// </summary>
    public class MonsterSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject monsterPrefab;
        [SerializeField] private int maxMonsters = 5;
        [SerializeField] private float spawnRadius = 20f;
        [SerializeField] private float spawnInterval = 5f;

        [Header("Monster Settings")]
        [SerializeField] private float minHP = 50f;
        [SerializeField] private float maxHP = 150f;
        [SerializeField] private float minAttack = 5f;
        [SerializeField] private float maxAttack = 15f;

        private int currentMonsterCount = 0;
        private float spawnTimer;

        void Start()
        {
            // Spawn monster awal
            for (int i = 0; i < maxMonsters; i++)
            {
                SpawnMonster();
            }
        }

        void Update()
        {
            // Spawn monster baru jika kurang dari max
            if (currentMonsterCount < maxMonsters)
            {
                spawnTimer -= Time.deltaTime;
                if (spawnTimer <= 0)
                {
                    SpawnMonster();
                    spawnTimer = spawnInterval;
                }
            }
        }

        /// <summary>
        /// Spawn monster di posisi random.
        /// </summary>
        private void SpawnMonster()
        {
            // Hitung posisi random
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            // Jika ada prefab, instantiate prefab
            if (monsterPrefab != null)
            {
                GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                monster.name = "Monster_" + currentMonsterCount;

                // Setup stats random
                SimpleMonsterAI ai = monster.GetComponent<SimpleMonsterAI>();
                if (ai != null)
                {
                    // Stats akan di-setup via SerializeField
                }

                currentMonsterCount++;
            }
            // Jika tidak ada prefab, buat monster sederhana
            else
            {
                CreateSimpleMonster(spawnPos);
            }
        }

        /// <summary>
        /// Buat monster sederhana tanpa prefab.
        /// </summary>
        private void CreateSimpleMonster(Vector3 position)
        {
            // Buat GameObject
            GameObject monster = new GameObject("Monster_" + currentMonsterCount);
            monster.transform.position = position;
            monster.tag = "Untagged"; // Ganti dengan tag monster jika ada

            // Buat body (capsule)
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(monster.transform);
            body.transform.localPosition = Vector3.up * 1f;
            body.transform.localScale = new Vector3(0.8f, 1f, 0.8f);

            // Set warna random
            Renderer bodyRenderer = body.GetComponent<Renderer>();
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = new Color(
                    Random.Range(0.3f, 0.7f),
                    Random.Range(0.1f, 0.3f),
                    Random.Range(0.1f, 0.3f)
                );
            }

            // Hapus collider capsule, tambah box collider ke parent
            Destroy(body.GetComponent<Collider>());

            BoxCollider col = monster.AddComponent<BoxCollider>();
            col.size = new Vector3(0.8f, 2f, 0.8f);
            col.center = Vector3.up * 1f;

            // Tambah Rigidbody
            Rigidbody rb = monster.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            // Tambah SimpleMonsterAI
            SimpleMonsterAI ai = monster.AddComponent<SimpleMonsterAI>();

            currentMonsterCount++;
        }

        /// <summary>
        /// Monster mati, kurangi count.
        /// </summary>
        public void OnMonsterDeath()
        {
            currentMonsterCount--;
        }

        /// <summary>
        /// Draw gizmos untuk spawn area.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
