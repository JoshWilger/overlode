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
    [SerializeField] private float itemCooldown;
    [SerializeField] public float disabledTransparency;

    private ToggleGroup toggleGroup;
    private Toggle[] toggles;
    public Toggle activeToggle;
    private ItemClass[] shopItems;
    private bool canActivateItem = true;
    private TextMeshProUGUI[] invTexts;
    private Image[] shopImages;

    private enum ShopItemTypes { energy, health, teleport, dynamite, c4, block }

    // Start is called before the first frame update
    private void Start()
    {
        toggleGroup = inventoryActive.GetComponent<ToggleGroup>();
        toggles = inventoryActive.GetComponentsInChildren<Toggle>();
        activeToggle = toggles[0];

        foreach (var item in toggles)
        {
            item.onValueChanged.AddListener(SelectedItem);
        }
        inventoryToggle.onValueChanged.AddListener((value) =>
        {
            InventoryActive(value);
        });
        selectedItem.GetComponent<Button>().onClick.AddListener(UseItemKeyPressed);

        shopItems = atlas.CreateInstance(ItemClass.ItemType.shopItem, false);
        foreach (var item in shopItems)
        {
            item.amountCollected = 10; // TESTING
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
        var mineralImages = RetrieveInventoryImage(ItemClass.ItemType.mineral);
        foreach (var item in mineralImages)
        {
            item.color = new(item.color.r, item.color.g, item.color.b, disabledTransparency);
        }
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
            miningScript.IsGrounded() && 
            shopItems[currentIndex].amountCollected - 1 >= 0 && 
            selectedItemQuantityText.text != "")
        {
            canActivateItem = false;
            Invoke(nameof(ActivateItemTrue), itemCooldown);

            switch (currentIndex)
            {
                case (int)ShopItemTypes.energy:
                    itemUsageScript.ActivateEnergy();
                    break;

                case (int)ShopItemTypes.health:
                    itemUsageScript.ActivateHealth();
                    break;

                case (int)ShopItemTypes.teleport:
                    itemUsageScript.ActivateTeleport();
                    break;

                case (int)ShopItemTypes.dynamite:
                    itemUsageScript.ActivateDynamite();
                    break;

                case (int)ShopItemTypes.c4:
                    itemUsageScript.ActivateC4();
                    break;

                case (int)ShopItemTypes.block:
                    itemUsageScript.ActivateBlock(shopItems[currentIndex]);
                    break;

                default:
                    break;
            }
            shopItems[currentIndex].amountCollected--;

            UpdateInfo();
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
}
