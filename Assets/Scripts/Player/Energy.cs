using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    [SerializeField] private Image energyBar;

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
