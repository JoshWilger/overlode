using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private Mining miningScript;
    [SerializeField] private ItemUsage itemUsageScript;
    [SerializeField] private Toggle inventoryToggle;
    [SerializeField] private GameObject inventoryActive;
    [SerializeField] private GameObject selectedItem;
    [SerializeField] private TextMeshProUGUI storageText;
    [SerializeField] private Image storageProgress;
    [SerializeField] private float itemCooldown;
    [SerializeField] public float disabledTransparency;

    public Toggle activeToggle;

    private ToggleGroup toggleGroup;
    private Toggle[] toggles;
    private Button[] mineralButtons;
    private ItemClass[] minerals;
    private ItemClass[] shopItems;
    private bool canActivateItem = true;
    private TextMeshProUGUI[] invTexts;
    private Image[] shopImages;

    private enum ShopItemTypes { energy, health, teleport, dynamite, c4, block }

    // Start is called before the first frame update
    private void Start()
    {
        atlas.currentUpgradeAmounts = new float[] { 10f, 20f, 7f, 180f, 50f, 80f }; // Max: 150, 120, 120, 180, 80, 80
        toggleGroup = inventoryActive.GetComponent<ToggleGroup>();
        toggles = inventoryActive.GetComponentsInChildren<Toggle>();
        mineralButtons = inventoryActive.GetComponentsInChildren<Button>();
        minerals = atlas.CreateInstance(ItemClass.ItemType.mineral);
        activeToggle = toggles[0];

        foreach (var item in toggles)
        {
            item.onValueChanged.AddListener(SelectedItem);
        }
        for (int i = 0; i < mineralButtons.Length; i++)
        {
            int b = i;
            mineralButtons[i].onClick.AddListener(() => RemoveMineral(b));
        }
        inventoryToggle.onValueChanged.AddListener((value) =>
        {
            InventoryActive(value);
        });
        selectedItem.GetComponent<Button>().onClick.AddListener(UseItemKeyPressed);

        shopItems = atlas.CreateInstance(ItemClass.ItemType.shopItem, false);
        foreach (var item in shopItems)
        {
            item.amountCollected = 100;
        }

        invTexts = RetrieveInventoryText(ItemClass.ItemType.miscGround);
        foreach (var item in invTexts)
        {
            item.alpha = disabledTransparency;
        }
        shopImages = RetrieveInventoryImage(ItemClass.ItemType.shopItem);
        foreach (var item in shopImages)
        {
            item.color = new(item.color.r, item.color.g, item.color.b, disabledTransparency);
        }
        foreach (var item in minerals)
        {
            item.amountCollected = 0;
        }
        UpdateMineralInfo();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!enabled) return;

        if (Input.GetButtonDown("Jump")) UseItemKeyPressed();
        InventoryKeyPressed(Input.GetKeyDown(KeyCode.E));

        Vector2 mouseScrolling = Input.mouseScrollDelta;

        if (mouseScrolling.y > 0)
        {
            ChangeActiveToggle(-1);
        }
        if (mouseScrolling.y < 0)
        {
            ChangeActiveToggle(1);
        }
    }

    private void UseItemKeyPressed()
    {
        shopItems = atlas.CreateInstance(ItemClass.ItemType.shopItem, false);
        int currentIndex = Array.IndexOf(toggles, activeToggle);
        TextMeshProUGUI selectedItemQuantityText = selectedItem.GetComponentInChildren<TextMeshProUGUI>();

        if (canActivateItem &&  
            shopItems[currentIndex].amountCollected - 1 >= 0 && 
            selectedItemQuantityText.text != "")
        {
            canActivateItem = false;

            bool success = false;
            switch (currentIndex)
            {
                case (int)ShopItemTypes.energy:
                    success = itemUsageScript.ActivateEnergy();
                    break;

                case (int)ShopItemTypes.health:
                    success = itemUsageScript.ActivateHealth();
                    break;

                case (int)ShopItemTypes.teleport:
                    success = itemUsageScript.ActivateTeleport();
                    break;

                case (int)ShopItemTypes.dynamite:
                    success = itemUsageScript.ActivateDynamite();
                    break;

                case (int)ShopItemTypes.c4:
                    success = itemUsageScript.ActivateC4();
                    break;

                case (int)ShopItemTypes.block:
                    success = itemUsageScript.ActivateBlock(shopItems[currentIndex]);
                    break;

                default:
                    break;
            }
            if (success)
            {
                Invoke(nameof(ActivateItemTrue), itemCooldown);
                shopItems[currentIndex].amountCollected--;

                UpdateInfo();
            }
            else
            {
                canActivateItem = true;
            }

        }
    }

    public void UpdateInfo()
    {
        TextMeshProUGUI selectedItemQuantityText = selectedItem.GetComponentInChildren<TextMeshProUGUI>();
        Image selectedItemImage = selectedItem.GetComponentsInChildren<Image>().Last();
        int currentIndex = Array.IndexOf(toggles, activeToggle);
        var toggleText = activeToggle.GetComponentInChildren<TextMeshProUGUI>();

        toggleText.text = "x" + shopItems[currentIndex].amountCollected;

        var color = shopImages[currentIndex].color;
        var selTextColor = selectedItemQuantityText.color;
        var selImageColor = selectedItemImage.color;
        if (shopItems[currentIndex].amountCollected == 0)
        {
            invTexts[currentIndex].alpha = disabledTransparency;
            shopImages[currentIndex].color = new(color.r, color.g, color.b, disabledTransparency);
            selectedItemQuantityText.color = new(selTextColor.r, selTextColor.g, selTextColor.b, disabledTransparency);
            selectedItemImage.color = new(selImageColor.r, selImageColor.g, selImageColor.b, disabledTransparency);
        }
        else
        {
            invTexts[currentIndex].alpha = 1;
            shopImages[currentIndex].color = new(color.r, color.g, color.b, 1);
            selectedItemQuantityText.color = new(selTextColor.r, selTextColor.g, selTextColor.b, 1);
            selectedItemImage.color = new(selImageColor.r, selImageColor.g, selImageColor.b, 1);
        }

        selectedItemQuantityText.text = activeToggle.GetComponentInChildren<TextMeshProUGUI>().text;        
    }

    private void ActivateItemTrue()
    {
        canActivateItem = true;
    }

    private void InventoryKeyPressed(bool isPressed)
    {
        if (isPressed)
        {
            inventoryToggle.isOn = !inventoryToggle.isOn;
        }
    }

    private void InventoryActive(bool toggledOn)
    {
        inventoryActive.SetActive(toggledOn);
        selectedItem.SetActive(!toggledOn);
    }

    public TextMeshProUGUI[] RetrieveInventoryText(ItemClass.ItemType type)
    {
        TextMeshProUGUI[] children = inventoryActive.GetComponentsInChildren<TextMeshProUGUI>();
        int amount = type == ItemClass.ItemType.mineral ? children.Length - 6 : type == ItemClass.ItemType.shopItem ? 6 : children.Length;
        TextMeshProUGUI[] inventoryTexts = new TextMeshProUGUI[amount];

        for (int i = 0; i < amount; i++)
        {
            if (type == ItemClass.ItemType.mineral)
            {
                inventoryTexts[i] = children[i + 6];
            }
            else
            {
                inventoryTexts[i] = children[i];
            }
        }

        return inventoryTexts;
    }

    public Image[] RetrieveInventoryImage(ItemClass.ItemType type)
    {
        if (type == ItemClass.ItemType.shopItem)
        {
            Image[] images = new Image[toggles.Length];

            for (int i = 0; i < toggles.Length; i++)
            {
                images[i] = toggles[i].image;
            }

            return images;
        }
        else
        {
            Image[] children = inventoryActive.GetComponentsInChildren<Image>();

            return type == ItemClass.ItemType.mineral ? children.Where((mineral) => mineral.name.Contains("Mineral")).ToArray() : children;
        }
    }

    private void SelectedItem(bool value)
    {
        if (value)
        {
            var maybeActiveToggle = toggleGroup.ActiveToggles().FirstOrDefault();
            if (maybeActiveToggle)
            {
                activeToggle = maybeActiveToggle;
            }
            var selectedImage = selectedItem.GetComponentsInChildren<Image>().Last();
            var activeImage = activeToggle.GetComponentInChildren<Image>();

            selectedImage.sprite = activeImage.sprite;

            selectedItem.GetComponentInChildren<TextMeshProUGUI>().text = activeToggle.GetComponentInChildren<TextMeshProUGUI>().text;

            selectedImage.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f);
            UpdateInfo();
        }
    }

    private void ChangeActiveToggle(int relativeIndex)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                int adder = i + relativeIndex;
                int newActiveIndex = adder < 0 ? adder + toggles.Length : adder > toggles.Length - 1 ? adder - toggles.Length : adder;

                toggles[newActiveIndex].isOn = true;
                toggles[i].isOn = false;

                activeToggle = toggles[newActiveIndex];
                toggleGroup.EnsureValidState();

                SelectedItem(true);

                return;
            }
        }
    }

    private void RemoveMineral(int mineralNumber)
    {
        if (minerals[mineralNumber].amountCollected > 0)
        {
            minerals[mineralNumber].amountCollected--;
            UpdateMineralInfo();
        }
    }

    public void UpdateMineralInfo()
    {
        var mineralTexts = RetrieveInventoryText(ItemClass.ItemType.mineral);
        var mineralImages = RetrieveInventoryImage(ItemClass.ItemType.mineral);

        for (int i = 0; i < minerals.Length; i++)
        {
            var currentAmount = minerals[i].amountCollected;
            var alpha = currentAmount > 0 ? 1 : disabledTransparency;

            mineralTexts[i].text = "x" + currentAmount;
            mineralTexts[i].alpha = alpha;

            var color = mineralImages[i].color;
            mineralImages[i].color = new(color.r, color.g, color.b, alpha);
        }

        UpdateStorageProgress();
    }

    public void UpdateStorageProgress()
    {
        var collection = miningScript.CountCollectedMinerals();
        var space = atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.storage];

        storageProgress.fillAmount = collection / space;
        storageText.text = Mathf.RoundToInt(100f * (collection / space)) + "%";
    }
}
