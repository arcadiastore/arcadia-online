using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArcadiaOnline.Shop
{
    /// <summary>
    /// UI untuk shop system.
    /// </summary>
    public class ShopUI : MonoBehaviour
    {
        public static ShopUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject shopPanel;

        [Header("Auto-Create UI")]
        [SerializeField] private bool autoCreateUI = true;

        // Internal references
        private Transform itemListParent;
        private Text shopNameText;
        private Text goldText;
        private Text itemDetailNameText;
        private Text itemDetailDescText;
        private Text itemDetailPriceText;
        private Button buyButton;
        private Button sellButton;
        private Button closeButton;
        private InputField quantityInput;
        private Text quantityText;

        // State
        private ShopItem selectedItem;
        private int quantity = 1;
        private bool isBuyMode = true; // true = buy, false = sell

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            if (autoCreateUI && shopPanel == null)
            {
                CreateShopUI();
            }

            // Register events
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OnShopOpened += OnShopOpened;
                ShopManager.Instance.OnShopClosed += OnShopClosed;
                ShopManager.Instance.OnItemBought += OnItemBought;
                ShopManager.Instance.OnItemSold += OnItemSold;
                ShopManager.Instance.OnGoldChanged += OnGoldChanged;
            }

            // Setup buttons
            SetupButtons();

            // Hide panel
            HidePanel();
        }

        void Update()
        {
            // Close shop dengan ESC
            if (Input.GetKeyDown(KeyCode.Escape) && ShopManager.Instance != null && ShopManager.Instance.IsShopOpen())
            {
                CloseShop();
            }
        }

        /// <summary>
        /// Setup button listeners.
        /// </summary>
        private void SetupButtons()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(OnBuyClicked);
            }

            if (sellButton != null)
            {
                sellButton.onClick.AddListener(OnSellClicked);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseShop);
            }

            if (quantityInput != null)
            {
                quantityInput.onValueChanged.AddListener(OnQuantityChanged);
            }
        }

        /// <summary>
        /// Show shop panel.
        /// </summary>
        private void ShowPanel()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                RefreshShop();
            }
        }

        /// <summary>
        /// Hide shop panel.
        /// </summary>
        private void HidePanel()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Refresh shop display.
        /// </summary>
        private void RefreshShop()
        {
            if (ShopManager.Instance == null) return;

            ShopData shop = ShopManager.Instance.GetCurrentShop();
            if (shop == null) return;

            // Update shop name
            if (shopNameText != null)
            {
                shopNameText.text = shop.shopName;
            }

            // Update gold
            UpdateGoldDisplay();

            // Clear existing items
            if (itemListParent != null)
            {
                foreach (Transform child in itemListParent)
                {
                    Destroy(child.gameObject);
                }
            }

            // Get items to display
            List<ShopItem> items = isBuyMode ? shop.GetAvailableItems() : GetPlayerItems();

            // Create item list
            foreach (ShopItem item in items)
            {
                CreateShopItem(item);
            }

            // Update buttons
            UpdateModeButtons();
        }

        /// <summary>
        /// Get player items for sell mode.
        /// </summary>
        private List<ShopItem> GetPlayerItems()
        {
            // TODO: Get from InventoryManager
            // For now, return empty list
            return new List<ShopItem>();
        }

        /// <summary>
        /// Create shop item in list.
        /// </summary>
        private void CreateShopItem(ShopItem item)
        {
            if (itemListParent == null) return;

            // Create item
            GameObject itemObj = new GameObject("ShopItem_" + item.itemName);
            itemObj.transform.SetParent(itemListParent, false);

            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 60);

            // Add LayoutElement
            LayoutElement layoutElement = itemObj.AddComponent<LayoutElement>();
            layoutElement.minHeight = 60;
            layoutElement.preferredHeight = 60;

            Image bg = itemObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            Button button = itemObj.AddComponent<Button>();

            // Item Name Text
            GameObject nameObj = new GameObject("ItemName");
            nameObj.transform.SetParent(itemObj.transform, false);

            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.5f);
            nameRect.anchorMax = new Vector2(0.6f, 1);
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-5, -5);

            Text nameText = nameObj.AddComponent<Text>();
            nameText.text = item.itemName;
            nameText.fontSize = 14;
            nameText.fontStyle = FontStyle.Bold;
            nameText.alignment = TextAnchor.MiddleLeft;
            nameText.color = Color.white;
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Price Text
            GameObject priceObj = new GameObject("Price");
            priceObj.transform.SetParent(itemObj.transform, false);

            RectTransform priceRect = priceObj.AddComponent<RectTransform>();
            priceRect.anchorMin = new Vector2(0.6f, 0.5f);
            priceRect.anchorMax = new Vector2(1, 1);
            priceRect.offsetMin = new Vector2(5, 0);
            priceRect.offsetMax = new Vector2(-10, -5);

            Text priceText = priceObj.AddComponent<Text>();
            int price = isBuyMode ? ShopManager.Instance.GetCurrentShop().GetBuyPrice(item) : ShopManager.Instance.GetCurrentShop().GetSellPrice(item);
            priceText.text = $"{price}G";
            priceText.fontSize = 14;
            priceText.alignment = TextAnchor.MiddleRight;
            priceText.color = Color.yellow;
            priceText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Stock Text
            GameObject stockObj = new GameObject("Stock");
            stockObj.transform.SetParent(itemObj.transform, false);

            RectTransform stockRect = stockObj.AddComponent<RectTransform>();
            stockRect.anchorMin = new Vector2(0, 0);
            stockRect.anchorMax = new Vector2(0.6f, 0.5f);
            stockRect.offsetMin = new Vector2(10, 5);
            stockRect.offsetMax = new Vector2(-5, 0);

            Text stockText = stockObj.AddComponent<Text>();
            stockText.text = item.stock == -1 ? "Stock: ∞" : $"Stock: {item.stock}";
            stockText.fontSize = 12;
            stockText.alignment = TextAnchor.MiddleLeft;
            stockText.color = new Color(0.7f, 0.7f, 0.7f);
            stockText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Level Text
            GameObject levelObj = new GameObject("Level");
            levelObj.transform.SetParent(itemObj.transform, false);

            RectTransform levelRect = levelObj.AddComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0.6f, 0);
            levelRect.anchorMax = new Vector2(1, 0.5f);
            levelRect.offsetMin = new Vector2(5, 5);
            levelRect.offsetMax = new Vector2(-10, 0);

            Text levelText = levelObj.AddComponent<Text>();
            levelText.text = $"Req. Lv.{item.requiredLevel}";
            levelText.fontSize = 12;
            levelText.alignment = TextAnchor.MiddleRight;
            levelText.color = new Color(0.7f, 0.7f, 0.7f);
            levelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Add click listener
            ShopItem itemRef = item;
            button.onClick.AddListener(() => SelectItem(itemRef));
        }

        /// <summary>
        /// Select item.
        /// </summary>
        private void SelectItem(ShopItem item)
        {
            selectedItem = item;
            quantity = 1;

            // Update detail display
            UpdateItemDetail();
        }

        /// <summary>
        /// Update item detail display.
        /// </summary>
        private void UpdateItemDetail()
        {
            if (selectedItem == null) return;

            if (itemDetailNameText != null)
            {
                itemDetailNameText.text = selectedItem.itemName;
            }

            if (itemDetailDescText != null)
            {
                itemDetailDescText.text = $"Level Required: {selectedItem.requiredLevel}";
            }

            if (itemDetailPriceText != null)
            {
                int price = isBuyMode ?
                    ShopManager.Instance.GetCurrentShop().GetBuyPrice(selectedItem) :
                    ShopManager.Instance.GetCurrentShop().GetSellPrice(selectedItem);
                int totalPrice = price * quantity;
                itemDetailPriceText.text = $"Total: {totalPrice}G";
            }

            if (quantityText != null)
            {
                quantityText.text = quantity.ToString();
            }
        }

        /// <summary>
        /// Update gold display.
        /// </summary>
        private void UpdateGoldDisplay()
        {
            if (goldText != null && ShopManager.Instance != null)
            {
                goldText.text = $"Gold: {ShopManager.Instance.GetPlayerGold()}";
            }
        }

        /// <summary>
        /// Update mode buttons.
        /// </summary>
        private void UpdateModeButtons()
        {
            if (buyButton != null)
            {
                Image buyImg = buyButton.GetComponent<Image>();
                if (buyImg != null)
                {
                    buyImg.color = isBuyMode ? new Color(0.3f, 0.6f, 0.3f) : new Color(0.3f, 0.3f, 0.3f);
                }
            }

            if (sellButton != null)
            {
                Image sellImg = sellButton.GetComponent<Image>();
                if (sellImg != null)
                {
                    sellImg.color = !isBuyMode ? new Color(0.6f, 0.3f, 0.3f) : new Color(0.3f, 0.3f, 0.3f);
                }
            }
        }

        /// <summary>
        /// Close shop.
        /// </summary>
        private void CloseShop()
        {
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.CloseShop();
            }
        }

        /// <summary>
        /// On buy button clicked.
        /// </summary>
        private void OnBuyClicked()
        {
            if (selectedItem == null || ShopManager.Instance == null) return;

            ShopManager.Instance.BuyItem(selectedItem.itemID, quantity);
            RefreshShop();
            UpdateItemDetail();
        }

        /// <summary>
        /// On sell button clicked.
        /// </summary>
        private void OnSellClicked()
        {
            if (selectedItem == null || ShopManager.Instance == null) return;

            ShopManager.Instance.SellItem(selectedItem.itemID, quantity);
            RefreshShop();
            UpdateItemDetail();
        }

        /// <summary>
        /// On quantity changed.
        /// </summary>
        private void OnQuantityChanged(string value)
        {
            if (int.TryParse(value, out int newQuantity))
            {
                quantity = Mathf.Max(1, newQuantity);
                UpdateItemDetail();
            }
        }

        // Event handlers
        private void OnShopOpened(ShopData shop)
        {
            ShowPanel();
        }

        private void OnShopClosed(ShopData shop)
        {
            HidePanel();
        }

        private void OnItemBought(ShopItem item, int amount)
        {
            Debug.Log($"[ShopUI] Bought {amount}x {item.itemName}");
        }

        private void OnItemSold(ShopItem item, int amount)
        {
            Debug.Log($"[ShopUI] Sold {amount}x {item.itemName}");
        }

        private void OnGoldChanged(int newGold)
        {
            UpdateGoldDisplay();
        }

        /// <summary>
        /// Auto-create shop UI.
        /// </summary>
        private void CreateShopUI()
        {
            // Find Canvas
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[ShopUI] Canvas not found!");
                return;
            }

            // Create Shop Panel
            shopPanel = new GameObject("ShopPanel");
            shopPanel.transform.SetParent(canvas.transform, false);

            RectTransform panelRect = shopPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.15f, 0.1f);
            panelRect.anchorMax = new Vector2(0.85f, 0.9f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelBg = shopPanel.AddComponent<Image>();
            panelBg.color = new Color(0, 0, 0, 0.9f);

            // Shop Name
            shopNameText = CreateTextElement("ShopName", "Shop", 20, TextAnchor.MiddleCenter, Color.yellow);
            shopNameText.rectTransform.SetParent(shopPanel.transform, false);
            shopNameText.rectTransform.anchorMin = new Vector2(0, 0.9f);
            shopNameText.rectTransform.anchorMax = new Vector2(1, 1);
            shopNameText.rectTransform.offsetMin = new Vector2(10, 0);
            shopNameText.rectTransform.offsetMax = new Vector2(-10, -5);

            // Gold Display
            goldText = CreateTextElement("GoldText", "Gold: 1000", 16, TextAnchor.MiddleRight, Color.yellow);
            goldText.rectTransform.SetParent(shopPanel.transform, false);
            goldText.rectTransform.anchorMin = new Vector2(0.5f, 0.9f);
            goldText.rectTransform.anchorMax = new Vector2(1, 1);
            goldText.rectTransform.offsetMin = new Vector2(10, 0);
            goldText.rectTransform.offsetMax = new Vector2(-10, -5);

            // Create item list parent
            itemListParent = CreatePanel("ItemList", shopPanel.transform,
                new Vector2(0, 0.1f), new Vector2(0.5f, 0.9f),
                new Vector2(10, 10), new Vector2(-5, -10));

            // Add VerticalLayoutGroup
            VerticalLayoutGroup layout = itemListParent.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5;
            layout.padding = new RectOffset(5, 5, 5, 5);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Create detail panel
            CreateDetailPanel();

            // Create mode buttons
            CreateModeButtons();

            // Create close button
            closeButton = CreateButton("CloseButton", "X", new Vector2(0.9f, 0.9f), new Vector2(1, 1));
            closeButton.onClick.AddListener(CloseShop);

            // Hide panel
            shopPanel.SetActive(false);

            Debug.Log("[ShopUI] Shop UI created!");
        }

        private void CreateDetailPanel()
        {
            GameObject detailPanel = new GameObject("DetailPanel");
            detailPanel.transform.SetParent(shopPanel.transform, false);

            RectTransform detailRect = detailPanel.AddComponent<RectTransform>();
            detailRect.anchorMin = new Vector2(0.5f, 0.1f);
            detailRect.anchorMax = new Vector2(1, 0.9f);
            detailRect.offsetMin = new Vector2(5, 10);
            detailRect.offsetMax = new Vector2(-10, -10);

            Image detailBg = detailPanel.AddComponent<Image>();
            detailBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Item Name
            itemDetailNameText = CreateTextElement("ItemName", "Select Item", 18, TextAnchor.UpperLeft, Color.white);
            itemDetailNameText.rectTransform.SetParent(detailPanel.transform, false);
            itemDetailNameText.rectTransform.anchorMin = new Vector2(0, 0.8f);
            itemDetailNameText.rectTransform.anchorMax = new Vector2(1, 1);
            itemDetailNameText.rectTransform.offsetMin = new Vector2(10, 0);
            itemDetailNameText.rectTransform.offsetMax = new Vector2(-10, -5);

            // Item Description
            itemDetailDescText = CreateTextElement("ItemDesc", "Description", 14, TextAnchor.UpperLeft, Color.white);
            itemDetailDescText.rectTransform.SetParent(detailPanel.transform, false);
            itemDetailDescText.rectTransform.anchorMin = new Vector2(0, 0.6f);
            itemDetailDescText.rectTransform.anchorMax = new Vector2(1, 0.8f);
            itemDetailDescText.rectTransform.offsetMin = new Vector2(10, 0);
            itemDetailDescText.rectTransform.offsetMax = new Vector2(-10, 0);

            // Price
            itemDetailPriceText = CreateTextElement("Price", "Price: 0G", 16, TextAnchor.MiddleCenter, Color.yellow);
            itemDetailPriceText.rectTransform.SetParent(detailPanel.transform, false);
            itemDetailPriceText.rectTransform.anchorMin = new Vector2(0, 0.4f);
            itemDetailPriceText.rectTransform.anchorMax = new Vector2(1, 0.6f);
            itemDetailPriceText.rectTransform.offsetMin = new Vector2(10, 0);
            itemDetailPriceText.rectTransform.offsetMax = new Vector2(-10, 0);

            // Quantity
            CreateQuantityUI(detailPanel.transform);

            // Buy/Sell buttons
            buyButton = CreateButton("BuyButton", "Buy", new Vector2(0, 0), new Vector2(0.5f, 0.2f));
            sellButton = CreateButton("SellButton", "Sell", new Vector2(0.5f, 0), new Vector2(1, 0.2f));
        }

        private void CreateQuantityUI(Transform parent)
        {
            // Quantity label
            Text quantityLabel = CreateTextElement("QuantityLabel", "Qty:", 14, TextAnchor.MiddleLeft, Color.white);
            quantityLabel.rectTransform.SetParent(parent, false);
            quantityLabel.rectTransform.anchorMin = new Vector2(0, 0.2f);
            quantityLabel.rectTransform.anchorMax = new Vector2(0.2f, 0.4f);
            quantityLabel.rectTransform.offsetMin = new Vector2(10, 0);
            quantityLabel.rectTransform.offsetMax = Vector2.zero;

            // Quantity input
            GameObject inputObj = new GameObject("QuantityInput");
            inputObj.transform.SetParent(parent, false);

            RectTransform inputRect = inputObj.AddComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0.2f, 0.2f);
            inputRect.anchorMax = new Vector2(0.5f, 0.4f);
            inputRect.offsetMin = Vector2.zero;
            inputRect.offsetMax = Vector2.zero;

            Image inputBg = inputObj.AddComponent<Image>();
            inputBg.color = new Color(0.3f, 0.3f, 0.3f);

            quantityInput = inputObj.AddComponent<InputField>();

            // Input text
            GameObject inputTextObj = new GameObject("Text");
            inputTextObj.transform.SetParent(inputObj.transform, false);

            RectTransform inputTextRect = inputTextObj.AddComponent<RectTransform>();
            inputTextRect.anchorMin = Vector2.zero;
            inputTextRect.anchorMax = Vector2.one;
            inputTextRect.offsetMin = new Vector2(5, 0);
            inputTextRect.offsetMax = new Vector2(-5, 0);

            Text inputText = inputTextObj.AddComponent<Text>();
            inputText.text = "1";
            inputText.fontSize = 14;
            inputText.alignment = TextAnchor.MiddleCenter;
            inputText.color = Color.white;
            inputText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            quantityInput.textComponent = inputText;
            quantityText = inputText;
        }

        private void CreateModeButtons()
        {
            // Buy mode button
            Button buyModeBtn = CreateButton("BuyModeBtn", "Buy Mode", new Vector2(0, 0.9f), new Vector2(0.25f, 1));
            buyModeBtn.onClick.AddListener(() => { isBuyMode = true; RefreshShop(); });

            // Sell mode button
            Button sellModeBtn = CreateButton("SellModeBtn", "Sell Mode", new Vector2(0.25f, 0.9f), new Vector2(0.5f, 1));
            sellModeBtn.onClick.AddListener(() => { isBuyMode = false; RefreshShop(); });
        }

        // Helper methods
        private Transform CreatePanel(string name, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            return panel.transform;
        }

        private Text CreateTextElement(string name, string text, int fontSize, TextAnchor alignment, Color color)
        {
            GameObject textObj = new GameObject(name);
            Text textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = fontSize;
            textComp.alignment = alignment;
            textComp.color = color;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return textComp;
        }

        private Button CreateButton(string name, string text, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(shopPanel.transform, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(5, 5);
            rect.offsetMax = new Vector2(-5, -5);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.3f, 0.5f, 0.3f);

            Button button = buttonObj.AddComponent<Button>();

            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = 14;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.color = Color.white;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return button;
        }
    }
}
