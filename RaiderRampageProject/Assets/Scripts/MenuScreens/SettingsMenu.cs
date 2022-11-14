using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    //Aim Sensitivity Slider
    [SerializeField] private Slider sensitivitySlider;
    //Aim Sensitivity Text
    [SerializeField] private Text sensitivityText;


    private void Start()
    {
        LoadSettingsData();
    }

    //ran on the value changing on the slider
    public void SetAimSensitivity()
    {
        SettingsManager.SetAimSensitivity(sensitivitySlider.value);
        sensitivityText.text = "Aim Sensitivity: " + sensitivitySlider.value.ToString();
    }

    public void LoadSettingsData()
    {
        SettingsManager.LoadSettingsData();

        sensitivitySlider.value = SettingsManager.aimSensitivity * 10;
        SetAimSensitivity();
    }

}
