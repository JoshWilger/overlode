using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryUI : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private UIController controller;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button sellButton;

    private TextMeshProUGUI total;
    private TextMeshProUGUI[] quantity;
    private TextMeshProUGUI[] mineralInfo;
    private ItemClass[] minerals;
    private TextMeshProUGUI[] inventoryMineralTexts;

    // Start is called before the first frame update
    private void Awake()
    {
        TextMeshProUGUI[] children = GetComponentsInChildren<TextMeshProUGUI>();
        quantity = new TextMeshProUGUI[children.Length / 2 - 1];
        mineralInfo = new TextMeshProUGUI[children.Length / 2 - 1];
        inventoryMineralTexts = controller.RetrieveInventoryText(ItemClass.ItemType.mineral);

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

        UpdateText();
    }

    private void OnDisable()
    {
        sellButton.onClick.RemoveAllListeners();
    }

    private void UpdateText()
    {
        minerals = atlas.CreateInstance(ItemClass.ItemType.mineral);
        long totalCounter = 0;

        for (int i = 0; i < minerals.Length; i++)
        {
            long addMe = minerals[i].amountCollected * minerals[i].itemWorth;

            quantity[i].text = "x" + minerals[i].amountCollected;
            inventoryMineralTexts[i].text = "x" + minerals[i].amountCollected;
            mineralInfo[i].text = minerals[i].itemName + " \t($" + minerals[i].itemWorth + ")\n$" + addMe;
            totalCounter += addMe;
        }

        total.text = "Total: $" + totalCounter;
    }

    private void SellAll()
    {
        long tempTotal = long.Parse(total.text.Substring(8));

        foreach (var item in minerals)
        {
            item.amountCollected = 0;
        }
        UpdateText();
        moneyText.text = "$" + (long.Parse(moneyText.text.Substring(1)) + tempTotal);
    }
}
