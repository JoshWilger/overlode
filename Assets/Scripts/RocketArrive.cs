using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketArrive : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject pauseToggle;
    [SerializeField] private AudioClip arriveSound;
    [SerializeField] private AudioClip departSound;

    private AudioSource aud;

    private void Awake()
    {
        aud = GetComponent<AudioSource>();
    }

    public void EnableObjects()
    {
        player.SetActive(true);
        hud.SetActive(true);
        pauseToggle.SetActive(true);
    }

    public void Arrive()
    {
        aud.clip = arriveSound;
        aud.Play();
    }
    
    public void Depart()
    {
        aud.clip = departSound;
        aud.Play();
    }

    public void DisableRocket()
    {
        gameObject.SetActive(false);
    }
}
