using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIController : MonoBehaviour
{
    [SerializeField] private HudUI hudController;
    [SerializeField] private MessageUI messageUiScript;
    [SerializeField] private TerrainGeneration terrainGenerationScript;
    [SerializeField] private GameObject focus;
    [SerializeField] private GameObject message;
    [SerializeField] private GameObject charging;
    [SerializeField] private GameObject factory;
    [SerializeField] private GameObject garage;
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject boss;
    [SerializeField] private TextMeshProUGUI depth;
    [SerializeField] private Animator earthquakeWarn;
    [SerializeField, Range(0, 1)] private float earthquakeChance;
    [SerializeField] private int badAltimeterDepth;
    [SerializeField] private Sprite bossBackground;
    [SerializeField] private SpriteRenderer background;
    
    public Toggle pauseToggle;
    public int[] messageDepths;

    private BoxCollider2D coll;
    private Energy energyScript;
    private Sprite regularBackground;

    // Start is called before the first frame update
    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        energyScript = GetComponent<Energy>();
        regularBackground = background.sprite;
        pauseToggle.onValueChanged.AddListener((value) =>
        {
            Paused(value);
        });
    }

    // Update is called once per frame
    private void Update()
    {
        var playerPos = Mathf.Round(coll.bounds.min.y * 2);

        if (playerPos < -messageDepths[messageDepths.Length - 2])
        {
            depth.text = "-6666m";
            BackgroundChange(true);
        }
        else if (playerPos < -badAltimeterDepth * 2)
        {
            depth.text = "???m";
            BackgroundChange(true);
        }
        else
        {
            depth.text = playerPos + "m";
            BackgroundChange();
        }
        if (playerPos <= -messageDepths[messageUiScript.currentMessageIndex])
        {
            message.SetActive(true);
            charging.SetActive(false);
            factory.SetActive(false);
            garage.SetActive(false);
            shop.SetActive(false);
        }
        if (messageUiScript.currentMessageIndex >= messageDepths.Length - 2 && playerPos >= -messageDepths[messageDepths.Length - 3])
        {
            boss.SetActive(false);
            messageUiScript.currentMessageIndex = messageDepths.Length - 2;
        }
        EscapePressed(Input.GetButtonDown("Cancel"));
    }

    public void BackgroundChange(bool isBoss = false)
    {
        background.sprite = isBoss ? bossBackground : regularBackground;
    }

    public void Paused(bool isPressed)
    {
        Time.timeScale = isPressed ? 0f : 1;
        hudController.enabled = !isPressed;
        energyScript.decreaseEnergy = !isPressed;
        if (!(charging.activeSelf || factory.activeSelf || garage.activeSelf || shop.activeSelf))
        {
            focus.SetActive(isPressed);
        }
    }

    private void EscapePressed(bool isPressed)
    {
        if (isPressed)
        {
            if (message.activeSelf)
            {
                message.SetActive(false);
                focus.SetActive(false);
            }
            else if (charging.activeSelf)
            {
                charging.SetActive(false);
                focus.SetActive(false);
            }
            else if (factory.activeSelf)
            {
                factory.SetActive(false);
                focus.SetActive(false);
            }
            else if (garage.activeSelf)
            {
                garage.SetActive(false);
                focus.SetActive(false);
            }
            else if (shop.activeSelf)
            {
                shop.SetActive(false);
                focus.SetActive(false);
            }
            else
            {
                pauseToggle.isOn = !pauseToggle.isOn;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool collided = false;

        if (collision.gameObject.CompareTag("Charging"))
        {
            charging.SetActive(true);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Factory"))
        {
            factory.SetActive(true);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Garage"))
        {
            garage.SetActive(true);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Shop"))
        {
            shop.SetActive(true);
            collided = true;
        }
        if (collided)
        {
            focus.SetActive(true);
            energyScript.decreaseEnergy = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool collided = false;

        if (collision.gameObject.CompareTag("Charging"))
        {
            charging.SetActive(false);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Factory"))
        {
            factory.SetActive(false);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Garage"))
        {
            garage.SetActive(false);
            collided = true;
        }
        else if (collision.gameObject.CompareTag("Shop"))
        {
            shop.SetActive(false);
            collided = true;
        }
        if (collided)
        {
            focus.SetActive(false);
            energyScript.decreaseEnergy = true;
            if (messageUiScript.currentMessageIndex > 2 && Random.value < earthquakeChance)
            {
                earthquakeWarn.SetBool("show", true);
                earthquakeWarn.speed = 4f;
                Invoke(nameof(DoEarthquake), 1.5f);
            }
        }
    }

    private void DoEarthquake()
    {
        terrainGenerationScript.Earthquake();
        earthquakeWarn.SetBool("show", false);
    }
}
