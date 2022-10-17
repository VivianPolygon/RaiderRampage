using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergescreenTimer : MonoBehaviour
{
    // Start is called before the first frame update

    public int timerDuration = 30;

    private Coroutine timerCoroutine;


    private void Start()
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
    private IEnumerator Timer()
    {
        for (float i = 0; i < timerDuration; i += Time.deltaTime)
        {
            //causes timer icons from UIData to flash, speeding up as the time gets lower
            foreach (Image timerIcon in UIData.instance.timerIndicators)
            {
                timerIcon.color = Color.Lerp(Color.grey, Color.red, i / timerDuration) * Mathf.RoundToInt(Mathf.Clamp(Mathf.Sin(i * i), 0, 1));
            }
            //stops the coroutine if the shootingstate is entered manualy before the timer is up
            if (GameStateManager.instance.gameState == GameStateManager.Gamestate.Shooting)
            {

                StopCoroutine(timerCoroutine);
            }
            yield return null;
        }
        if(GameStateManager.instance.gameState != GameStateManager.Gamestate.Shooting)
        {
            //cancels coroutines used for other rotations, if timed up
            GameStateManager.instance.StopAllCoroutines();
            //drops picked up barrels and addons back to their previous spot when booted out of
            MergeingInputDetection.instance.MergingScreenCanceled();
            //changed the state to shooting
            GameStateManager.instance.UpdateGameState((int)GameStateManager.Gamestate.Shooting);
        }
    }
}