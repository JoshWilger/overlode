using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeOnLoad : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    private void Awake()
    {
        LoadVolume();
    }

    private void LoadVolume()
    {
        float musicVolume = PlayerPrefs.GetFloat(VolumeSettings.MIXER_MUSIC, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(VolumeSettings.MIXER_SFX, 1f);

        mixer.SetFloat(VolumeSettings.MIXER_MUSIC, Mathf.Log10(musicVolume) * 20);
        mixer.SetFloat(VolumeSettings.MIXER_SFX, Mathf.Log10(sfxVolume) * 20);
    }
}
