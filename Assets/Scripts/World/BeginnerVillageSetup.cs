using UnityEngine;
using System.Collections.Generic;
using ArcadiaOnline.Quest;
using ArcadiaOnline.Dialogue;
using ArcadiaOnline.Shop;
using ArcadiaOnline.Combat;
using ArcadiaOnline.Monster;
using ArcadiaOnline.Player;

namespace ArcadiaOnline.World
{
    public class BeginnerVillageSetup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool autoCreateOnStart = true;
        [SerializeField] private bool showDebug = true;

        // Positions (3x bigger map)
        private Vector3 villageCenter = Vector3.zero;
        private Vector3 trainingGround = new Vector3(180, 0, 0);
        private Vector3 forestEntrance = new Vector3(-180, 0, 0);

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
            if (showDebug) Debug.Log("[Village] Creating expanded village (3x)...");

            // Parents
            envParent = new GameObject("Environment").transform;
            structParent = new GameObject("Structures").transform;
            npcParent = new GameObject("NPCs").transform;
            monsterParent = new GameObject("Monsters").transform;

            // Ground
            MakeGround("OuterGrass", Vector3.zero, new Vector3(600, 0.05f, 600), new Color(0.35f, 0.55f, 0.30f));
            MakeGround("VillagePlaza", villageCenter, new Vector3(160, 0.10f, 160), new Color(0.55f, 0.45f, 0.35f));
            MakeGround("TrainingArea", trainingGround, new Vector3(100, 0.10f, 100), new Color(0.65f, 0.55f, 0.40f));
            MakeGround("ForestFloor", forestEntrance, new Vector3(140, 0.10f, 140), new Color(0.25f, 0.40f, 0.25f));

            // Roads
            MakeRoad(villageCenter + new Vector3(80, 0.12f, 0), new Vector3(140, 0.12f, 10));
            MakeRoad(villageCenter + new Vector3(-80, 0.12f, 0), new Vector3(140, 0.12f, 10));
            MakeRoad(villageCenter, new Vector3(10, 0.12f, 140));
            MakeRoad(villageCenter, new Vector3(140, 0.12f, 10));
            // Ring road
            MakeRoad(villageCenter + new Vector3(0, 0.12f, 50), new Vector3(100, 0.12f, 6));
            MakeRoad(villageCenter + new Vector3(0, 0.12f, -50), new Vector3(100, 0.12f, 6));
            MakeRoad(villageCenter + new Vector3(50, 0.12f, 0), new Vector3(6, 0.12f, 100));
            MakeRoad(villageCenter + new Vector3(-50, 0.12f, 0), new Vector3(6, 0.12f, 100));

            // Major buildings (6)
            MakeBuilding("VillageHall", villageCenter + new Vector3(0, 0, -60), new Vector3(35, 18, 25), new Color(0.70f, 0.60f, 0.50f));
            MakeBuilding("Inn", villageCenter + new Vector3(45, 0, -40), new Vector3(22, 14, 18), new Color(0.60f, 0.40f, 0.30f));
            MakeBuilding("Warehouse", villageCenter + new Vector3(-45, 0, -40), new Vector3(28, 12, 20), new Color(0.50f, 0.50f, 0.50f));
            MakeBuilding("Temple", villageCenter + new Vector3(0, 0, 60), new Vector3(25, 22, 18), new Color(0.90f, 0.88f, 0.80f));
            MakeBuilding("TrainingHall", trainingGround + new Vector3(0, 0, -40), new Vector3(30, 12, 20), new Color(0.60f, 0.50f, 0.40f));
            MakeBuilding("Watchtower", forestEntrance + new Vector3(-50, 0, -50), new Vector3(12, 25, 12), new Color(0.45f, 0.35f, 0.25f));

            // Houses (16)
            Vector3[] housePos = {
                new Vector3(35,0,35), new Vector3(-35,0,35), new Vector3(55,0,15), new Vector3(-55,0,15),
                new Vector3(55,0,-15), new Vector3(-55,0,-15), new Vector3(35,0,-35), new Vector3(-35,0,-35),
                new Vector3(70,0,0), new Vector3(-70,0,0), new Vector3(0,0,70), new Vector3(0,0,-70),
                new Vector3(70,0,40), new Vector3(-70,0,40), new Vector3(70,0,-40), new Vector3(-70,0,-40)
            };
            Color[] houseCol = {
                new Color(.80f,.60f,.40f), new Color(.70f,.50f,.30f), new Color(.65f,.45f,.25f), new Color(.85f,.70f,.50f),
                new Color(.75f,.55f,.35f), new Color(.60f,.40f,.20f), new Color(.80f,.65f,.45f), new Color(.55f,.35f,.15f),
                new Color(.78f,.68f,.55f), new Color(.68f,.58f,.45f), new Color(.88f,.78f,.65f), new Color(.58f,.48f,.35f),
                new Color(.82f,.62f,.42f), new Color(.72f,.52f,.32f), new Color(.62f,.42f,.22f), new Color(.90f,.75f,.55f)
            };
            for (int i = 0; i < 16; i++)
                MakeBuilding($"House_{i+1}", villageCenter + housePos[i], new Vector3(14, Random.Range(8f,12f), 12), houseCol[i]);

            // Shops (4)
            MakeBuilding("GeneralStore", villageCenter + new Vector3(-25, 0, 25), new Vector3(18, 11, 14), new Color(.3f,.6f,.3f));
            MakeBuilding("WeaponShop", villageCenter + new Vector3(-40, 0, 10), new Vector3(18, 11, 14), new Color(.6f,.3f,.3f));
            MakeBuilding("ArmorShop", villageCenter + new Vector3(25, 0, 25), new Vector3(18, 11, 14), new Color(.3f,.3f,.6f));
            MakeBuilding("PotionShop", villageCenter + new Vector3(40, 0, 10), new Vector3(18, 11, 14), new Color(.6f,.3f,.6f));

            // Farm
            MakeFarm(villageCenter + new Vector3(100, 0, -50));

            // Fountain
            MakeFountain();

            // Benches (12)
            Vector3[] benchPos = {
                new Vector3(18,0,0), new Vector3(-18,0,0), new Vector3(0,0,18), new Vector3(0,0,-18),
                new Vector3(12,0,12), new Vector3(-12,0,12), new Vector3(12,0,-12), new Vector3(-12,0,-12),
                new Vector3(25,0,8), new Vector3(-25,0,8), new Vector3(8,0,25), new Vector3(-8,0,25)
            };
            foreach (var p in benchPos) MakeBench(villageCenter + p);

            // Lamp posts (20)
            Vector3[] lampPos = {
                new Vector3(12,0,0), new Vector3(-12,0,0), new Vector3(0,0,12), new Vector3(0,0,-12),
                new Vector3(25,0,0), new Vector3(-25,0,0), new Vector3(0,0,25), new Vector3(0,0,-25),
                new Vector3(40,0,0), new Vector3(-40,0,0), new Vector3(0,0,40), new Vector3(0,0,-40),
                new Vector3(60,0,0), new Vector3(-60,0,0), new Vector3(0,0,60), new Vector3(0,0,-60),
                new Vector3(35,0,35), new Vector3(-35,0,35), new Vector3(35,0,-35), new Vector3(-35,0,-35)
            };
            foreach (var p in lampPos) MakeLampPost(villageCenter + p);

            // Fences
            for (int a = 0; a < 360; a += 10)
            {
                float rad = a * Mathf.Deg2Rad;
                MakeFence(villageCenter + new Vector3(Mathf.Cos(rad)*78, 0, Mathf.Sin(rad)*78), a);
            }
            for (int i = -40; i <= 40; i += 8)
            {
                MakeFence(trainingGround + new Vector3(i,0,-40), 0);
                MakeFence(trainingGround + new Vector3(i,0,40), 0);
            }

            // Market stalls (4)
            MakeStall(villageCenter + new Vector3(30,0,30), new Color(.8f,.2f,.2f));
            MakeStall(villageCenter + new Vector3(-30,0,30), new Color(.2f,.2f,.8f));
            MakeStall(villageCenter + new Vector3(30,0,-30), new Color(.8f,.8f,.2f));
            MakeStall(villageCenter + new Vector3(-30,0,-30), new Color(.2f,.8f,.8f));

            // Trees (~120)
            for (int i = 0; i < 70; i++)
            {
                Vector3 p = new Vector3(Random.Range(-250f,250f), 0, Random.Range(-250f,250f));
                if (Vector3.Distance(p, villageCenter) > 50) MakeTree(p);
            }
            for (int i = 0; i < 50; i++)
                MakeTree(forestEntrance + new Vector3(Random.Range(-65f,65f), 0, Random.Range(-65f,65f)));

            // Flowers (60)
            for (int i = 0; i < 60; i++)
                MakeFlower(villageCenter + new Vector3(Random.Range(-70f,70f), 0.25f, Random.Range(-70f,70f)));

            // Rocks (30)
            for (int i = 0; i < 30; i++)
            {
                Vector3 p = new Vector3(Random.Range(-200f,200f), 0, Random.Range(-200f,200f));
                if (Vector3.Distance(p, villageCenter) > 40) MakeRock(p);
            }

            // Animals (16)
            for (int i = 0; i < 8; i++) MakeAnimal("Chicken", villageCenter + new Vector3(Random.Range(-50f,50f),0.3f,Random.Range(-50f,50f)), Color.white, 0.4f);
            for (int i = 0; i < 4; i++) MakeAnimal("Dog", villageCenter + new Vector3(Random.Range(-60f,60f),0.4f,Random.Range(-60f,60f)), new Color(.6f,.4f,.2f), 0.6f);
            for (int i = 0; i < 4; i++) MakeAnimal("Cat", villageCenter + new Vector3(Random.Range(-60f,60f),0.3f,Random.Range(-60f,60f)), Color.gray, 0.3f);

            // NPCs
            MakeNPC("VillageChief", villageCenter + new Vector3(15,0,15), Color.blue);
            MakeNPC("Shopkeeper", villageCenter + new Vector3(-15,0,15), Color.green);
            MakeNPC("Blacksmith", villageCenter + new Vector3(-30,0,0), Color.red);
            MakeNPC("Elder", villageCenter + new Vector3(0,0,-30), Color.yellow);

            // Villagers (15)
            for (int i = 0; i < 15; i++)
            {
                Vector3 p = villageCenter + new Vector3(Random.Range(-60f,60f), 0, Random.Range(-60f,60f));
                MakeNPC($"Villager_{i+1}", p, new Color(Random.Range(.5f,.9f), Random.Range(.5f,.9f), Random.Range(.5f,.9f)));
            }

            // Monsters
            for (int i = 0; i < 10; i++) MakeMonster("Slime", trainingGround + new Vector3(Random.Range(-25f,25f),0.5f,Random.Range(-25f,25f)), Color.green, 1);
            for (int i = 0; i < 6; i++) MakeMonster("Wolf", forestEntrance + new Vector3(Random.Range(-40f,40f),0.5f,Random.Range(-40f,40f)), Color.gray, 3);
            MakeMonster("AlphaWolf", forestEntrance + new Vector3(-50,0.5f,-50), Color.black, 5);

            // Warp points
            MakeWarp(villageCenter + new Vector3(60,0,0), trainingGround + new Vector3(-20,0,0));
            MakeWarp(villageCenter + new Vector3(-60,0,0), forestEntrance + new Vector3(20,0,0));
            MakeWarp(trainingGround + new Vector3(-20,0,0), forestEntrance + new Vector3(20,0,0));
            MakeWarp(forestEntrance + new Vector3(0,0,0), villageCenter);

            // Training dummies (5)
            for (int i = 0; i < 5; i++)
                MakeTrainingDummy(trainingGround + new Vector3(Random.Range(-20f,20f),1,Random.Range(-20f,20f)));

            if (showDebug) Debug.Log("[Village] === DONE: 200+ objects created ===");
        }

        // ======================== HELPERS ========================

        private void MakeGround(string name, Vector3 pos, Vector3 scale, Color color)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = name; g.transform.SetParent(envParent);
            g.transform.position = pos; g.transform.localScale = scale;
            g.GetComponent<Renderer>().material.color = color;
            Destroy(g.GetComponent<Collider>());
        }

        private void MakeRoad(Vector3 pos, Vector3 scale)
        {
            MakeGround("Road", pos, scale, new Color(0.45f, 0.38f, 0.28f));
        }

        private void MakeBuilding(string name, Vector3 pos, Vector3 size, Color color)
        {
            // Body
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = name; body.transform.SetParent(structParent);
            body.transform.position = pos + Vector3.up * (size.y/2);
            body.transform.localScale = size;
            body.GetComponent<Renderer>().material.color = color;

            // Roof
            var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = name+"_Roof"; roof.transform.SetParent(structParent);
            roof.transform.position = pos + Vector3.up * (size.y + 1.5f);
            roof.transform.localScale = new Vector3(size.x+3, 2.5f, size.z+3);
            roof.GetComponent<Renderer>().material.color = new Color(0.50f, 0.20f, 0.15f);

            // Door
            var door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = name+"_Door"; door.transform.SetParent(structParent);
            door.transform.position = pos + new Vector3(0, 1.5f, size.z/2+0.1f);
            door.transform.localScale = new Vector3(2.5f, 3.5f, 0.2f);
            door.GetComponent<Renderer>().material.color = new Color(0.35f, 0.18f, 0.08f);
            Destroy(door.GetComponent<Collider>());

            // Windows
            MakeWindow(pos + new Vector3(-size.x*0.28f, size.y*0.55f, size.z/2+0.11f));
            MakeWindow(pos + new Vector3(size.x*0.28f, size.y*0.55f, size.z/2+0.11f));

            // Chimney for tall buildings
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

        private void MakeFarm(Vector3 farmPos)
        {
            MakeGround("FarmGround", farmPos, new Vector3(50, 0.12f, 50), new Color(0.40f, 0.30f, 0.18f));
            for (int x = -20; x <= 20; x += 6)
                for (int z = -20; z <= 20; z += 6)
                {
                    var c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.name = "Crop"; c.transform.SetParent(envParent);
                    c.transform.position = farmPos + new Vector3(x, 0.5f, z);
                    c.transform.localScale = new Vector3(2.5f, Random.Range(0.8f,1.5f), 2.5f);
                    c.GetComponent<Renderer>().material.color = new Color(Random.Range(.2f,.4f), Random.Range(.5f,.8f), Random.Range(.1f,.3f));
                    Destroy(c.GetComponent<Collider>());
                }
            MakeBuilding("FarmHouse", farmPos + new Vector3(30,0,0), new Vector3(15,10,12), new Color(0.60f, 0.40f, 0.20f));
        }

        private void MakeFountain()
        {
            var b = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            b.name = "FountainBase"; b.transform.SetParent(envParent);
            b.transform.position = villageCenter;
            b.transform.localScale = new Vector3(14, 2, 14);
            b.GetComponent<Renderer>().material.color = new Color(0.70f, 0.68f, 0.65f);

            var p = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            p.name = "FountainPillar"; p.transform.SetParent(envParent);
            p.transform.position = villageCenter + Vector3.up * 3.5f;
            p.transform.localScale = new Vector3(2.5f, 5, 2.5f);
            p.GetComponent<Renderer>().material.color = new Color(0.60f, 0.58f, 0.55f);

            var w = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            w.name = "FountainWater"; w.transform.SetParent(envParent);
            w.transform.position = villageCenter + Vector3.up * 6;
            w.transform.localScale = new Vector3(8, 2.5f, 8);
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
            counter.transform.localScale = new Vector3(6, 2, 3);
            counter.GetComponent<Renderer>().material.color = new Color(0.55f, 0.38f, 0.22f);

            var awning = GameObject.CreatePrimitive(PrimitiveType.Cube);
            awning.name = "StallAwning"; awning.transform.SetParent(envParent);
            awning.transform.position = pos + new Vector3(0, 5.2f, -1.5f);
            awning.transform.localScale = new Vector3(7, 0.2f, 4);
            awning.GetComponent<Renderer>().material.color = awningColor;
            Destroy(awning.GetComponent<Collider>());
        }

        private void MakeTree(Vector3 pos)
        {
            float h = Random.Range(5f, 10f);
            float w = Random.Range(4f, 8f);

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

        private void MakeNPC(string name, Vector3 pos, Color color)
        {
            var npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = name; npc.transform.SetParent(npcParent);
            npc.transform.position = pos;
            npc.GetComponent<Renderer>().material.color = color;

            // Name label (TextMesh)
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
        }

        private void MakeMonster(string name, Vector3 pos, Color color, int level)
        {
            var m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m.name = $"{name}_Lv{level}"; m.transform.SetParent(monsterParent);
            m.transform.position = pos;
            m.transform.localScale = name == "AlphaWolf" ? Vector3.one * 2.5f : Vector3.one;
            m.GetComponent<Renderer>().material.color = color;
            try { m.tag = "Enemy"; } catch { m.tag = "Untagged"; }

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
            tm.color = Color.red;
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
    }
}
