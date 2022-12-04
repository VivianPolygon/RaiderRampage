using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//updates audio from the slider values using functions held on the AudioManager Script
//sliders should go between a really low valuue and 1. like 0.0001 - 1

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private Slider settingsSlider;
    [SerializeField] private Text sliderText;

    [SerializeField] private string baseText;

    public enum AudioSliderType
    {
        Music,
        SFX
    }

    [SerializeField] private AudioSliderType audioType;

    //subscribes to event to handle visual updates
    private void OnEnable()
    {
        if(ProgressManager.instance != null)
        {
            ProgressManager.instance.onLoadTriggered += UpdateFromSettings;
        }
        else
        {
            Debug.LogWarning("Progress manager instance was null and could not subscribe to load update event" + name);
        }
    }
    private void OnDisable()
    {
        if (ProgressManager.instance != null)
        {
            ProgressManager.instance.onLoadTriggered -= UpdateFromSettings;
        }
        else
        {
            //Debug.LogWarning("Progress manager instance was null and could not unsubscribe to load update event" + name);
        }
    }

    public void SliderUpdate()
    {
        //sets the audio settings
        if(audioType == AudioSliderType.Music)
        {
            AudioManager.SetMusicVolume(settingsSlider.value);
        }
        else if(audioType == AudioSliderType.SFX)
        {
            AudioManager.SetSFXVolume(settingsSlider.value);
        }
        //sets the text between 1 and 10
        sliderText.text = baseText + Mathf.RoundToInt(settingsSlider.value * 10).ToString(); 
    }

    public void UpdateFromSettings()
    {
        //sets the audio settings
        if (audioType == AudioSliderType.Music)
        {
            settingsSlider.value = AudioManager.musicVolume;
        }
        else if (audioType == AudioSliderType.SFX)
        {
            settingsSlider.value = AudioManager.SFXVolume;
        }
        //sets the text between 1 and 10
        sliderText.text = baseText + Mathf.RoundToInt(settingsSlider.value * 10).ToString();
    }

    public void Start()
    {
        UpdateFromSettings();
    }

}
