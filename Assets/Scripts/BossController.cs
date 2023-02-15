using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [SerializeField] private UIController uiController;
    [SerializeField] private ItemUsage itemUsageScript;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject focus;
    [SerializeField] private GameObject bossBar;
    [SerializeField] private Image bossImage;
    [SerializeField] private Image bossHealth;
    [SerializeField] private Sprite chickSprite;
    [SerializeField] private Sprite golemSprite;
    [SerializeField] public float bossDamageDivisor;
    [SerializeField] public float golemSpawnDelay;

    private ChickenMovement chick;
    private GolemMovement golem;
    private Animator focusAnim;
    private float health;
    public bool nextBoss;

    private void OnEnable()
    {
        golem = GetComponent<GolemMovement>();
        chick = GetComponent<ChickenMovement>();
        focusAnim = focus.GetComponent<Animator>();
        chick.enabled = true;
        nextBoss = false;
        health = 1;
        UpdateHealth();
        bossImage.sprite = chickSprite;
        bossBar.SetActive(true);
    }

    private void OnDisable()
    {
        golem.enabled = false;
        chick.enabled = false;
        bossBar.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (health <= 0 && !nextBoss)
        {
            nextBoss = true; 
            health = 1;
            UpdateHealth();
            bossImage.sprite = golemSprite;
            chick.enabled = false;
            bossBar.SetActive(false);
            Invoke(nameof(SpawnGolem), golemSpawnDelay);
        }
        else if (health <= 0 && nextBoss)
        {
            health = 0.1f;
            bossBar.SetActive(false);
            golem.enabled = false;
            Invoke(nameof(EndingSequence), golemSpawnDelay);
        }
    }

    private void SpawnGolem()
    {
        golem.enabled = true;
        bossBar.SetActive(true);
    }

    private void EndingSequence()
    {
        focus.SetActive(true);
        focusAnim.SetTrigger("ending");
        itemUsageScript.FreezePlayer();
        hud.SetActive(false);
        uiController.pauseToggle.enabled = false;
        cameraController.credits = true;
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
