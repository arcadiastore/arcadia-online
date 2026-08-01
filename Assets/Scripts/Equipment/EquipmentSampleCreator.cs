using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArcadiaOnline.Equipment
{
    /// <summary>
    /// Helper untuk membuat sample equipment items.
    /// Attach ke GameObject dan klik "Create Sample Items" di Inspector.
    /// </summary>
    public class EquipmentSampleCreator : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Click to create sample items")]
        [SerializeField] private bool createSamples = false;

        void OnValidate()
        {
            if (createSamples)
            {
                createSamples = false;
                CreateAllSampleItems();
            }
        }

        [ContextMenu("Create Sample Equipment")]
        public void CreateAllSampleItems()
        {
            // Create folder
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Items"))
                AssetDatabase.CreateFolder("Assets/Resources", "Items");

            // Create subfolders
            CreateFolder("Assets/Resources/Items", "Weapons");
            CreateFolder("Assets/Resources/Items", "Armors");
            CreateFolder("Assets/Resources/Items", "Accessories");
            CreateFolder("Assets/Resources/Items", "Capes");
            CreateFolder("Assets/Resources/Items", "Costumes");

            // === WEAPONS (One Hand) ===
            CreateItem("Wooden_Sword", "Wooden Sword", "Pedang kayu sederhana",
                EquipmentSlot.WeaponOneHand, EquipmentRarity.Common, 1,
                atk: 5, weapon: WeaponType.Sword);

            CreateItem("Iron_Sword", "Iron Sword", "Pedang besi yang kuat",
                EquipmentSlot.WeaponOneHand, EquipmentRarity.Uncommon, 5,
                atk: 12, weapon: WeaponType.Sword);

            CreateItem("Steel_Sword", "Steel Sword", "Pedang baja berkualitas tinggi",
                EquipmentSlot.WeaponOneHand, EquipmentRarity.Rare, 10,
                atk: 25, critRate: 5, weapon: WeaponType.Sword);

            CreateItem("Flame_Sword", "Flame Sword", "Pedang yang menyala api",
                EquipmentSlot.WeaponOneHand, EquipmentRarity.Epic, 20,
                atk: 45, critRate: 8, critDmg: 20, weapon: WeaponType.Sword);

            // === WEAPONS (Two Hand) ===
            CreateItem("Wooden_Staff", "Wooden Staff", "Tongkat kayu sihir",
                EquipmentSlot.WeaponTwoHand, EquipmentRarity.Common, 1,
                matk: 8, weapon: WeaponType.Staff);

            CreateItem("Iron_Great_Sword", "Iron Great Sword", "Pedang besar besi",
                EquipmentSlot.WeaponTwoHand, EquipmentRarity.Uncommon, 5,
                atk: 20, weapon: WeaponType.Sword, twoHand: true);

            CreateItem("Crystal_Staff", "Crystal Staff", "Tongkat kristal berkekuatan tinggi",
                EquipmentSlot.WeaponTwoHand, EquipmentRarity.Rare, 10,
                matk: 35, mp: 50, weapon: WeaponType.Staff);

            CreateItem("Dragon_Slayer", "Dragon Slayer", "Pedang legendaris pembunuh naga",
                EquipmentSlot.WeaponTwoHand, EquipmentRarity.Legendary, 30,
                atk: 100, critRate: 15, critDmg: 50, weapon: WeaponType.Sword, twoHand: true);

            // === HELM ===
            CreateItem("Leather_Helm", "Leather Helm", "Helm kulit ringan",
                EquipmentSlot.Helm, EquipmentRarity.Common, 1,
                def: 3, hp: 20);

            CreateItem("Iron_Helm", "Iron Helm", "Helm besi kuat",
                EquipmentSlot.Helm, EquipmentRarity.Uncommon, 5,
                def: 8, hp: 50);

            CreateItem("Knight_Helm", "Knight Helm", "Helm ksatria sejati",
                EquipmentSlot.Helm, EquipmentRarity.Rare, 15,
                def: 15, hp: 100);

            // === T-SHIRT (Armor) ===
            CreateItem("Cloth_Shirt", "Cloth Shirt", "Baju kain sederhana",
                EquipmentSlot.TShirt, EquipmentRarity.Common, 1,
                def: 2, hp: 30);

            CreateItem("Leather_Armor", "Leather Armor", "Armor kulit",
                EquipmentSlot.TShirt, EquipmentRarity.Uncommon, 5,
                def: 10, hp: 80);

            CreateItem("Iron_Armor", "Iron Armor", "Armor besi berat",
                EquipmentSlot.TShirt, EquipmentRarity.Rare, 10,
                def: 20, hp: 150, spd: -5);

            CreateItem("Dragon_Armor", "Dragon Armor", "Armor dari sisik naga",
                EquipmentSlot.TShirt, EquipmentRarity.Epic, 25,
                def: 40, hp: 300, mdef: 20);

            // === PANTS ===
            CreateItem("Cloth_Pants", "Cloth Pants", "Celana kain",
                EquipmentSlot.Pants, EquipmentRarity.Common, 1,
                def: 2, hp: 20);

            CreateItem("Leather_Pants", "Leather Pants", "Celana kulit",
                EquipmentSlot.Pants, EquipmentRarity.Uncommon, 5,
                def: 6, hp: 50);

            CreateItem("Iron_Greaves", "Iron Greaves", "Pelindung kaki besi",
                EquipmentSlot.Pants, EquipmentRarity.Rare, 10,
                def: 12, hp: 100);

            // === SHOES ===
            CreateItem("Leather_Shoes", "Leather Shoes", "Sepatu kulit",
                EquipmentSlot.Shoes, EquipmentRarity.Common, 1,
                def: 1, spd: 2);

            CreateItem("Boots_of_Speed", "Boots of Speed", "Sepatu kecepatan",
                EquipmentSlot.Shoes, EquipmentRarity.Uncommon, 5,
                def: 3, spd: 8);

            CreateItem("Ninja_Shoes", "Ninja Shoes", "Sepatu ninja",
                EquipmentSlot.Shoes, EquipmentRarity.Rare, 15,
                def: 5, spd: 15, critRate: 3);

            // === WINGS / CAPE ===
            CreateItem("Cloth_Cape", "Cloth Cape", "Jubah kain sederhana",
                EquipmentSlot.WingsCape, EquipmentRarity.Common, 1,
                def: 2);

            CreateItem("Red_Cape", "Red Cape", "Jubah merah",
                EquipmentSlot.WingsCape, EquipmentRarity.Uncommon, 5,
                def: 5, atk: 3);

            CreateItem("Angel_Wings", "Angel Wings", "Sayang malaikat",
                EquipmentSlot.WingsCape, EquipmentRarity.Rare, 15,
                def: 10, hp: 50, spd: 5);

            CreateItem("Dragon_Wings", "Dragon Wings", "Sayap naga hitam",
                EquipmentSlot.WingsCape, EquipmentRarity.Epic, 25,
                def: 20, atk: 15, spd: 10, critRate: 5);

            CreateItem("Phoenix_Wings", "Phoenix Wings", "Sayap phoenix abadi",
                EquipmentSlot.WingsCape, EquipmentRarity.Legendary, 35,
                def: 30, atk: 25, spd: 15, hp: 200, critDmg: 30);

            // === RINGS ===
            CreateItem("Copper_Ring", "Copper Ring", "Cincin tembaga",
                EquipmentSlot.RingLeft, EquipmentRarity.Common, 1,
                atk: 2);

            CreateItem("Silver_Ring", "Silver Ring", "Cincin perak",
                EquipmentSlot.RingLeft, EquipmentRarity.Uncommon, 5,
                atk: 5, def: 3);

            CreateItem("Ring_of_Power", "Ring of Power", "Cincin kekuatan",
                EquipmentSlot.RingLeft, EquipmentRarity.Rare, 15,
                atk: 15, critRate: 5);

            CreateItem("Ring_of_Wisdom", "Ring of Wisdom", "Cincin kebijaksanaan",
                EquipmentSlot.RingRight, EquipmentRarity.Rare, 15,
                matk: 15, mp: 100);

            // === NECKLACE ===
            CreateItem("Wooden_Necklace", "Wooden Necklace", "Kalung kayu",
                EquipmentSlot.Necklace, EquipmentRarity.Common, 1,
                hp: 20);

            CreateItem("Gold_Necklace", "Gold Necklace", "Kalung emas",
                EquipmentSlot.Necklace, EquipmentRarity.Uncommon, 5,
                hp: 50, mp: 30);

            CreateItem("Amulet_of_Life", "Amulet of Life", "Jimat kehidupan",
                EquipmentSlot.Necklace, EquipmentRarity.Rare, 15,
                hp: 150, def: 10);

            // === COSTUME ===
            CreateItem("School_Uniform", "School Uniform", "Seragam sekolah",
                EquipmentSlot.Costume, EquipmentRarity.Common, 1);

            CreateItem("Butler_Suit", "Butler Suit", "Jas pelayan",
                EquipmentSlot.Costume, EquipmentRarity.Uncommon, 1);

            CreateItem("Royal_Gown", "Royal Gown", "Gaun kerajaan",
                EquipmentSlot.Costume, EquipmentRarity.Rare, 1);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Equipment] Sample items created!");
        }

        private void CreateFolder(string parent, string name)
        {
            string path = parent + "/" + name;
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, name);
        }

        private void CreateItem(string id, string name, string desc,
            EquipmentSlot slot, EquipmentRarity rarity, int level,
            float atk = 0, float matk = 0, float def = 0, float mdef = 0,
            float hp = 0, float mp = 0, float spd = 0,
            float critRate = 0, float critDmg = 0, float atkSpd = 0,
            WeaponType weapon = WeaponType.None, bool twoHand = false)
        {
            // Determine folder
            string folder;
            if (slot == EquipmentSlot.WeaponOneHand || slot == EquipmentSlot.WeaponTwoHand)
                folder = "Weapons";
            else if (slot == EquipmentSlot.TShirt || slot == EquipmentSlot.Pants || slot == EquipmentSlot.Helm || slot == EquipmentSlot.Shoes)
                folder = "Armors";
            else if (slot == EquipmentSlot.WingsCape)
                folder = "Capes";
            else if (slot == EquipmentSlot.Costume)
                folder = "Costumes";
            else
                folder = "Accessories";

            string path = $"Assets/Resources/Items/{folder}/{id}.asset";

            // Check if already exists
            if (AssetDatabase.LoadAssetAtPath<EquipmentData>(path) != null)
                return;

            EquipmentData item = ScriptableObject.CreateInstance<EquipmentData>();
            item.id = id;
            item.itemName = name;
            item.description = desc;
            item.slot = slot;
            item.rarity = rarity;
            item.levelRequirement = level;
            item.sellPrice = level * 10;
            item.weaponType = weapon;
            item.isTwoHanded = twoHand;
            item.atkBonus = atk;
            item.matkBbonus = matk;
            item.defBonus = def;
            item.mdefBonus = mdef;
            item.hpBonus = hp;
            item.mpBonus = mp;
            item.spdBonus = spd;
            item.critRateBonus = critRate;
            item.critDmgBonus = critDmg;
            item.atkSpdBonus = atkSpd;

            AssetDatabase.CreateAsset(item, path);
        }
#endif
    }
}
