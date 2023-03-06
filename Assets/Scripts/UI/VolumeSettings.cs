using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public static string MIXER_MUSIC = "MusicVolume";
    public static string MIXER_SFX = "SFXVolume";

    private void OnEnable()
    {
        if (musicSlider != null && sfxSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat(MIXER_MUSIC, 1f);
            sfxSlider.value = PlayerPrefs.GetFloat(MIXER_SFX, 1f);

            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }

    private void OnDisable()
    {
        if (musicSlider != null && sfxSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.RemoveAllListeners();

            PlayerPrefs.SetFloat(MIXER_MUSIC, musicSlider.value);
            PlayerPrefs.SetFloat(MIXER_SFX, sfxSlider.value);
        }
    }

    private void SetMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
    }

    private void SetSfxVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
    }
}