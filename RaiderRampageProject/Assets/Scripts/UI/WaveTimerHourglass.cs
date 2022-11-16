using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveTimerHourglass : MonoBehaviour
{
    private Image hourglassFillImage;

    private void Start()
    {
        if (TryGetComponent(out Image image))
        {
            hourglassFillImage = image;
            hourglassFillImage.fillAmount = 0;
        }
        else
        {
            Debug.LogWarning(name + ": dosent have an image component on it, script has been deleted");
            Destroy(this);
        }
    }

    private void Update()
    {
        //only updates if not shooting
        if(GameStateManager.instance.gameState != GameStateManager.Gamestate.Shooting)
        {
            hourglassFillImage.fillAmount = 1 -(MergescreenTimer.currentTimerPlace / MergescreenTimer.waveTimerDuration);
        }
    }
}
