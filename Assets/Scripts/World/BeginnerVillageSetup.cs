using UnityEngine;
using System.Collections.Generic;
using ArcadiaOnline.Shop;
using ArcadiaOnline.Quest;
using ArcadiaOnline.Dialogue;
using ArcadiaOnline.Monster;

namespace ArcadiaOnline.World
{
    /// <summary>
    /// Setup untuk Beginner Village - Vertical Slice pertama.
    /// Attach ke GameObject di scene.
    /// </summary>
    public class BeginnerVillageSetup : MonoBehaviour
    {
        [Header("Auto-Create")]
        [SerializeField] private bool createOnStart = true;

        [Header("Map Settings")]
        [SerializeField] private float villageSize = 100f;
        [SerializeField] private int monsterCount = 10;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        // Village areas
        private GameObject villageCenter;
        private GameObject trainingGround;
        private GameObject forestEntrance;

        void Start()
        {
            if (createOnStart)
            {
                CreateBeginnerVillage();
            }
        }

        /// <summary>
        /// Create entire Beginner Village.
        /// </summary>
        public void CreateBeginnerVillage()
        {
            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Creating Beginner Village...");
            }

            // Create village areas
            CreateVillageCenter();
            CreateTrainingGround();
            CreateForestEntrance();

            // Create NPCs
            CreateVillageChief();
            CreateShopkeeper();
            CreateBlacksmith();
            CreateElder();

            // Create monsters
            CreateSlimes();
            CreateWolves();
            CreateAlphaWolf();

            // Create warp points
            CreateWarpPoints();

            // Create environment
            CreateEnvironment();

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Beginner Village created!");
                Debug.Log("[BeginnerVillage] Areas: Village Center, Training Ground, Forest Entrance");
                Debug.Log("[BeginnerVillage] NPCs: Village Chief, Shopkeeper, Blacksmith, Elder");
                Debug.Log("[BeginnerVillage] Monsters: Slime (Lv.1), Wolf (Lv.3), Alpha Wolf (Lv.5)");
            }
        }

        /// <summary>
        /// Create Village Center area.
        /// </summary>
        private void CreateVillageCenter()
        {
            villageCenter = new GameObject("VillageCenter");
            villageCenter.transform.position = Vector3.zero;

            // Ground plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "VillageGround";
            ground.transform.SetParent(villageCenter.transform);
            ground.transform.localPosition = Vector3.zero;
            ground.transform.localScale = new Vector3(5, 1, 5); // 50x50 meters
            ground.GetComponent<Renderer>().material.color = new Color(0.4f, 0.6f, 0.3f); // Green grass

            // Remove collider from ground (player can walk through)
            Destroy(ground.GetComponent<Collider>());

            // Add boundary walls
            CreateBoundaryWalls(villageCenter.transform, 50f);

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created Village Center");
            }
        }

        /// <summary>
        /// Create Training Ground area.
        /// </summary>
        private void CreateTrainingGround()
        {
            trainingGround = new GameObject("TrainingGround");
            trainingGround.transform.position = new Vector3(60, 0, 0);

            // Ground plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "TrainingGround";
            ground.transform.SetParent(trainingGround.transform);
            ground.transform.localPosition = Vector3.zero;
            ground.transform.localScale = new Vector3(3, 1, 3); // 30x30 meters
            ground.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.4f); // Sandy

            // Remove collider
            Destroy(ground.GetComponent<Collider>());

            // Add training dummies
            CreateTrainingDummies(trainingGround.transform);

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created Training Ground");
            }
        }

        /// <summary>
        /// Create Forest Entrance area.
        /// </summary>
        private void CreateForestEntrance()
        {
            forestEntrance = new GameObject("ForestEntrance");
            forestEntrance.transform.position = new Vector3(-60, 0, 0);

            // Ground plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "ForestGround";
            ground.transform.SetParent(forestEntrance.transform);
            ground.transform.localPosition = Vector3.zero;
            ground.transform.localScale = new Vector3(4, 1, 4); // 40x40 meters
            ground.GetComponent<Renderer>().material.color = new Color(0.3f, 0.5f, 0.2f); // Dark green

            // Remove collider
            Destroy(ground.GetComponent<Collider>());

            // Add trees
            CreateTrees(forestEntrance.transform, 20);

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created Forest Entrance");
            }
        }

        /// <summary>
        /// Create Village Chief NPC.
        /// </summary>
        private void CreateVillageChief()
        {
            GameObject npc = CreateNPC("VillageChief", new Vector3(5, 0, 5), Color.blue);

            // Add dialogue
            DialogueTrigger dialogueTrigger = npc.AddComponent<DialogueTrigger>();

            // Add quest giver
            QuestGiver questGiver = npc.AddComponent<QuestGiver>();

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created Village Chief (Quest Giver)");
            }
        }

        /// <summary>
        /// Create Shopkeeper NPC.
        /// </summary>
        private void CreateShopkeeper()
        {
            GameObject npc = CreateNPC("Shopkeeper", new Vector3(-5, 0, 5), Color.green);

            // Add shop trigger
            ShopTrigger shopTrigger = npc.AddComponent<ShopTrigger>();
            shopTrigger.SetShopID("general_shop");

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created Shopkeeper (General Store)");
            }
        }

        /// <summary>
        /// Create Blacksmith NPC.
        /// </summary>
        private void CreateBlacksmith()
        {
            GameObject npc = CreateNPC("Blacksmith", new Vector3(-10, 0, 0), Color.red);

            // Add weapon shop
            ShopTrigger shopTrigger = npc.AddComponent<ShopTrigger>();
            shopTrigger.SetShopID("weapon_shop");

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created Blacksmith (Weapon Shop)");
            }
        }

        /// <summary>
        /// Create Elder NPC.
        /// </summary>
        private void CreateElder()
        {
            GameObject npc = CreateNPC("Elder", new Vector3(0, 0, -10), Color.yellow);

            // Add dialogue
            DialogueTrigger dialogueTrigger = npc.AddComponent<DialogueTrigger>();

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created Elder (Dialogue)");
            }
        }

        /// <summary>
        /// Create Slime monsters.
        /// </summary>
        private void CreateSlimes()
        {
            // Spawn slimes in training ground
            for (int i = 0; i < 5; i++)
            {
                Vector3 pos = new Vector3(
                    60 + Random.Range(-10f, 10f),
                    0.5f,
                    Random.Range(-10f, 10f)
                );

                GameObject slime = CreateMonster("Slime", pos, Color.green, 1);
                slime.tag = "Enemy";
            }

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created 5 Slimes (Lv.1)");
            }
        }

        /// <summary>
        /// Create Wolf monsters.
        /// </summary>
        private void CreateWolves()
        {
            // Spawn wolves in forest
            for (int i = 0; i < 3; i++)
            {
                Vector3 pos = new Vector3(
                    -60 + Random.Range(-15f, 15f),
                    0.5f,
                    Random.Range(-15f, 15f)
                );

                GameObject wolf = CreateMonster("Wolf", pos, Color.gray, 3);
                wolf.tag = "Enemy";
            }

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created 3 Wolves (Lv.3)");
            }
        }

        /// <summary>
        /// Create Alpha Wolf boss.
        /// </summary>
        private void CreateAlphaWolf()
        {
            Vector3 pos = new Vector3(-80, 0.5f, -20);
            GameObject alphaWolf = CreateMonster("AlphaWolf", pos, Color.black, 5);
            alphaWolf.tag = "Enemy";
            alphaWolf.transform.localScale = Vector3.one * 2f; // Bigger

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created Alpha Wolf (Lv.5 Boss)");
            }
        }

        /// <summary>
        /// Create warp points.
        /// </summary>
        private void CreateWarpPoints()
        {
            // Warp to training ground
            CreateWarpPoint("WarpToTraining", new Vector3(25, 0, 0), new Vector3(55, 0, 0));

            // Warp to forest
            CreateWarpPoint("WarpToForest", new Vector3(-25, 0, 0), new Vector3(-55, 0, 0));

            // Warp back to village
            CreateWarpPoint("WarpToVillage1", new Vector3(65, 0, 0), new Vector3(25, 0, 0));
            CreateWarpPoint("WarpToVillage2", new Vector3(-65, 0, 0), new Vector3(-25, 0, 0));

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created 4 Warp Points");
            }
        }

        /// <summary>
        /// Create environment objects.
        /// </summary>
        private void CreateEnvironment()
        {
            // Village buildings
            CreateBuilding("House1", new Vector3(15, 0, 15), new Vector3(4, 3, 4));
            CreateBuilding("House2", new Vector3(-15, 0, 15), new Vector3(4, 3, 4));
            CreateBuilding("Inn", new Vector3(0, 0, 20), new Vector3(6, 4, 5));
            CreateBuilding("Shop", new Vector3(-10, 0, 10), new Vector3(5, 3, 4));

            // Decorations
            CreateFountain(new Vector3(0, 0, 0));
            CreateBenches(5);

            if (showDebug)
            {
                Debug.Log("[BeginnerVillage] Created environment objects");
            }
        }

        // === HELPER METHODS ===

        /// <summary>
        /// Create NPC GameObject.
        /// </summary>
        private GameObject CreateNPC(string name, Vector3 position, Color color)
        {
            GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = name;
            npc.transform.position = position;
            npc.GetComponent<Renderer>().material.color = color;

            // Add name label
            CreateNameLabel(npc, name);

            return npc;
        }

        /// <summary>
        /// Create monster GameObject.
        /// </summary>
        private GameObject CreateMonster(string name, Vector3 position, Color color, int level)
        {
            GameObject monster = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            monster.name = $"{name}_Lv{level}";
            monster.transform.position = position;
            monster.GetComponent<Renderer>().material.color = color;

            // Add SimpleMonsterAI
            SimpleMonsterAI ai = monster.AddComponent<SimpleMonsterAI>();

            return monster;
        }

        /// <summary>
        /// Create warp point.
        /// </summary>
        private void CreateWarpPoint(string name, Vector3 position, Vector3 destination)
        {
            GameObject warp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            warp.name = name;
            warp.transform.position = position;
            warp.transform.localScale = new Vector3(2, 0.1f, 2);
            warp.GetComponent<Renderer>().material.color = Color.cyan;

            // Add trigger collider
            BoxCollider col = warp.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(2, 2, 2);

            // Add warp script (will be created separately)
            // WarpTrigger warpTrigger = warp.AddComponent<WarpTrigger>();
            // warpTrigger.SetDestination(destination);
        }

        /// <summary>
        /// Create building.
        /// </summary>
        private void CreateBuilding(string name, Vector3 position, Vector3 size)
        {
            GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            building.name = name;
            building.transform.position = position + Vector3.up * (size.y / 2);
            building.transform.localScale = size;
            building.GetComponent<Renderer>().material.color = new Color(0.6f, 0.4f, 0.2f); // Brown

            // Add roof
            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = $"{name}_Roof";
            roof.transform.SetParent(building.transform);
            roof.transform.localPosition = new Vector3(0, 0.6f, 0);
            roof.transform.localScale = new Vector3(1.2f, 0.2f, 1.2f);
            roof.GetComponent<Renderer>().material.color = new Color(0.5f, 0.2f, 0.1f); // Dark red
        }

        /// <summary>
        /// Create fountain.
        /// </summary>
        private void CreateFountain(Vector3 position)
        {
            // Base
            GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseObj.name = "Fountain_Base";
            baseObj.transform.position = position;
            baseObj.transform.localScale = new Vector3(3, 0.5f, 3);
            baseObj.GetComponent<Renderer>().material.color = Color.gray;

            // Water
            GameObject water = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            water.name = "Fountain_Water";
            water.transform.SetParent(baseObj.transform);
            water.transform.localPosition = new Vector3(0, 0.5f, 0);
            water.transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);
            water.GetComponent<Renderer>().material.color = new Color(0.2f, 0.5f, 0.8f); // Blue

            // Pillar
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = "Fountain_Pillar";
            pillar.transform.SetParent(baseObj.transform);
            pillar.transform.localPosition = new Vector3(0, 1f, 0);
            pillar.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
            pillar.GetComponent<Renderer>().material.color = Color.gray;
        }

        /// <summary>
        /// Create benches.
        /// </summary>
        private void CreateBenches(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-20f, 20f),
                    0.25f,
                    Random.Range(-20f, 20f)
                );

                GameObject bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bench.name = $"Bench_{i}";
                bench.transform.position = pos;
                bench.transform.localScale = new Vector3(2, 0.5f, 0.5f);
                bench.GetComponent<Renderer>().material.color = new Color(0.4f, 0.3f, 0.2f); // Wood
            }
        }

        /// <summary>
        /// Create name label.
        /// </summary>
        private void CreateNameLabel(GameObject parent, string text)
        {
            // Create Canvas
            GameObject canvasObj = new GameObject($"{parent.name}_Label");
            canvasObj.transform.SetParent(parent.transform);
            canvasObj.transform.localPosition = Vector3.up * 2.5f;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<RectTransform>().sizeDelta = new Vector2(200, 50);

            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(canvasObj.transform);
            textObj.transform.localPosition = Vector3.zero;

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            UnityEngine.UI.Text textComp = textObj.AddComponent<UnityEngine.UI.Text>();
            textComp.text = text;
            textComp.fontSize = 24;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = Color.white;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        /// <summary>
        /// Create training dummies.
        /// </summary>
        private void CreateTrainingDummies(Transform parent)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-8f, 8f),
                    1f,
                    Random.Range(-8f, 8f)
                );

                GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                dummy.name = $"TrainingDummy_{i}";
                dummy.transform.SetParent(parent);
                dummy.transform.localPosition = pos;
                dummy.GetComponent<Renderer>().material.color = Color.white;

                // Add TestDummy script
                dummy.AddComponent<TestDummy>();
            }
        }

        /// <summary>
        /// Create boundary walls.
        /// </summary>
        private void CreateBoundaryWalls(Transform parent, float size)
        {
            // North wall
            CreateWall(parent, "Wall_North", new Vector3(0, 2.5f, size), new Vector3(size * 2, 5, 0.5f));
            // South wall
            CreateWall(parent, "Wall_South", new Vector3(0, 2.5f, -size), new Vector3(size * 2, 5, 0.5f));
            // East wall
            CreateWall(parent, "Wall_East", new Vector3(size, 2.5f, 0), new Vector3(0.5f, 5, size * 2));
            // West wall
            CreateWall(parent, "Wall_West", new Vector3(-size, 2.5f, 0), new Vector3(0.5f, 5, size * 2));
        }

        /// <summary>
        /// Create wall.
        /// </summary>
        private void CreateWall(Transform parent, string name, Vector3 position, Vector3 size)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.localPosition = position;
            wall.transform.localScale = size;
            wall.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f); // Gray

            // Make walls invisible but solid
            Renderer rend = wall.GetComponent<Renderer>();
            rend.enabled = false; // Invisible but collider works
        }

        /// <summary>
        /// Create trees.
        /// </summary>
        private void CreateTrees(Transform parent, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-15f, 15f),
                    0,
                    Random.Range(-15f, 15f)
                );

                // Trunk
                GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                trunk.name = $"Tree_Trunk_{i}";
                trunk.transform.SetParent(parent);
                trunk.transform.localPosition = pos + Vector3.up * 2f;
                trunk.transform.localScale = new Vector3(0.3f, 2f, 0.3f);
                trunk.GetComponent<Renderer>().material.color = new Color(0.4f, 0.3f, 0.2f); // Brown

                // Leaves
                GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                leaves.name = $"Tree_Leaves_{i}";
                leaves.transform.SetParent(trunk.transform);
                leaves.transform.localPosition = Vector3.up * 1.5f;
                leaves.transform.localScale = new Vector3(2f, 2f, 2f);
                leaves.GetComponent<Renderer>().material.color = new Color(0.1f, 0.5f, 0.1f); // Dark green
            }
        }
    }
}
