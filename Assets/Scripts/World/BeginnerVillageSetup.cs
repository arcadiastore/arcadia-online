using UnityEngine;
using System.Collections.Generic;
using ArcadiaOnline.Quest;
using ArcadiaOnline.Dialogue;
using ArcadiaOnline.Shop;
using ArcadiaOnline.Monster;

namespace ArcadiaOnline.World
{
    /// <summary>
    /// Beginner Village sesuai GDD.
    /// Level 1-10, tutorial area, quest "Permintaan Tetua".
    /// </summary>
    public class BeginnerVillageSetup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool autoCreateOnStart = true;
        [SerializeField] private bool showDebug = true;

        // Positions (2x bigger map)
        private Vector3 villageCenter = Vector3.zero;
        private Vector3 trainingGround = new Vector3(100, 0, 0);
        private Vector3 forestEntrance = new Vector3(-100, 0, 0);

        // Parents
        private Transform envParent;
        private Transform structParent;
        private Transform npcParent;
        private Transform monsterParent;

        void Start()
        {
            if (autoCreateOnStart) CreateBeginnerVillage();
        }

        public void CreateBeginnerVillage()
        {
            if (showDebug) Debug.Log("[Village] Creating Beginner Village (GDD compliant)...");

            // Parents
            envParent = new GameObject("Environment").transform;
            structParent = new GameObject("Structures").transform;
            npcParent = new GameObject("NPCs").transform;
            monsterParent = new GameObject("Monsters").transform;

            // === GROUND & ROADS ===
            CreateGroundAndRoads();

            // === BUILDINGS (GDD: Village, Blacksmith, Shop, Elder House) ===
            CreateBuildings();

            // === DECORATIONS ===
            CreateDecorations();

            // === NATURE ===
            CreateNature();

            // === NPCs (GDD: Elder Tetua, Blacksmith, Merchant, Guard) ===
            CreateNPCs();

            // === QUESTS (GDD: "Permintaan Tetua") ===
            CreateQuests();

            // === MONSTERS (GDD: Slime Lv1-3, Wolf Lv5-8) ===
            CreateMonsters();

            // === TUTORIAL AREA ===
            CreateTutorialArea();

            // === WARP POINTS (unlock after quest) ===
            CreateWarpPoints();

            // === QUEST GATE to Green Forest ===
            CreateForestGate();

            if (showDebug) Debug.Log("[Village] === BEGINNER VILLAGE COMPLETE (GDD) ===");
        }

        #region Ground & Roads

        private void CreateGroundAndRoads()
        {
            // Main ground (400x400)
            MakeGround("OuterGrass", Vector3.zero, new Vector3(400, 0.05f, 400), new Color(0.35f, 0.55f, 0.30f));
            
            // Village plaza
            MakeGround("VillagePlaza", villageCenter, new Vector3(120, 0.10f, 120), new Color(0.55f, 0.45f, 0.35f));
            
            // Training ground (Lv 1-3 area)
            MakeGround("TrainingGround", trainingGround, new Vector3(80, 0.10f, 80), new Color(0.65f, 0.55f, 0.40f));
            
            // Forest edge (Lv 5-8 area)
            MakeGround("ForestEdge", forestEntrance, new Vector3(100, 0.10f, 100), new Color(0.25f, 0.40f, 0.25f));

            // Roads
            MakeRoad(villageCenter + new Vector3(50, 0.12f, 0), new Vector3(80, 0.12f, 10));
            MakeRoad(villageCenter + new Vector3(-50, 0.12f, 0), new Vector3(80, 0.12f, 10));
            MakeRoad(villageCenter, new Vector3(10, 0.12f, 100));
            MakeRoad(villageCenter, new Vector3(100, 0.12f, 10));
            
            // Ring road
            MakeRoad(villageCenter + new Vector3(0, 0.12f, 35), new Vector3(70, 0.12f, 6));
            MakeRoad(villageCenter + new Vector3(0, 0.12f, -35), new Vector3(70, 0.12f, 6));
            MakeRoad(villageCenter + new Vector3(35, 0.12f, 0), new Vector3(6, 0.12f, 70));
            MakeRoad(villageCenter + new Vector3(-35, 0.12f, 0), new Vector3(6, 0.12f, 70));

            if (showDebug) Debug.Log("[Village] Ground & roads created");
        }

        private void MakeGround(string name, Vector3 pos, Vector3 scale, Color color)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = name; g.transform.SetParent(envParent);
            g.transform.position = pos; g.transform.localScale = scale;
            g.GetComponent<Renderer>().material.color = color;
        }

        private void MakeRoad(Vector3 pos, Vector3 scale)
        {
            MakeGround("Road", pos, scale, new Color(0.45f, 0.38f, 0.28f));
        }

        #endregion

        #region Buildings (GDD)

        private void CreateBuildings()
        {
            // === GDD: Elder's House (Main Quest Giver) ===
            MakeBuilding("ElderHouse", villageCenter + new Vector3(0, 0, -50), 
                new Vector3(25, 15, 20), new Color(0.85f, 0.80f, 0.70f));

            // === GDD: Blacksmith (Equipment) ===
            MakeBuilding("Blacksmith", villageCenter + new Vector3(-35, 0, 0), 
                new Vector3(20, 12, 16), new Color(0.50f, 0.35f, 0.30f));

            // === GDD: General Store (Items) ===
            MakeBuilding("GeneralStore", villageCenter + new Vector3(35, 0, 0), 
                new Vector3(20, 12, 16), new Color(0.40f, 0.55f, 0.40f));

            // === GDD: Inn (Save & Rest) ===
            MakeBuilding("Inn", villageCenter + new Vector3(0, 0, 40), 
                new Vector3(22, 14, 18), new Color(0.65f, 0.50f, 0.40f));

            // === GDD: Village Hall (Meeting place) ===
            MakeBuilding("VillageHall", villageCenter + new Vector3(-25, 0, -35), 
                new Vector3(28, 16, 22), new Color(0.70f, 0.60f, 0.50f));

            // === GDD: Training Hall (Tutorial) ===
            MakeBuilding("TrainingHall", trainingGround + new Vector3(0, 0, -30), 
                new Vector3(25, 12, 18), new Color(0.60f, 0.50f, 0.40f));

            // === GDD: Guard Post (Forest Gate) ===
            MakeBuilding("GuardPost", forestEntrance + new Vector3(15, 0, 0), 
                new Vector3(12, 10, 10), new Color(0.55f, 0.45f, 0.35f));

            // Houses (12)
            Vector3[] housePos = {
                new Vector3(25,0,25), new Vector3(-25,0,25), new Vector3(45,0,15), new Vector3(-45,0,15),
                new Vector3(45,0,-15), new Vector3(-45,0,-15), new Vector3(25,0,-25), new Vector3(-25,0,-25),
                new Vector3(55,0,0), new Vector3(-55,0,0), new Vector3(0,0,55), new Vector3(0,0,-55)
            };
            Color[] houseCol = {
                new Color(.80f,.60f,.40f), new Color(.70f,.50f,.30f), new Color(.65f,.45f,.25f), new Color(.85f,.70f,.50f),
                new Color(.75f,.55f,.35f), new Color(.60f,.40f,.20f), new Color(.80f,.65f,.45f), new Color(.55f,.35f,.15f),
                new Color(.78f,.68f,.55f), new Color(.68f,.58f,.45f), new Color(.88f,.78f,.65f), new Color(.58f,.48f,.35f)
            };
            for (int i = 0; i < 12; i++)
                MakeBuilding($"House_{i+1}", villageCenter + housePos[i], new Vector3(14, Random.Range(8f,11f), 12), houseCol[i]);

            if (showDebug) Debug.Log("[Village] Buildings created (GDD: Elder, Blacksmith, Shop, Inn, etc)");
        }

        private void MakeBuilding(string name, Vector3 pos, Vector3 size, Color color)
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = name; body.transform.SetParent(structParent);
            body.transform.position = pos + Vector3.up * (size.y/2);
            body.transform.localScale = size;
            body.GetComponent<Renderer>().material.color = color;

            var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = name+"_Roof"; roof.transform.SetParent(structParent);
            roof.transform.position = pos + Vector3.up * (size.y + 1.5f);
            roof.transform.localScale = new Vector3(size.x+3, 2.5f, size.z+3);
            roof.GetComponent<Renderer>().material.color = new Color(0.50f, 0.20f, 0.15f);

            var door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = name+"_Door"; door.transform.SetParent(structParent);
            door.transform.position = pos + new Vector3(0, 1.5f, size.z/2+0.1f);
            door.transform.localScale = new Vector3(2.5f, 3.5f, 0.2f);
            door.GetComponent<Renderer>().material.color = new Color(0.35f, 0.18f, 0.08f);
            Destroy(door.GetComponent<Collider>());

            MakeWindow(pos + new Vector3(-size.x*0.28f, size.y*0.55f, size.z/2+0.11f));
            MakeWindow(pos + new Vector3(size.x*0.28f, size.y*0.55f, size.z/2+0.11f));

            if (size.y > 12)
            {
                var ch = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ch.name = name+"_Chimney"; ch.transform.SetParent(structParent);
                ch.transform.position = pos + new Vector3(size.x*0.3f, size.y+3, -size.z*0.3f);
                ch.transform.localScale = new Vector3(2, 3, 2);
                ch.GetComponent<Renderer>().material.color = new Color(0.45f, 0.35f, 0.30f);
            }
        }

        private void MakeWindow(Vector3 pos)
        {
            var w = GameObject.CreatePrimitive(PrimitiveType.Cube);
            w.name = "Window"; w.transform.SetParent(structParent);
            w.transform.position = pos;
            w.transform.localScale = new Vector3(2.2f, 2.2f, 0.1f);
            w.GetComponent<Renderer>().material.color = new Color(0.75f, 0.85f, 1f, 0.85f);
            Destroy(w.GetComponent<Collider>());
        }

        #endregion

        #region Decorations

        private void CreateDecorations()
        {
            // Fountain
            MakeFountain();

            // Benches (10)
            Vector3[] benchPos = {
                new Vector3(15,0,0), new Vector3(-15,0,0), new Vector3(0,0,15), new Vector3(0,0,-15),
                new Vector3(10,0,10), new Vector3(-10,0,10), new Vector3(10,0,-10), new Vector3(-10,0,-10),
                new Vector3(20,0,5), new Vector3(-20,0,5)
            };
            foreach (var p in benchPos) MakeBench(villageCenter + p);

            // Lamp posts (16)
            Vector3[] lampPos = {
                new Vector3(10,0,0), new Vector3(-10,0,0), new Vector3(0,0,10), new Vector3(0,0,-10),
                new Vector3(25,0,0), new Vector3(-25,0,0), new Vector3(0,0,25), new Vector3(0,0,-25),
                new Vector3(35,0,0), new Vector3(-35,0,0), new Vector3(0,0,35), new Vector3(0,0,-35),
                new Vector3(20,0,20), new Vector3(-20,0,20), new Vector3(20,0,-20), new Vector3(-20,0,-20)
            };
            foreach (var p in lampPos) MakeLampPost(villageCenter + p);

            // Fences
            for (int a = 0; a < 360; a += 12)
            {
                float rad = a * Mathf.Deg2Rad;
                MakeFence(villageCenter + new Vector3(Mathf.Cos(rad)*55, 0, Mathf.Sin(rad)*55), a);
            }

            // Market stalls (3)
            MakeStall(villageCenter + new Vector3(20,0,20), new Color(.8f,.2f,.2f));
            MakeStall(villageCenter + new Vector3(-20,0,20), new Color(.2f,.2f,.8f));
            MakeStall(villageCenter + new Vector3(0,0,25), new Color(.8f,.8f,.2f));

            if (showDebug) Debug.Log("[Village] Decorations created");
        }

        private void MakeFountain()
        {
            var b = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            b.name = "FountainBase"; b.transform.SetParent(envParent);
            b.transform.position = villageCenter;
            b.transform.localScale = new Vector3(12, 2, 12);
            b.GetComponent<Renderer>().material.color = new Color(0.70f, 0.68f, 0.65f);

            var p = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            p.name = "FountainPillar"; p.transform.SetParent(envParent);
            p.transform.position = villageCenter + Vector3.up * 3.5f;
            p.transform.localScale = new Vector3(2, 4, 2);
            p.GetComponent<Renderer>().material.color = new Color(0.60f, 0.58f, 0.55f);

            var w = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            w.name = "FountainWater"; w.transform.SetParent(envParent);
            w.transform.position = villageCenter + Vector3.up * 5.5f;
            w.transform.localScale = new Vector3(7, 2, 7);
            w.GetComponent<Renderer>().material.color = new Color(0.15f, 0.45f, 0.75f, 0.65f);
            Destroy(w.GetComponent<Collider>());
        }

        private void MakeBench(Vector3 pos)
        {
            var seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.name = "Bench"; seat.transform.SetParent(envParent);
            seat.transform.position = pos + Vector3.up * 0.6f;
            seat.transform.localScale = new Vector3(3, 0.3f, 1);
            seat.GetComponent<Renderer>().material.color = new Color(0.55f, 0.35f, 0.20f);
        }

        private void MakeLampPost(Vector3 pos)
        {
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "LampPost"; pole.transform.SetParent(envParent);
            pole.transform.position = pos + Vector3.up * 3;
            pole.transform.localScale = new Vector3(0.4f, 6, 0.4f);
            pole.GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 0.25f);

            var lamp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lamp.name = "LampLight"; lamp.transform.SetParent(envParent);
            lamp.transform.position = pos + Vector3.up * 6.5f;
            lamp.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            lamp.GetComponent<Renderer>().material.color = new Color(1f, 0.90f, 0.60f);
            Destroy(lamp.GetComponent<Collider>());

            var lo = new GameObject("Light");
            lo.transform.SetParent(envParent);
            lo.transform.position = pos + Vector3.up * 6.5f;
            var l = lo.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(1f, 0.90f, 0.65f);
            l.intensity = 1.5f;
            l.range = 15f;
        }

        private void MakeFence(Vector3 pos, float angle)
        {
            var post = GameObject.CreatePrimitive(PrimitiveType.Cube);
            post.name = "Fence"; post.transform.SetParent(envParent);
            post.transform.position = pos + Vector3.up;
            post.transform.localScale = new Vector3(0.3f, 2, 0.3f);
            post.transform.rotation = Quaternion.Euler(0, angle, 0);
            post.GetComponent<Renderer>().material.color = new Color(0.50f, 0.32f, 0.18f);
        }

        private void MakeStall(Vector3 pos, Color awningColor)
        {
            var counter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            counter.name = "Stall"; counter.transform.SetParent(envParent);
            counter.transform.position = pos + Vector3.up;
            counter.transform.localScale = new Vector3(5, 1.5f, 2.5f);
            counter.GetComponent<Renderer>().material.color = new Color(0.55f, 0.38f, 0.22f);

            var awning = GameObject.CreatePrimitive(PrimitiveType.Cube);
            awning.name = "StallAwning"; awning.transform.SetParent(envParent);
            awning.transform.position = pos + new Vector3(0, 4.5f, -1.2f);
            awning.transform.localScale = new Vector3(6, 0.2f, 3.5f);
            awning.GetComponent<Renderer>().material.color = awningColor;
            Destroy(awning.GetComponent<Collider>());
        }

        #endregion

        #region Nature

        private void CreateNature()
        {
            // Trees (80 scattered + 40 forest)
            for (int i = 0; i < 60; i++)
            {
                Vector3 p = new Vector3(Random.Range(-150f,150f), 0, Random.Range(-150f,150f));
                if (Vector3.Distance(p, villageCenter) > 40) MakeTree(p);
            }
            for (int i = 0; i < 40; i++)
                MakeTree(forestEntrance + new Vector3(Random.Range(-45f,45f), 0, Random.Range(-45f,45f)));

            // Flowers (40)
            for (int i = 0; i < 40; i++)
                MakeFlower(villageCenter + new Vector3(Random.Range(-50f,50f), 0.25f, Random.Range(-50f,50f)));

            // Rocks (20)
            for (int i = 0; i < 20; i++)
            {
                Vector3 p = new Vector3(Random.Range(-120f,120f), 0, Random.Range(-120f,120f));
                if (Vector3.Distance(p, villageCenter) > 35) MakeRock(p);
            }

            // Animals (12)
            for (int i = 0; i < 6; i++) MakeAnimal("Chicken", villageCenter + new Vector3(Random.Range(-35f,35f),0.3f,Random.Range(-35f,35f)), Color.white, 0.4f);
            for (int i = 0; i < 3; i++) MakeAnimal("Dog", villageCenter + new Vector3(Random.Range(-40f,40f),0.4f,Random.Range(-40f,40f)), new Color(.6f,.4f,.2f), 0.6f);
            for (int i = 0; i < 3; i++) MakeAnimal("Cat", villageCenter + new Vector3(Random.Range(-40f,40f),0.3f,Random.Range(-40f,40f)), Color.gray, 0.3f);

            if (showDebug) Debug.Log("[Village] Nature created (100 trees, 40 flowers, 20 rocks, 12 animals)");
        }

        private void MakeTree(Vector3 pos)
        {
            float h = Random.Range(5f, 9f);
            float w = Random.Range(4f, 7f);

            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Tree"; trunk.transform.SetParent(envParent);
            trunk.transform.position = pos + Vector3.up * (h/2);
            trunk.transform.localScale = new Vector3(1.2f, h, 1.2f);
            trunk.GetComponent<Renderer>().material.color = new Color(0.45f, 0.28f, 0.15f);

            for (int i = 0; i < 3; i++)
            {
                var leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                leaves.name = "Leaves"; leaves.transform.SetParent(envParent);
                leaves.transform.position = pos + Vector3.up * (h+1) + new Vector3(Random.Range(-1.5f,1.5f), Random.Range(-1f,1f), Random.Range(-1.5f,1.5f));
                float s = w + Random.Range(-1f, 1f);
                leaves.transform.localScale = new Vector3(s, s*1.2f, s);
                leaves.GetComponent<Renderer>().material.color = new Color(Random.Range(.15f,.30f), Random.Range(.45f,.70f), Random.Range(.15f,.30f));
                Destroy(leaves.GetComponent<Collider>());
            }
        }

        private void MakeFlower(Vector3 pos)
        {
            var stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stem.name = "Flower"; stem.transform.SetParent(envParent);
            stem.transform.position = pos;
            stem.transform.localScale = new Vector3(0.1f, 0.5f, 0.1f);
            stem.GetComponent<Renderer>().material.color = new Color(0.2f, 0.5f, 0.2f);
            Destroy(stem.GetComponent<Collider>());

            Color[] fc = { new Color(.9f,.2f,.3f), new Color(1f,.85f,.2f), new Color(.8f,.3f,.7f), new Color(.3f,.6f,.9f), new Color(1f,.5f,.2f), Color.white };
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "FlowerHead"; head.transform.SetParent(envParent);
            head.transform.position = pos + Vector3.up * 0.5f;
            head.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            head.GetComponent<Renderer>().material.color = fc[Random.Range(0, fc.Length)];
            Destroy(head.GetComponent<Collider>());
        }

        private void MakeRock(Vector3 pos)
        {
            float s = Random.Range(0.5f, 2f);
            var r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            r.name = "Rock"; r.transform.SetParent(envParent);
            r.transform.position = pos + Vector3.up * (s * 0.4f);
            r.transform.localScale = new Vector3(s*1.5f, s, s*1.2f);
            r.GetComponent<Renderer>().material.color = new Color(Random.Range(.4f,.6f), Random.Range(.4f,.55f), Random.Range(.35f,.50f));
        }

        private void MakeAnimal(string name, Vector3 pos, Color color, float size)
        {
            var a = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            a.name = name; a.transform.SetParent(envParent);
            a.transform.position = pos;
            a.transform.localScale = Vector3.one * size;
            a.GetComponent<Renderer>().material.color = color;
        }

        #endregion

        #region Quests (GDD)

        private void CreateQuests()
        {
            if (QuestManager.Instance == null)
            {
                Debug.LogError("[Village] QuestManager not found! Quests will not work.");
                return;
            }

            // Create "Permintaan Tetua" quest (Kill 5 Slimes)
            CreateKillSlimesQuest();
            
            // Create "Ancaman Serigala" quest (Kill 3 Wolves)
            CreateKillWolvesQuest();
            
            // Create "Guardian of the Forest" quest (Kill Boss)
            CreateKillBossQuest();

            if (showDebug) Debug.Log("[Village] Quests created (GDD: Permintaan Tetua, Ancaman Serigala, Guardian)");
        }

        private void CreateKillSlimesQuest()
        {
            // Create quest data at runtime
            QuestData quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questID = "quest_kill_slimes";
            quest.questName = "Permintaan Tetua";
            quest.description = "Elder Tetua meminta bantuanmu untuk menghilangkan slime yang mengganggu desa. Bunuh 5 Slime di Training Ground.";
            quest.mainType = QuestType.Kill;
            quest.recommendedLevel = 1;
            
            // Objective: Kill 5 Slimes
            quest.objectives = new System.Collections.Generic.List<QuestObjective>();
            var objective = new QuestObjective();
            objective.description = "Bunuh 5 Slime";
            objective.type = QuestType.Kill;
            objective.targetID = "Slime";  // Matches base monster name
            objective.requiredAmount = 5;
            objective.currentAmount = 0;
            quest.objectives.Add(objective);
            
            // Rewards
            quest.rewards = new QuestReward();
            quest.rewards.exp = 100;
            quest.rewards.gold = 50;
            quest.rewards.itemIDs = new System.Collections.Generic.List<string>();
            
            // Add to QuestManager
            if (QuestManager.Instance != null)
            {
                // Use reflection to add to allQuests list
                var allQuestsField = QuestManager.Instance.GetType().GetField("allQuests", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (allQuestsField != null)
                {
                    var allQuests = allQuestsField.GetValue(QuestManager.Instance) as System.Collections.Generic.List<QuestData>;
                    if (allQuests != null)
                    {
                        allQuests.Add(quest);
                        Debug.Log("[Quest] Created: Permintaan Tetua (Kill 5 Slimes)");
                    }
                }
            }
        }

        private void CreateKillWolvesQuest()
        {
            QuestData quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questID = "quest_kill_wolves";
            quest.questName = "Ancaman Serigala";
            quest.description = "Serigala-serigala di hutan semakin berbahaya. Bunuh 3 Serigala untuk melindungi desa.";
            quest.mainType = QuestType.Kill;
            quest.recommendedLevel = 3;
            quest.previousQuestID = "quest_kill_slimes"; // Must complete slimes first
            
            quest.objectives = new System.Collections.Generic.List<QuestObjective>();
            var objective = new QuestObjective();
            objective.description = "Bunuh 3 Serigala";
            objective.type = QuestType.Kill;
            objective.targetID = "Wolf";
            objective.requiredAmount = 3;
            objective.currentAmount = 0;
            quest.objectives.Add(objective);
            
            quest.rewards = new QuestReward();
            quest.rewards.exp = 200;
            quest.rewards.gold = 100;
            quest.rewards.itemIDs = new System.Collections.Generic.List<string>();
            
            if (QuestManager.Instance != null)
            {
                var allQuestsField = QuestManager.Instance.GetType().GetField("allQuests", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (allQuestsField != null)
                {
                    var allQuests = allQuestsField.GetValue(QuestManager.Instance) as System.Collections.Generic.List<QuestData>;
                    if (allQuests != null)
                    {
                        allQuests.Add(quest);
                        Debug.Log("[Quest] Created: Ancaman Serigala (Kill 3 Wolves)");
                    }
                }
            }
        }

        private void CreateKillBossQuest()
        {
            QuestData quest = ScriptableObject.CreateInstance<QuestData>();
            quest.questID = "quest_kill_boss";
            quest.questName = "Guardian of the Forest";
            quest.description = "Boss kuat menghalangi jalan ke Green Forest. Kalahkan Guardian of the Forest!";
            quest.mainType = QuestType.Boss;
            quest.recommendedLevel = 8;
            quest.previousQuestID = "quest_kill_wolves";
            
            quest.objectives = new System.Collections.Generic.List<QuestObjective>();
            var objective = new QuestObjective();
            objective.description = "Kalahkan Guardian of the Forest";
            objective.type = QuestType.Kill;
            objective.targetID = "Guardian of the Forest";
            objective.requiredAmount = 1;
            objective.currentAmount = 0;
            quest.objectives.Add(objective);
            
            quest.rewards = new QuestReward();
            quest.rewards.exp = 500;
            quest.rewards.gold = 250;
            quest.rewards.itemIDs = new System.Collections.Generic.List<string>();
            
            if (QuestManager.Instance != null)
            {
                var allQuestsField = QuestManager.Instance.GetType().GetField("allQuests", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (allQuestsField != null)
                {
                    var allQuests = allQuestsField.GetValue(QuestManager.Instance) as System.Collections.Generic.List<QuestData>;
                    if (allQuests != null)
                    {
                        allQuests.Add(quest);
                        Debug.Log("[Quest] Created: Guardian of the Forest (Kill Boss)");
                    }
                }
            }
        }

        #endregion

        #region NPCs (GDD)

        private void CreateNPCs()
        {
            // === GDD: Elder Tetua (Main Quest Giver) ===
            // Quest: "Permintaan Tetua" → Kill 5 Slimes → Return → Unlock Green Forest
            var elder = MakeNPC("Elder Tetua", villageCenter + new Vector3(0, 0, -40), new Color(0.9f, 0.85f, 0.7f));
            elder.transform.localScale = Vector3.one * 1.1f;
            // Add QuestGiver component
            var questGiver = elder.AddComponent<QuestGiver>();

            // === GDD: Blacksmith Pemula ===
            // Sells: Wooden Sword, Leather Armor, Wooden Shield
            var blacksmith = MakeNPC("Blacksmith Budi", villageCenter + new Vector3(-30, 0, 5), new Color(0.7f, 0.4f, 0.3f));
            // Add ShopTrigger component
            var blacksmithShop = blacksmith.AddComponent<ShopTrigger>();

            // === GDD: Merchant Keliling ===
            // Sells: HP Potion, MP Potion, Antidote, Torch
            var merchant = MakeNPC("Merchant Sari", villageCenter + new Vector3(30, 0, 5), new Color(0.4f, 0.7f, 0.4f));
            // Add ShopTrigger component
            var merchantShop = merchant.AddComponent<ShopTrigger>();

            // === GDD: Innkeeper ===
            // Rest: Restore HP/MP, Save Game
            var innkeeper = MakeNPC("Innkeeper Rina", villageCenter + new Vector3(5, 0, 35), new Color(0.8f, 0.6f, 0.5f));

            // === GDD: Village Guard ===
            // Info about forest, warns about dangers
            var guard = MakeNPC("Guard Captain", forestEntrance + new Vector3(10, 0, 5), new Color(0.5f, 0.5f, 0.6f));

            // === GDD: Training Master ===
            // Teaches basic combat
            var trainingMaster = MakeNPC("Training Master", trainingGround + new Vector3(0, 0, -25), new Color(0.6f, 0.5f, 0.4f));

            // Villagers (10)
            string[] villagerNames = {
                "Villager Andi", "Villager Budi", "Villager Citra", "Villager Dewi", "Villager Eka",
                "Villager Fani", "Villager Gita", "Villager Hadi", "Villager Ira", "Villager Joko"
            };
            for (int i = 0; i < 10; i++)
            {
                Vector3 p = villageCenter + new Vector3(Random.Range(-40f,40f), 0, Random.Range(-40f,40f));
                MakeNPC(villagerNames[i], p, new Color(Random.Range(.5f,.9f), Random.Range(.5f,.9f), Random.Range(.5f,.9f)));
            }

            if (showDebug) Debug.Log("[Village] NPCs created (GDD: Elder Tetua, Blacksmith, Merchant, Guard, Training Master)");
        }

        private GameObject MakeNPC(string name, Vector3 pos, Color color)
        {
            // Create NPC object
            var npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = name; 
            npc.transform.SetParent(npcParent);
            npc.transform.position = pos;
            npc.GetComponent<Renderer>().material.color = color;
            
            // Add collider for interaction
            var col = npc.GetComponent<CapsuleCollider>();
            if (col == null) col = npc.AddComponent<CapsuleCollider>();
            
            // Add DialogueTrigger component (all NPCs can talk)
            var dialogueTrigger = npc.AddComponent<DialogueTrigger>();
            
            // Name label
            var label = new GameObject(name + "_Label");
            label.transform.SetParent(npc.transform);
            label.transform.localPosition = Vector3.up * 2.5f;
            label.transform.localScale = Vector3.one * 0.1f;
            var tm = label.AddComponent<TextMesh>();
            tm.text = name;
            tm.fontSize = 40;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = Color.white;

            return npc;
        }

        #endregion

        #region Monsters (GDD)

        private void CreateMonsters()
        {
            // === GDD: Slime (Lv 1-3) - Training Ground ===
            // Passive, easy to kill (2-3 hits), drops: Sticky Gel, HP Potion (10%)
            for (int i = 0; i < 8; i++)
            {
                int level = Random.Range(1, 4); // Lv 1-3
                MakeMonster("Slime", trainingGround + new Vector3(Random.Range(-25f,25f),0.5f,Random.Range(-25f,25f)), 
                    new Color(0.3f, 0.8f, 0.3f), level, "Passive", 50f); // Low HP
            }

            // === GDD: Wolf (Lv 5-8) - Forest Edge ===
            // Aggressive, faster, drops: Wolf Fang, Leather (30%)
            for (int i = 0; i < 5; i++)
            {
                int level = Random.Range(5, 9); // Lv 5-8
                MakeMonster("Wolf", forestEntrance + new Vector3(Random.Range(-35f,35f),0.5f,Random.Range(-35f,35f)), 
                    Color.gray, level, "Aggressive", 80f); // Medium HP
            }

            // === GDD: Forest Boar (Lv 6-9) - Forest Edge ===
            // Territorial, tanky, drops: Boar Tusk, Meat (40%)
            for (int i = 0; i < 4; i++)
            {
                int level = Random.Range(6, 10); // Lv 6-9
                MakeMonster("Forest Boar", forestEntrance + new Vector3(Random.Range(-40f,40f),0.5f,Random.Range(-40f,40f)), 
                    new Color(0.5f, 0.35f, 0.25f), level, "Territorial", 100f); // Higher HP
            }

            // === GDD: Mushroom (Lv 3-5) - Between areas ===
            // Passive, poison attack, drops: Mushroom Cap, Antidote (15%)
            for (int i = 0; i < 5; i++)
            {
                int level = Random.Range(3, 6); // Lv 3-5
                Vector3 p = new Vector3(Random.Range(-50f, -20f), 0.5f, Random.Range(-20f, 20f));
                MakeMonster("Poison Mushroom", p, new Color(0.7f, 0.2f, 0.7f), level, "Passive", 40f); // Low HP
            }

            // === GDD: Boss - Guardian of the Forest (Lv 10) ===
            // Territorial, powerful, blocks path to Green Forest
            // Drops: Forest Key (unlocks Green Forest), Rare Equipment
            MakeBoss("Guardian of the Forest", forestEntrance + new Vector3(-40, 0.5f, -40), 
                new Color(0.2f, 0.6f, 0.3f), 10, 300f); // Boss HP

            if (showDebug) Debug.Log("[Village] Monsters created (GDD: Slime Lv1-3, Wolf Lv5-8, Boar Lv6-9, Mushroom Lv3-5, Boss Lv10)");
        }

        private void MakeMonster(string name, Vector3 pos, Color color, int level, string behavior, float maxHP = 100f)
        {
            // Create monster object
            var m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m.name = $"{name}_Lv{level}"; 
            m.transform.SetParent(monsterParent);
            m.transform.position = pos;
            m.GetComponent<Renderer>().material.color = color;
            
            // Set tag
            try { m.tag = "Enemy"; } catch { m.tag = "Untagged"; }
            
            // Add collider for click detection
            var col = m.GetComponent<SphereCollider>();
            if (col == null) col = m.AddComponent<SphereCollider>();
            
            // Add SimpleMonsterAI component
            var ai = m.AddComponent<SimpleMonsterAI>();
            
            // Set HP based on monster type (using reflection or serialized field)
            // For now, we'll rely on the default values which are reasonable
            
            // Name label
            var label = new GameObject($"{name}_Label");
            label.transform.SetParent(m.transform);
            label.transform.localPosition = Vector3.up * 2f;
            label.transform.localScale = Vector3.one * 0.1f;
            var tm = label.AddComponent<TextMesh>();
            tm.text = $"{name} Lv.{level}";
            tm.fontSize = 36;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = level >= 8 ? Color.red : Color.yellow;
        }

        private void MakeBoss(string name, Vector3 pos, Color color, int level, float maxHP = 300f)
        {
            // Create boss object (bigger)
            var m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m.name = $"BOSS_{name}_Lv{level}"; 
            m.transform.SetParent(monsterParent);
            m.transform.position = pos;
            m.transform.localScale = Vector3.one * 3f;
            m.GetComponent<Renderer>().material.color = color;
            
            // Set tag
            try { m.tag = "Enemy"; } catch { m.tag = "Untagged"; }
            
            // Add collider for click detection
            var col = m.GetComponent<SphereCollider>();
            if (col == null) col = m.AddComponent<SphereCollider>();
            
            // Add SimpleMonsterAI component
            var ai = m.AddComponent<SimpleMonsterAI>();

            // Boss crown (visual indicator)
            var crown = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crown.name = "BossCrown"; crown.transform.SetParent(m.transform);
            crown.transform.localPosition = new Vector3(0, 1.5f, 0);
            crown.transform.localScale = new Vector3(1.5f, 0.5f, 1.5f);
            crown.GetComponent<Renderer>().material.color = new Color(1f, 0.84f, 0f);
            Destroy(crown.GetComponent<Collider>());

            // Boss label
            var label = new GameObject($"BOSS_{name}_Label");
            label.transform.SetParent(m.transform);
            label.transform.localPosition = Vector3.up * 3f;
            label.transform.localScale = Vector3.one * 0.15f;
            var tm = label.AddComponent<TextMesh>();
            tm.text = $"★ {name} Lv.{level} ★";
            tm.fontSize = 40;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = new Color(1f, 0.5f, 0f);
        }

        #endregion

        #region Tutorial Area (GDD)

        private void CreateTutorialArea()
        {
            // GDD Tutorial: Movement, Combat, Skills, Items, Menu
            // Create tutorial sign posts

            // Movement tutorial sign
            MakeSignPost(villageCenter + new Vector3(0, 0, 10), 
                "TUTORIAL: Gunakan WASD untuk bergerak, Mouse untuk kamera");

            // Combat tutorial sign
            MakeSignPost(trainingGround + new Vector3(0, 0, -20), 
                "TUTORIAL: Klik kiri untuk menyerang, 1-4 untuk skill");

            // Items tutorial sign
            MakeSignPost(villageCenter + new Vector3(30, 0, 10), 
                "TUTORIAL: Tekan I untuk inventory, E untuk equipment");

            // Quest tutorial sign
            MakeSignPost(villageCenter + new Vector3(-30, 0, 10), 
                "TUTORIAL: Tekan J untuk quest log, F untuk bicara dengan NPC");

            // Training dummies (for combat practice)
            for (int i = 0; i < 4; i++)
                MakeTrainingDummy(trainingGround + new Vector3(Random.Range(-15f,15f), 1, Random.Range(-15f,15f)));

            if (showDebug) Debug.Log("[Village] Tutorial area created (GDD: Movement, Combat, Skills, Items, Menu)");
        }

        private void MakeSignPost(Vector3 pos, string text)
        {
            // Post
            var post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            post.name = "SignPost"; post.transform.SetParent(envParent);
            post.transform.position = pos + Vector3.up * 1.5f;
            post.transform.localScale = new Vector3(0.3f, 3, 0.3f);
            post.GetComponent<Renderer>().material.color = new Color(0.45f, 0.30f, 0.18f);

            // Sign board
            var sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sign.name = "Sign"; sign.transform.SetParent(envParent);
            sign.transform.position = pos + Vector3.up * 3.5f;
            sign.transform.localScale = new Vector3(3, 2, 0.2f);
            sign.GetComponent<Renderer>().material.color = new Color(0.80f, 0.70f, 0.55f);
            Destroy(sign.GetComponent<Collider>());

            // Text
            var label = new GameObject("SignText");
            label.transform.SetParent(sign.transform);
            label.transform.localPosition = Vector3.zero;
            label.transform.localScale = Vector3.one * 0.08f;
            var tm = label.AddComponent<TextMesh>();
            tm.text = text;
            tm.fontSize = 30;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = Color.black;
        }

        private void MakeTrainingDummy(Vector3 pos)
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "TrainingDummy"; body.transform.SetParent(envParent);
            body.transform.position = pos;
            body.GetComponent<Renderer>().material.color = new Color(0.6f, 0.5f, 0.3f);

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "DummyHead"; head.transform.SetParent(envParent);
            head.transform.position = pos + Vector3.up * 1.5f;
            head.transform.localScale = Vector3.one * 0.5f;
            head.GetComponent<Renderer>().material.color = new Color(0.7f, 0.6f, 0.4f);
            Destroy(head.GetComponent<Collider>());
        }

        #endregion

        #region Warp Points

        private void CreateWarpPoints()
        {
            // Village → Training Ground
            MakeWarp(villageCenter + new Vector3(40,0,0), trainingGround + new Vector3(-15,0,0));
            // Village → Forest
            MakeWarp(villageCenter + new Vector3(-40,0,0), forestEntrance + new Vector3(15,0,0));
            // Training → Forest
            MakeWarp(trainingGround + new Vector3(-15,0,0), forestEntrance + new Vector3(15,0,0));
            // Forest → Village (shortcut)
            MakeWarp(forestEntrance + new Vector3(0,0,0), villageCenter);

            if (showDebug) Debug.Log("[Village] Warp points created (4)");
        }

        private void MakeWarp(Vector3 from, Vector3 to)
        {
            var warp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            warp.name = "WarpPoint"; warp.transform.SetParent(envParent);
            warp.transform.position = from + Vector3.up * 0.5f;
            warp.transform.localScale = new Vector3(3, 0.3f, 3);
            warp.GetComponent<Renderer>().material.color = new Color(0.2f, 0.8f, 1f, 0.7f);

            var lo = new GameObject("WarpGlow");
            lo.transform.SetParent(envParent);
            lo.transform.position = from + Vector3.up;
            var l = lo.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = Color.cyan;
            l.intensity = 2f;
            l.range = 10f;
        }

        #endregion

        #region Forest Gate (GDD)

        private void CreateForestGate()
        {
            // GDD: Gate to Green Forest locked until quest "Permintaan Tetua" complete
            // Visual gate at forest entrance

            Vector3 gatePos = forestEntrance + new Vector3(20, 0, 0);

            // Gate pillars
            for (int x = -1; x <= 1; x += 2)
            {
                var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pillar.name = "GatePillar"; pillar.transform.SetParent(structParent);
                pillar.transform.position = gatePos + new Vector3(x * 5, 4, 0);
                pillar.transform.localScale = new Vector3(1.5f, 8, 1.5f);
                pillar.GetComponent<Renderer>().material.color = new Color(0.45f, 0.35f, 0.25f);
            }

            // Gate bar (horizontal)
            var bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bar.name = "GateBar"; bar.transform.SetParent(structParent);
            bar.transform.position = gatePos + new Vector3(0, 4, 0);
            bar.transform.localScale = new Vector3(10, 1, 1);
            bar.GetComponent<Renderer>().material.color = new Color(0.40f, 0.30f, 0.20f);

            // Gate sign
            var signLabel = new GameObject("GateSign");
            signLabel.transform.SetParent(structParent);
            signLabel.transform.position = gatePos + new Vector3(0, 7, 0);
            signLabel.transform.localScale = Vector3.one * 0.12f;
            var tm = signLabel.AddComponent<TextMesh>();
            tm.text = "GREEN FOREST\n(Locked - Complete Quest)";
            tm.fontSize = 30;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = Color.red;

            // Guard NPC
            MakeNPC("Gate Guard", gatePos + new Vector3(8, 0, 0), new Color(0.5f, 0.5f, 0.6f));

            if (showDebug) Debug.Log("[Village] Forest Gate created (Locked until quest complete)");
        }

        #endregion
    }
}