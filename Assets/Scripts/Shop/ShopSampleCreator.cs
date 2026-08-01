using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArcadiaOnline.Shop
{
    /// <summary>
    /// Auto-create sample shops.
    /// Attach ke GameObject di scene, akan create saat Start().
    /// </summary>
    public class ShopSampleCreator : MonoBehaviour
    {
        [Header("Auto-Create")]
        [SerializeField] private bool createOnStart = true;

        [Header("Sample Shops")]
        [SerializeField] private bool createGeneralShop = true;
        [SerializeField] private bool createWeaponShop = true;
        [SerializeField] private bool createArmorShop = true;
        [SerializeField] private bool createAccessoryShop = true;

        [Header("Debug")]
        [SerializeField] private bool showDebug = true;

        void Start()
        {
            if (createOnStart)
            {
                CreateSampleShops();
            }
        }

        /// <summary>
        /// Create all sample shops.
        /// </summary>
        public void CreateSampleShops()
        {
            if (showDebug)
            {
                Debug.Log("[ShopSampleCreator] Creating sample shops...");
            }

            // Pastikan ada ShopManager
            if (ShopManager.Instance == null)
            {
                GameObject managerObj = new GameObject("ShopManager");
                managerObj.AddComponent<ShopManager>();

                if (showDebug)
                {
                    Debug.Log("[ShopSampleCreator] Created ShopManager");
                }
            }

            // Create shops
            List<ShopData> shops = new List<ShopData>();

            if (createGeneralShop)
            {
                ShopData generalShop = CreateGeneralShop();
                shops.Add(generalShop);
            }

            if (createWeaponShop)
            {
                ShopData weaponShop = CreateWeaponShop();
                shops.Add(weaponShop);
            }

            if (createArmorShop)
            {
                ShopData armorShop = CreateArmorShop();
                shops.Add(armorShop);
            }

            if (createAccessoryShop)
            {
                ShopData accessoryShop = CreateAccessoryShop();
                shops.Add(accessoryShop);
            }

            // Add shops to manager
            if (ShopManager.Instance != null && shops.Count > 0)
            {
                // Use reflection to set private field
                var field = typeof(ShopManager).GetField("allShops",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (field != null)
                {
                    field.SetValue(ShopManager.Instance, shops);
                }
            }

            // Create ShopUI
            if (ShopUI.Instance == null)
            {
                GameObject uiObj = new GameObject("ShopUI");
                uiObj.AddComponent<ShopUI>();

                if (showDebug)
                {
                    Debug.Log("[ShopSampleCreator] Created ShopUI");
                }
            }

            if (showDebug)
            {
                Debug.Log($"[ShopSampleCreator] Created {shops.Count} shops!");
            }
        }

        /// <summary>
        /// Create General Shop.
        /// </summary>
        private ShopData CreateGeneralShop()
        {
            ShopData shop = ScriptableObject.CreateInstance<ShopData>();
            shop.shopID = "general_shop";
            shop.shopName = "General Store";
            shop.description = "Sells basic items and potions.";
            shop.shopType = ShopType.General;
            shop.npcName = "Shopkeeper";
            shop.canBuy = true;
            shop.canSell = true;
            shop.buyMultiplier = 1.0f;
            shop.sellMultiplier = 0.5f;

            shop.items = new List<ShopItem>()
            {
                new ShopItem()
                {
                    itemID = "potion_hp",
                    itemName = "HP Potion",
                    buyPrice = 50,
                    sellPrice = 25,
                    stock = -1,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "potion_mp",
                    itemName = "MP Potion",
                    buyPrice = 75,
                    sellPrice = 37,
                    stock = -1,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "antidote",
                    itemName = "Antidote",
                    buyPrice = 100,
                    sellPrice = 50,
                    stock = -1,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "return_scroll",
                    itemName = "Return Scroll",
                    buyPrice = 200,
                    sellPrice = 100,
                    stock = -1,
                    isAvailable = true,
                    requiredLevel = 5
                }
            };

            if (showDebug)
            {
                Debug.Log("[ShopSampleCreator] Created General Shop");
            }

            return shop;
        }

        /// <summary>
        /// Create Weapon Shop.
        /// </summary>
        private ShopData CreateWeaponShop()
        {
            ShopData shop = ScriptableObject.CreateInstance<ShopData>();
            shop.shopID = "weapon_shop";
            shop.shopName = "Weapon Shop";
            shop.description = "Sells weapons for all classes.";
            shop.shopType = ShopType.Weapon;
            shop.npcName = "Blacksmith";
            shop.canBuy = true;
            shop.canSell = true;
            shop.buyMultiplier = 1.0f;
            shop.sellMultiplier = 0.4f;

            shop.items = new List<ShopItem>()
            {
                new ShopItem()
                {
                    itemID = "sword_iron",
                    itemName = "Iron Sword",
                    buyPrice = 500,
                    sellPrice = 250,
                    stock = 5,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "staff_wooden",
                    itemName = "Wooden Staff",
                    buyPrice = 600,
                    sellPrice = 300,
                    stock = 3,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "bow_short",
                    itemName = "Short Bow",
                    buyPrice = 450,
                    sellPrice = 225,
                    stock = 4,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "sword_steel",
                    itemName = "Steel Sword",
                    buyPrice = 1500,
                    sellPrice = 750,
                    stock = 2,
                    isAvailable = true,
                    requiredLevel = 10
                },
                new ShopItem()
                {
                    itemID = "staff_apprentice",
                    itemName = "Apprentice Staff",
                    buyPrice = 2000,
                    sellPrice = 1000,
                    stock = 2,
                    isAvailable = true,
                    requiredLevel = 15
                }
            };

            if (showDebug)
            {
                Debug.Log("[ShopSampleCreator] Created Weapon Shop");
            }

            return shop;
        }

        /// <summary>
        /// Create Armor Shop.
        /// </summary>
        private ShopData CreateArmorShop()
        {
            ShopData shop = ScriptableObject.CreateInstance<ShopData>();
            shop.shopID = "armor_shop";
            shop.shopName = "Armor Shop";
            shop.description = "Sells armor and shields.";
            shop.shopType = ShopType.Armor;
            shop.npcName = "Armorer";
            shop.canBuy = true;
            shop.canSell = true;
            shop.buyMultiplier = 1.0f;
            shop.sellMultiplier = 0.5f;

            shop.items = new List<ShopItem>()
            {
                new ShopItem()
                {
                    itemID = "armor_leather",
                    itemName = "Leather Armor",
                    buyPrice = 400,
                    sellPrice = 200,
                    stock = 5,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "armor_iron",
                    itemName = "Iron Armor",
                    buyPrice = 1200,
                    sellPrice = 600,
                    stock = 3,
                    isAvailable = true,
                    requiredLevel = 10
                },
                new ShopItem()
                {
                    itemID = "shield_wooden",
                    itemName = "Wooden Shield",
                    buyPrice = 300,
                    sellPrice = 150,
                    stock = 4,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "shield_iron",
                    itemName = "Iron Shield",
                    buyPrice = 800,
                    sellPrice = 400,
                    stock = 2,
                    isAvailable = true,
                    requiredLevel = 10
                }
            };

            if (showDebug)
            {
                Debug.Log("[ShopSampleCreator] Created Armor Shop");
            }

            return shop;
        }

        /// <summary>
        /// Create Accessory Shop.
        /// </summary>
        private ShopData CreateAccessoryShop()
        {
            ShopData shop = ScriptableObject.CreateInstance<ShopData>();
            shop.shopID = "accessory_shop";
            shop.shopName = "Accessory Shop";
            shop.description = "Sells rings, necklaces, and accessories.";
            shop.shopType = ShopType.Accessory;
            shop.npcName = "Jeweler";
            shop.canBuy = true;
            shop.canSell = true;
            shop.buyMultiplier = 1.2f;
            shop.sellMultiplier = 0.3f;

            shop.items = new List<ShopItem>()
            {
                new ShopItem()
                {
                    itemID = "ring_hp",
                    itemName = "HP Ring",
                    buyPrice = 300,
                    sellPrice = 100,
                    stock = 3,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "ring_mp",
                    itemName = "MP Ring",
                    buyPrice = 300,
                    sellPrice = 100,
                    stock = 3,
                    isAvailable = true,
                    requiredLevel = 1
                },
                new ShopItem()
                {
                    itemID = "necklace_atk",
                    itemName = "ATK Necklace",
                    buyPrice = 800,
                    sellPrice = 300,
                    stock = 2,
                    isAvailable = true,
                    requiredLevel = 10
                },
                new ShopItem()
                {
                    itemID = "necklace_def",
                    itemName = "DEF Necklace",
                    buyPrice = 800,
                    sellPrice = 300,
                    stock = 2,
                    isAvailable = true,
                    requiredLevel = 10
                }
            };

            if (showDebug)
            {
                Debug.Log("[ShopSampleCreator] Created Accessory Shop");
            }

            return shop;
        }
    }
}
