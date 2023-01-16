using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health healthScript;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private float pricePerLiter;
    [SerializeField] private Image healthProgress;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button refillButton;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateHealth();
    }

    void OnEnable()
    {
        refillButton.onClick.AddListener(RefillHealth);
    }

    private void OnDisable()
    {
        refillButton.onClick.RemoveAllListeners();
    }

    private void UpdateHealth()
    {
        long money = long.Parse(moneyText.text.Substring(1));
        int cost = Mathf.CeilToInt((1 - healthScript.health) * 10f * pricePerLiter);
        
        healthProgress.fillAmount = healthScript.health;

        progressText.text = Mathf.FloorToInt(healthScript.health * 10f) + " /  10 HP";
        costText.text = "$" + cost;

        refillButton.interactable = money > 0 && healthScript.health != 1;
    }

    private void RefillHealth()
    {
        long money = long.Parse(moneyText.text.Substring(1));
        int cost = Mathf.CeilToInt((1 - healthScript.health) * 10f * pricePerLiter);

        if (money - cost >= 0)
        {
            moneyText.text = "$" + (money - cost);
            healthScript.health = 1;
            UpdateHealth();
            healthScript.UpdateHealthBar();
        }
        else
        {
            healthScript.health += money / pricePerLiter / 10f;
            moneyText.text = "$0";
            UpdateHealth();
            healthScript.UpdateHealthBar();
        }
    }
}
