using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergescreenTimer : MonoBehaviour
{
    // Start is called before the first frame update

    public int timerDuration = 30;

    private Coroutine timerCoroutine;

    //static versions of variables set in start and coroutines
    public static int waveTimerDuration;
    public static float currentTimerPlace;

    private void OnEnable()
    {
        GameStateManager.instance.onWaveEnd += StartTimer;     
    }

    public void StartTimer()
    {
        timerCoroutine = StartCoroutine(Timer());
    }
    private void OnDisable()
    {
        GameStateManager.instance.onWaveEnd -= StartTimer;
    }

    private void Start()
    {
        waveTimerDuration = timerDuration;
    }

    private IEnumerator Timer()
    {
        if (UIData.instance.timerIndicators == null)
        {
            //prevents errors by preventing from running if array is null, hapens when exiting out of the game on ocasion
        }
        else
        {
            for (float t = 0; t < timerDuration; t += Time.deltaTime)
            {
                currentTimerPlace = t;

                //causes timer icons from UIData to flash, speeding up as the time gets lower
                foreach (Image timerIcon in UIData.instance.timerIndicators)
                {
                    if(t > waveTimerDuration / 2) //only flashes if there is half time left or less
                    {
                        timerIcon.color = Color.Lerp(Color.grey, Color.red, t / timerDuration);
                    }
                }
                //stops the coroutine if the shootingstate is entered manualy before the timer is up
                if (GameStateManager.instance.gameState == GameStateManager.Gamestate.Shooting)
                {

                    StopCoroutine(timerCoroutine);
                }
                yield return null;
            }
            if (GameStateManager.instance.gameState != GameStateManager.Gamestate.Shooting)
            {
                //cancels coroutines used for other rotations, if timed up
                GameStateManager.instance.StopAllCoroutines();
                //drops picked up barrels and addons back to their previous spot when booted out of
                MergeingInputDetection.instance.MergingScreenCanceled();
                //changed the state to shooting
                GameStateManager.instance.UpdateGameState((int)GameStateManager.Gamestate.Shooting);
            }
        }
        currentTimerPlace = waveTimerDuration;
    }
}
