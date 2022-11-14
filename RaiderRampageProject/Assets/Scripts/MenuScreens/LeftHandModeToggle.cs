using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftHandModeToggle: MonoBehaviour
{
    [SerializeField] private Toggle _localLeftHandToggle;

    private void Start()
    {
        SettingsManager.leftHandToggle = _localLeftHandToggle;
    }

    public void SetToggle()
    {
        SettingsManager.SetToggleLeftHandMode();
    }
}
