using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryUI : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private HudUI controller;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button sellButton;

    private TextMeshProUGUI total;
    private TextMeshProUGUI[] quantity;
    private TextMeshProUGUI[] mineralInfo;
    private ItemClass[] minerals;
    private TextMeshProUGUI[] inventoryMineralTexts;
    private Image[] inventoryMineralImages;
    private Image[] mineralImages;

    // Start is called before the first frame update
    private void Awake()
    {
        TextMeshProUGUI[] children = GetComponentsInChildren<TextMeshProUGUI>();
        quantity = new TextMeshProUGUI[children.Length / 2 - 1];
        mineralInfo = new TextMeshProUGUI[children.Length / 2 - 1];
        inventoryMineralTexts = controller.RetrieveInventoryText(ItemClass.ItemType.mineral);
        inventoryMineralImages = controller.RetrieveInventoryImage(ItemClass.ItemType.mineral);
        mineralImages = GetComponentsInChildren<Image>().Where((mineral) => mineral.name.Contains("Mineral")).ToArray();

        for (int i = 0; i < children.Length; i++)
        {
            if (i == children.Length - 1)
            {
                total = children[i];
            }
            else if (i % 2 == 0 && i < children.Length - 3)
            {
                quantity[i / 2] = children[i];
            }
            else if (i < children.Length - 3)
            {
                mineralInfo[i / 2] = children[i];
            }
        }
    }


    private void OnEnable()
    {
        sellButton.onClick.AddListener(SellAll);

        UpdateInfo();
    }

    private void OnDisable()
    {
        sellButton.onClick.RemoveAllListeners();
    }

    private void UpdateInfo()
    {
        minerals = atlas.CreateInstance(ItemClass.ItemType.mineral);
        long totalCounter = 0;

        for (int i = 0; i < minerals.Length; i++)
        {
            long addMe = minerals[i].amountCollected * minerals[i].itemWorth;

            UpdateAlpha(i);

            quantity[i].text = "x" + minerals[i].amountCollected;
            inventoryMineralTexts[i].text = "x" + minerals[i].amountCollected;
            mineralInfo[i].text = minerals[i].itemName + " \t($" + minerals[i].itemWorth + ")\n$" + addMe;
            totalCounter += addMe;
        }

        total.text = "Total: $" + totalCounter;
        sellButton.interactable = totalCounter > 0;
    }

    private void UpdateAlpha(int i)
    {
        var color = mineralImages[i].color;
        var invColor = inventoryMineralImages[i].color;

        if (minerals[i].amountCollected == 0)
        {
            mineralImages[i].color = new(color.r, color.g, color.b, controller.disabledTransparency);
            quantity[i].alpha = controller.disabledTransparency;
            mineralInfo[i].alpha = controller.disabledTransparency;
            inventoryMineralImages[i].color = new(invColor.r, invColor.g, invColor.b, controller.disabledTransparency);
            inventoryMineralTexts[i].alpha = controller.disabledTransparency;
        }
        else
        {
            mineralImages[i].color = new(color.r, color.g, color.b, 1);
            quantity[i].alpha = 1;
            mineralInfo[i].alpha = 1;
            inventoryMineralImages[i].color = new(invColor.r, invColor.g, invColor.b, 1);
            inventoryMineralTexts[i].alpha = 1;
        }
    }

    private void SellAll()
    {
        long tempTotal = long.Parse(total.text.Substring(8));

        foreach (var item in minerals)
        {
            item.amountCollected = 0;
        }
        UpdateInfo();
        moneyText.text = "$" + (long.Parse(moneyText.text.Substring(1)) + tempTotal);
    }
}
