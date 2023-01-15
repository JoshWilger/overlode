using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    [SerializeField] private Image energyBar;
    [SerializeField] private TextMeshProUGUI low;
    [SerializeField] private Animator warning;

    public float energy;
    public float energyLossAmount;
    public bool decreaseEnergy;

    // Start is called before the first frame update
    void Start()
    {
        energy = 1;
        decreaseEnergy = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnergy();
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

    private void UpdateEnergy()
    {
        if (energy > 0 && decreaseEnergy)
        {
            energy -= energyLossAmount;
            energyBar.fillAmount = energy;
        }
    }

    public void UpdateEnergyBar()
    {
        energyBar.fillAmount = energy;
    }
}
