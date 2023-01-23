using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageUI : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private Mining miningScript;
    [SerializeField] private TextMeshProUGUI storageText;
    [SerializeField] private Image storageProgress;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI upgradeInfoText;
    [SerializeField] private TextMeshProUGUI typeInfoText;
    [SerializeField] private TextMeshProUGUI typeTitleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private Button incrementButton;
    [SerializeField] private Button decrementButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Image part;

    private ItemClass[][] upgrades;
    private int[] equippedUpgrades;
    private ToggleGroup toggleGroup;
    private Toggle[] toggles;
    private int currentToggleIndex = 0;
    private int currentUpgradeIndex = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        toggleGroup = GetComponentInChildren<ToggleGroup>();
        toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        upgrades = atlas.CreateMultiInstance(ItemClass.ItemType.upgrade);
        equippedUpgrades = Enumerable.Repeat(0, upgrades.Length).ToArray();

        foreach (var item in upgrades)
        {
            foreach (var i in item)
            {
                i.amountCollected = 0;
            }
        }
    }

    private void OnEnable()
    {
        foreach (var item in toggles)
        {
            item.onValueChanged.AddListener(UpdateTypeInfo);
        }
        incrementButton.onClick.AddListener(IncrementUpgrade);
        decrementButton.onClick.AddListener(DecrementUpgrade);
        purchaseButton.onClick.AddListener(PurchaseUpgrade);

        UpdateUpgradeText();
    }

    private void OnDisable()
    {
        incrementButton.onClick.RemoveAllListeners();
        decrementButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.RemoveAllListeners();
        foreach (var item in toggles)
        {
            item.onValueChanged.RemoveAllListeners();
        }
    }

    private void UpdateTypeInfo(bool value)
    {
        var selectedImage = toggleGroup.GetFirstActiveToggle().image;
        part.sprite = selectedImage.sprite;

        currentToggleIndex = Array.IndexOf(toggles, toggleGroup.ActiveToggles().FirstOrDefault());

        currentUpgradeIndex = 0;
        var upgradeType = upgrades[currentToggleIndex][currentUpgradeIndex].upgradeType;
        typeTitleText.text = upgradeType.itemName;
        typeInfoText.text = upgradeType.itemDescription;
        UpdateUpgradeText();
    }

    private void UpdateUpgradeText()
    {
        var upgrade = upgrades[currentToggleIndex][currentUpgradeIndex];

        upgradeInfoText.text = upgrade.itemDescription + "\n$" + upgrade.itemWorth;
        levelText.text = "Level " + upgrade.itemName;
        currentLevelText.text = "Current Level: " + equippedUpgrades[currentToggleIndex];

        purchaseButton.interactable = long.Parse(moneyText.text.Substring(1)) - upgrade.itemWorth >= 0 && upgrade.amountCollected == 0;
        purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.amountCollected == 0 ? "Purchase" : "Bought";
        incrementButton.interactable = upgrades[currentToggleIndex].Length - 1 > currentUpgradeIndex;
        decrementButton.interactable = currentUpgradeIndex > 0;
    }

    private void IncrementUpgrade()
    {
        Mathf.Clamp(currentUpgradeIndex++, 0, upgrades[currentToggleIndex].Length);
        UpdateUpgradeText();
    }

    private void DecrementUpgrade()
    {
        Mathf.Clamp(currentUpgradeIndex--, 0, upgrades[currentToggleIndex].Length);
        UpdateUpgradeText();
    }

    private void PurchaseUpgrade()
    {
        long money = long.Parse(moneyText.text.Substring(1));

        if (money - upgrades[currentToggleIndex][currentUpgradeIndex].itemWorth >= 0)
        {
            moneyText.text = "$" + (money - upgrades[currentToggleIndex][currentUpgradeIndex].itemWorth);
            upgrades[currentToggleIndex][currentUpgradeIndex].amountCollected++;
            equippedUpgrades[currentToggleIndex] = currentUpgradeIndex + 1;
            atlas.currentUpgradeAmounts[currentToggleIndex] = 
                float.Parse(upgrades[currentToggleIndex][currentUpgradeIndex].itemDescription.Where(c => char.IsDigit(c)).ToArray());

            if (currentToggleIndex == (int)ItemAtlas.UpgradeTypes.storage)
            {
                var collection = miningScript.CountCollectedMinerals();
                var space = atlas.currentUpgradeAmounts[currentToggleIndex];

                storageProgress.fillAmount = collection / space;
                storageText.text = Mathf.RoundToInt(100f * (collection / space)) + "%";
            }
        }

        UpdateUpgradeText();
    }
}
