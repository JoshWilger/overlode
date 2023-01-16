using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChargingUI : MonoBehaviour
{
    [SerializeField] private Energy energyScript;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private float pricePerLiter;

    private Image energyProgress;
    private TextMeshProUGUI progressText;
    private TextMeshProUGUI costText;
    private Button refillButton;

    // Start is called before the first frame update
    void Awake()
    {
        var images = GetComponentsInChildren<Image>();
        energyProgress = images[2];

        var texts = GetComponentsInChildren<TextMeshProUGUI>();
        progressText = texts[0];
        costText = texts[2];
        refillButton = GetComponentInChildren<Button>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        refillButton.onClick.AddListener(RefillEnergy);
        UpdateEnergy();
    }

    private void OnDisable()
    {
        refillButton.onClick.RemoveAllListeners();
    }

    private void UpdateEnergy()
    {
        long money = long.Parse(moneyText.text.Substring(1));
        int cost = Mathf.CeilToInt((1 - energyScript.energy) * 10f * pricePerLiter);

        energyProgress.fillAmount = energyScript.energy;

        progressText.text = Mathf.FloorToInt(energyScript.energy * 10f) + " /  10 L";
        costText.text = "$" + cost;

        refillButton.interactable = money > 0 && energyScript.energy != 1;
    }

    private void RefillEnergy()
    {
        long money = long.Parse(moneyText.text.Substring(1));
        int cost = Mathf.CeilToInt((1 - energyScript.energy) * 10f * pricePerLiter);

        if (money - cost >= 0)
        {
            moneyText.text = "$" + (money - cost);
            energyScript.energy = 1;
            UpdateEnergy();
            energyScript.UpdateEnergyBar();
        }
        else
        {
            energyScript.energy += money / pricePerLiter / 10f;
            moneyText.text = "$0";
            UpdateEnergy();
            energyScript.UpdateEnergyBar();
        }
    }
}