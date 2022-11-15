using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingUIFlip : MonoBehaviour
{
    [Header("Fire Button")]
    [SerializeField] private Image fireButton;
    [SerializeField] private RectTransform fireButtonRight;
    [SerializeField] private RectTransform fireButtonLeft;
    [Header("Reload Button")]
    [SerializeField] private Image reloadButton;
    [SerializeField] private RectTransform reloadButtonRight;
    [SerializeField] private RectTransform reloadButtonLeft;
    [Header("Grenade Button")]
    [SerializeField] private Image grenadeButton;
    [SerializeField] private RectTransform grenadeButtonRight;
    [SerializeField] private RectTransform grenadeButtonLeft;

    [Header("Overdrive Activator")]
    [SerializeField] private Image overdriveActivator;
    [SerializeField] private RectTransform overdriveActivatorRight;
    [SerializeField] private RectTransform overdriveActivatorLeft;

    [Header("Control Stick (Back)")]
    [SerializeField] private Image controlStick;
    [SerializeField] private RectTransform controlStickRight;
    [SerializeField] private RectTransform controlStickLeft;


    private void Start()
    {
        OrientUI();
    }

    public void OrientUI()
    {
        if (SettingsManager.leftHandMode)
        {
            SetLeft();
        }
        else if(!SettingsManager.leftHandMode)
        {
            SetRight();
        }
    }

    public void SetLeft()
    {
        SetFireButton(fireButtonLeft);
        SetReloadButton(reloadButtonLeft);
        SetGrenadeButton(grenadeButtonLeft);
        SetOverdriveActivator(overdriveActivatorLeft);
        SetControlStick(controlStickLeft);
    }
    public void SetRight()
    {
        SetFireButton(fireButtonRight);
        SetReloadButton(reloadButtonRight);
        SetGrenadeButton(grenadeButtonRight);
        SetOverdriveActivator(overdriveActivatorRight);
        SetControlStick(controlStickRight);
    }


    private void SetFireButton(RectTransform rect)
    {
        RectTransform buttonRect = fireButton.gameObject.GetComponent<RectTransform>();

        buttonRect.anchorMin = rect.anchorMin;
        buttonRect.anchorMax = rect.anchorMax;
        buttonRect.anchoredPosition = rect.anchoredPosition;
        buttonRect.sizeDelta = rect.sizeDelta;
    }
    private void SetReloadButton(RectTransform rect)
    {
        RectTransform buttonRect = reloadButton.gameObject.GetComponent<RectTransform>();

        buttonRect.anchorMin = rect.anchorMin;
        buttonRect.anchorMax = rect.anchorMax;
        buttonRect.anchoredPosition = rect.anchoredPosition;
        buttonRect.sizeDelta = rect.sizeDelta;
    }
    private void SetGrenadeButton(RectTransform rect)
    {
        RectTransform buttonRect = grenadeButton.gameObject.GetComponent<RectTransform>();

        buttonRect.anchorMin = rect.anchorMin;
        buttonRect.anchorMax = rect.anchorMax;
        buttonRect.anchoredPosition = rect.anchoredPosition;
        buttonRect.sizeDelta = rect.sizeDelta;
    }
    private void SetOverdriveActivator(RectTransform rect)
    {
        RectTransform buttonRect = overdriveActivator.gameObject.GetComponent<RectTransform>();

        buttonRect.anchorMin = rect.anchorMin;
        buttonRect.anchorMax = rect.anchorMax;
        buttonRect.anchoredPosition = rect.anchoredPosition;
        buttonRect.sizeDelta = rect.sizeDelta;
    }
    private void SetControlStick(RectTransform rect)
    {
        RectTransform buttonRect = controlStick.gameObject.GetComponent<RectTransform>();

        buttonRect.anchorMin = rect.anchorMin;
        buttonRect.anchorMax = rect.anchorMax;
        buttonRect.anchoredPosition = rect.anchoredPosition;
        buttonRect.sizeDelta = rect.sizeDelta;
    }
}
