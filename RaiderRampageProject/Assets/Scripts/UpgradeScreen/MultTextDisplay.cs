using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultTextDisplay : MonoBehaviour
{
    public enum MultTextType
    {
        damageMult,
        fireSpeedMult
    }

    [SerializeField] private MultTextType multTextDisplay;
    private Text displayText;
    

    private void Start()
    {
        if (TryGetComponent(out Text text))
        {
            displayText = text;
        }   
        else
        {
            Debug.LogWarning("Mult Display Text Object: " + name + " does not have a Text component attatched, doublecheck to make sure it does, destroyed at runtime");
            Destroy(this.gameObject);
        }

    }

    private void OnEnable()
    {
        UIEvents.instance.onUpdateMults += UpdateText;
    }
    private void OnDisable()
    {
        UIEvents.instance.onUpdateMults -= UpdateText;
    }

    public void UpdateText()
    {
        switch (multTextDisplay)
        {
            case MultTextType.damageMult:
                displayText.text = ("Damage x " + PlayerResourcesManager.damageMult);
                break;
            case MultTextType.fireSpeedMult:
                displayText.text = ("Fire Rate x " + PlayerResourcesManager.fireSpeedMult);
                break;
            default:
                break;
        }

    }
}
