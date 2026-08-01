using UnityEngine;

namespace ArcadiaOnline.Monster
{
    /// <summary>
    /// Spawn monster di area untuk testing.
    /// </summary>
    public class MonsterSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private int maxMonsters = 5;
        [SerializeField] private float spawnRadius = 20f;
        [SerializeField] private float spawnInterval = 5f;

        private int currentMonsterCount = 0;
        private float spawnTimer;

        void Start()
        {
            for (int i = 0; i < maxMonsters; i++)
            {
                SpawnMonster();
            }
        }

        void Update()
        {
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

        private void SpawnMonster()
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            CreateSimpleMonster(spawnPos);
        }

        private void CreateSimpleMonster(Vector3 position)
        {
            GameObject monster = new GameObject("Monster_" + currentMonsterCount);
            monster.transform.position = position;

            // Body (capsule)
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(monster.transform);
            body.transform.localPosition = Vector3.up * 1f;
            body.transform.localScale = new Vector3(0.8f, 1f, 0.8f);

            // Warna random (merah)
            Renderer bodyRenderer = body.GetComponent<Renderer>();
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = new Color(
                    Random.Range(0.5f, 0.8f),
                    Random.Range(0.1f, 0.2f),
                    Random.Range(0.1f, 0.2f)
                );
            }

            // Collider
            BoxCollider col = monster.AddComponent<BoxCollider>();
            col.size = new Vector3(0.8f, 2f, 0.8f);
            col.center = Vector3.up * 1f;

            // Rigidbody
            Rigidbody rb = monster.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            // AI
            SimpleMonsterAI ai = monster.AddComponent<SimpleMonsterAI>();

            currentMonsterCount++;

            Debug.Log($"[Spawner] Monster spawned at {position}");
        }

        public void OnMonsterDeath()
        {
            currentMonsterCount--;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
