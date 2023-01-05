using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private HudUI controller;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI selectedItemQuantityText;
    [SerializeField] private Button purchaseButton;

    private TextMeshProUGUI shopItemNameText;
    private TextMeshProUGUI shopItemDescriptionText;
    private TextMeshProUGUI[] quantity;
    private ItemClass[] shopItems;
    private TextMeshProUGUI[] inventoryShopItemTexts;
    private ToggleGroup toggleGroup;
    private Toggle[] toggles;
    private int currentIndex = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        inventoryShopItemTexts = controller.RetrieveInventoryText(ItemClass.ItemType.shopItem);
        toggleGroup = GetComponentInChildren<ToggleGroup>();
        toggles = toggleGroup.GetComponentsInChildren<Toggle>();

        TextMeshProUGUI[] children = GetComponentsInChildren<TextMeshProUGUI>();
        quantity = new TextMeshProUGUI[6];

        for (int i = 0; i < 8; i++)
        {
            if (i == 0)
            {
                shopItemNameText = children[i];
            }
            else if (i == 1)
            {
                shopItemDescriptionText = children[i];
            }
            else
            {
                quantity[i - 2] = children[i];
            }
        }

        shopItems = atlas.CreateInstance(ItemClass.ItemType.shopItem, false);
        foreach (var item in shopItems)
        {
            item.amountCollected = 0;
        }
        UpdateText();
    }

    private void OnEnable()
    {
        toggleGroup.SetAllTogglesOff();

        foreach (var item in toggles)
        {
            item.onValueChanged.AddListener(UpdateItemInfo);
        }
        purchaseButton.onClick.AddListener(PurchaseItem);

        UpdateText();
    }

    private void OnDisable()
    {
        purchaseButton.onClick.RemoveAllListeners();
    }

    private void UpdateText()
    {
        shopItems = atlas.CreateInstance(ItemClass.ItemType.shopItem, false);

        for (int i = 0; i < shopItems.Length; i++)
        {
            quantity[i].text = "x" + shopItems[i].amountCollected;
            inventoryShopItemTexts[i].text = "x" + shopItems[i].amountCollected;
        }

        if (selectedItemQuantityText.text != "")
        {
            selectedItemQuantityText.text = controller.activeToggle.GetComponentInChildren<TextMeshProUGUI>().text;
        }
    }

    private void PurchaseItem()
    {
        long money = long.Parse(moneyText.text.Substring(1));

        if (money - shopItems[currentIndex].itemWorth >= 0)
        {
            moneyText.text = "$" + (money - shopItems[currentIndex].itemWorth);
            shopItems[currentIndex].amountCollected++;
        }

        UpdateText();
    }

    private void UpdateItemInfo(bool value)
    {
        if (value)
        {
            currentIndex = Array.IndexOf(toggles, toggleGroup.ActiveToggles().FirstOrDefault());
            shopItemNameText.text = shopItems[currentIndex].itemName;
            shopItemDescriptionText.text = shopItems[currentIndex].itemDescription;
            costText.text = "Cost: $" + shopItems[currentIndex].itemWorth;
        }
    }
}
