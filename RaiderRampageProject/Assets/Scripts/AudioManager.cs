using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    private static AudioMixer _mixer;

    public AudioMixerGroup music;
    public AudioMixerGroup SFX;

    public static float musicVolume;
    public static float SFXVolume;

    private const string _musicVolume = "_musicVolume";
    private const string _SFXVolume = "_SFXVolume";


    public static void SetMusicVolume(float newValue)
    {
        musicVolume = newValue;
        UpdateAudioValues();
    }
    public static void SetSFXVolume(float newValue)
    {
        SFXVolume = newValue;
        UpdateAudioValues();
    }


    public static void UpdateAudioValues()
    {
        if(_mixer != null)
        {
            _mixer.SetFloat(_musicVolume, (Mathf.Log10(musicVolume) * 20));
            _mixer.SetFloat(_SFXVolume, (Mathf.Log10(SFXVolume) * 20));
        }
        else
        {
            Debug.LogWarning("Instance of Mixer could not be set to static");
        }
    }

    private void Awake()
    {
        //sets the instance of mixer
        _mixer = mixer;
    }

}
