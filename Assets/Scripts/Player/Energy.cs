using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    [SerializeField] private ItemAtlas atlas;
    [SerializeField] private Image energyBar;
    [SerializeField] private TextMeshProUGUI low;
    [SerializeField] private Animator warning;

    public float energy;
    public bool decreaseEnergy;

    // Start is called before the first frame update
    void Start()
    {
        energy = 0.55f;
        decreaseEnergy = true;
    }

    private void FixedUpdate()
    {
        UpdateEnergy(1000f);
        if (energy < 0.05)
        {
            low.text = "Critical!";
            warning.speed = 8f;
            warning.SetBool("show", true);
        }
        else if (energy < 0.2)
        {
            low.text = "Low";
            warning.speed = 1f;
            warning.SetBool("show", true);
        }

        else
        {
            warning.SetBool("show", false);
        }
    }

    public void UpdateEnergy(float divisor)
    {
        if (energy > 0 && decreaseEnergy)
        {
            energy -= 1f / (divisor * atlas.currentUpgradeAmounts[(int)ItemAtlas.UpgradeTypes.battery]);
            energyBar.fillAmount = energy;
        }
    }

    public void UpdateEnergyBar()
    {
        energyBar.fillAmount = energy;
    }
}
