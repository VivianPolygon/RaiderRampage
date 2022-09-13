using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    static public GameStateManager instance;

    [SerializeField]
    private Gamestate startingState;

    private bool facingForward;

    public enum Gamestate
    {
        Shooting,
        BetweenWaves,
        Mergeing,
        Paused
    }

    public Gamestate gameState;
    private Gamestate previousGamestate;

    void Awake()
    {
        if (instance != null) 
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        facingForward = true;
    }

    private void Start()
    {
        UpdateGameState((int)startingState);
    }

    public void UpdateGameState(int newState)
    {
        gameState = (Gamestate)newState;

        switch (gameState)
        {
            case Gamestate.Shooting:
                BeginShootingState();
                break;
            case Gamestate.BetweenWaves:
                BeginShootingState();
                break;
            case Gamestate.Mergeing:
                BeginMergeState();
                break;
            case Gamestate.Paused:
                PlayerResourcesManager.ammoRegen = false;

                break;
            default:
                break;
        }
    }

    private void BeginMergeState()
    {
        PlayerResourcesManager.ammoRegen = true;
        StartCoroutine(RotatePlayerBack());
    }

    private void BeginShootingState()
    {
        PlayerResourcesManager.ammoRegen = true;
        GunData.instance.SwitchGunHeads(GunData.instance.currentHeadNumber);
        StartCoroutine(RotatePlayerForward());
    }


    private IEnumerator RotatePlayerBack()
    {
        UIData.instance.shootingControlsCanvas.enabled = false;

        if (facingForward)
        {
            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                Camera.main.transform.Rotate(0, 180 * Time.deltaTime, 0);

                yield return null;
            }
            UIData.instance.mergeingCanvas.enabled = true;

            Camera.main.transform.rotation = Quaternion.AngleAxis(180, Camera.main.transform.up);
            facingForward = false;
        }
    }

    private IEnumerator RotatePlayerForward()
    {
        UIData.instance.mergeingCanvas.enabled = false;

        if (!facingForward)
        {
            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                Camera.main.transform.Rotate(0, -180 * Time.deltaTime, 0);

                yield return null;
            }
            UIData.instance.shootingControlsCanvas.enabled = true;

            Camera.main.transform.rotation = Quaternion.AngleAxis(0, Camera.main.transform.up);
            facingForward = true;
        }

    }

}
