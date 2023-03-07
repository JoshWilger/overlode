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
    [SerializeField] private GameObject help;
    [SerializeField] private GameObject message;
    [SerializeField] private GameObject charging;
    [SerializeField] private GameObject factory;
    [SerializeField] private GameObject garage;
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject boss;
    [SerializeField] private TextMeshProUGUI depth;
    [SerializeField] private Animator earthquakeWarn;
    [SerializeField] private AudioSource exitAud;
    [SerializeField] private AudioSource pauseAud;
    [SerializeField] private AudioSource musicAud;
    [SerializeField] private AudioClip exitSound;
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip shopMusic;
    [SerializeField, Range(0, 1)] private float earthquakeChance;
    [SerializeField] private int badAltimeterDepth;
    [SerializeField] private Sprite bossBackground;
    [SerializeField] private SpriteRenderer background;

    public Toggle pauseToggle;
    public int[] messageDepths;

    private BoxCollider2D coll;
    private Energy energyScript;
    private Sprite regularBackground;
    private AudioClip previousAudio;
    private AudioSource aud;
    private float previousTimestamp;
    private bool exited = false;

    // Start is called before the first frame update
    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        energyScript = GetComponent<Energy>();
        aud = GetComponent<AudioSource>();
        regularBackground = background.sprite;
        pauseToggle.onValueChanged.AddListener((value) =>
        {
            Paused(value);
            pauseAud.Play();
            AudioListener.pause = value;
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

        if (!(charging.activeSelf || factory.activeSelf || garage.activeSelf || shop.activeSelf) && !exited)
        {
            exited = true;
            musicAud.clip = previousAudio;
            musicAud.time = previousTimestamp;
            musicAud.Play();
        }
    }

    private void BackgroundChange(bool isBoss = false)
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
        aud.mute = isPressed;
    }

    private void EscapePressed(bool isPressed)
    {
        if (isPressed)
        {
            if (message.activeSelf)
            {
                message.SetActive(false);
                focus.SetActive(false);
                pauseToggle.interactable = true;
            }
            else if (charging.activeSelf)
            {
                charging.SetActive(false);
                focus.SetActive(false);
                pauseToggle.interactable = true;
            }
            else if (factory.activeSelf)
            {
                factory.SetActive(false);
                focus.SetActive(false);
                pauseToggle.interactable = true;
            }
            else if (garage.activeSelf)
            {
                garage.SetActive(false);
                focus.SetActive(false);
                pauseToggle.interactable = true;
            }
            else if (shop.activeSelf)
            {
                shop.SetActive(false);
                focus.SetActive(false);
                pauseToggle.interactable = true;
            }
            else if (help.activeSelf)
            {
                help.SetActive(false);
                focus.SetActive(false);
                Paused(false);
                pauseToggle.interactable = true;
            }
            else
            {
                pauseToggle.isOn = !pauseToggle.isOn;
                return;
            }
            exitAud.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool collided = false;
        pauseToggle.interactable = false;

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
            exited = false;
            focus.SetActive(true);
            energyScript.decreaseEnergy = false;

            if (musicAud.clip != null)
            {
                previousAudio = musicAud.clip;
                previousTimestamp = musicAud.time;
                musicAud.clip = shopMusic;
                musicAud.time = 75;
                musicAud.Play();
            }
            else
            {
                exited = true;
                pauseToggle.interactable = true;
            }
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
            if (!exited)
            {
                musicAud.clip = previousAudio;
                musicAud.time = previousTimestamp;
                musicAud.Play();
            }
            exited = true;
            focus.SetActive(false);
            energyScript.decreaseEnergy = true;
            pauseToggle.interactable = true;
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
