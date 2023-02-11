using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [SerializeField] private GameObject bossBar;
    [SerializeField] private Image bossImage;
    [SerializeField] private Image bossHealth;
    [SerializeField] private Sprite golemSprite;
    [SerializeField] public float bossDamageDivisor;

    private float health;
    public bool nextBoss;

    private void OnEnable()
    {
        health = 1;
        nextBoss = false;
        bossBar.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (health <= 0 && !nextBoss)
        {
            nextBoss = true;
            GetComponent<ChickenMovement>().enabled = false;
            GetComponent<GolemMovement>().enabled = true;
            bossImage.sprite = golemSprite;
            health = 1;
            UpdateHealth();
        }
    }

    public void UpdateHealth(float damage = 0)
    {
        if (health > 0)
        {
            health -= damage;
            bossHealth.fillAmount = health;
        }
    }
}
