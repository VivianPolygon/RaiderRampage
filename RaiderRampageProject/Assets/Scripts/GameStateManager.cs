using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//Script that controls the overall gamestate
public class GameStateManager : MonoBehaviour
{
    //used as a singleton
    static public GameStateManager instance;

    //initial state, set in the inspector
    [SerializeField]
    private Gamestate startingState;
    //state of the level, used to determine if its a level or the shooting range
    [SerializeField]
    private LevelMode levelMode = LevelMode.StandardLevel;


    public event Action onWaveEnd;
    public event Action onWaveStart;

    public void WaveEnd()
    {
        onWaveEnd?.Invoke();
        //Debug.log("Wave Ended");
    }
    public void WaveStart()
    {
        onWaveStart?.Invoke();
        //Debug.log("Wave Started");
    }

    //enum for gamestates
    public enum Gamestate
    {
        Shooting,
        BetweenWaves,
        InInventory,
        Paused
    }
    public enum InventoryUIState
    {
        MergeScreen,
        UpgradeScreen
    }
    public enum LevelMode
    {
        StandardLevel,
        ShootingRange
    }

    //current gamestate
    [HideInInspector]
    public Gamestate gameState;
    //previous gamestate, will be needed for pausing
    private Gamestate previousGamestate;

    //establishes a singleton
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


    }

    private void Start()
    {
        //sets the initial state from input in the inspector


        InitilizeFromLevelMode();
    }

    //switch statement used to update the gamestate
    public void UpdateGameState(int newState)
    {
        gameState = (Gamestate)newState;

        switch (levelMode)
        {
            case LevelMode.StandardLevel:
                //controls standard levels flow
                switch (gameState)
                {
                    case Gamestate.Shooting:
                        SetInventoryCanvases(false);
                        BeginShootingState(true);
                        WaveStart();
                        break;
                    case Gamestate.BetweenWaves:
                        WaveEnd();
                        UIEvents.instance.UpdateAll();
                        break;
                    case Gamestate.InInventory:
                        UIEvents.instance.UpdateAll();
                        UIData.instance.shootingControlsCanvas.enabled = false;
                        UpdateInventoryState(0);
                        break;
                    case Gamestate.Paused:
                        PlayerResourcesManager.ammoRegen = false;

                        break;
                    default:
                        break;
                }
                break;
                //controls shooting range's flow
            case LevelMode.ShootingRange:
                switch (gameState)
                {
                    case Gamestate.Shooting:
                        SetInventoryCanvases(false);
                        BeginShootingState(true);
                        break;
                    case Gamestate.BetweenWaves:
                        break;
                    case Gamestate.InInventory:
                        UIEvents.instance.UpdateAll();
                        UIData.instance.shootingControlsCanvas.enabled = false;
                        UpdateInventoryState(0);
                        break;
                    case Gamestate.Paused:
                        PlayerResourcesManager.ammoRegen = false;

                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }


    }

    //function called when switching to the merge state
    public void UpdateInventoryState(int state)
    {
        PlayerResourcesManager.ammoRegen = true;
        StartCoroutine(RotatePlayerInventory(state));
    }

    //function called when switching the the begin shooting state
    private void BeginShootingState(bool turn)
    {
        PlayerResourcesManager.ammoRegen = true;
        GunData.instance.ScrollGunHead(0);
        if(turn)
        {
            StartCoroutine(RotatePlayerForward());
        }
        else
        {
            UIData.instance.shootingControlsCanvas.enabled = true;
            Camera.main.transform.rotation = Quaternion.AngleAxis(0, Camera.main.transform.up);
        }
    }

    //TEMPORARY
    //function used to rotate the player backwards, facing the workshop
    private IEnumerator RotatePlayerInventory(int state)
    {
        float startingRotation = Camera.main.gameObject.transform.rotation.eulerAngles.y;

        switch ((InventoryUIState)state)
        {
            case InventoryUIState.MergeScreen:

                UIData.instance.upgradesCanvas.enabled = false;

                for (float i = 0; i < 1; i += Time.deltaTime)
                {
                    Camera.main.transform.Rotate(0, (130 - startingRotation) * Time.deltaTime, 0);

                    yield return null;
                }

                Camera.main.transform.rotation = Quaternion.AngleAxis(130, Camera.main.transform.up);

                UIData.instance.mergeingCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                UIData.instance.mergeingCanvas.enabled = true;
                break;

            case InventoryUIState.UpgradeScreen:

                UIData.instance.mergeingCanvas.enabled = false;

                for (float i = 0; i < 1; i += Time.deltaTime)
                {
                    Camera.main.transform.Rotate(0, (230 - startingRotation) * Time.deltaTime, 0);

                    yield return null;
                }

                Camera.main.transform.rotation = Quaternion.AngleAxis(230, Camera.main.transform.up);

                UIData.instance.upgradesCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                UIData.instance.upgradesCanvas.enabled = true;
                break;
            default:
                break;
        }

    }

    //TEMPORARY
    //function used to rotate the player forwards, facing the level
    private IEnumerator RotatePlayerForward()
    {
        float startingRotation = Camera.main.gameObject.transform.rotation.eulerAngles.y;

        if (startingRotation > 180)
        {
            startingRotation = startingRotation - 180;

            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                Camera.main.transform.Rotate(0, (180 - startingRotation) * Time.deltaTime, 0);
                yield return null;
            }
        }
        else
        {
            startingRotation = 180 - startingRotation;

            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                Camera.main.transform.Rotate(0, (-180 + startingRotation) * Time.deltaTime, 0);
                yield return null;
            }
        }

        UIData.instance.shootingControlsCanvas.enabled = true;

        Camera.main.transform.rotation = Quaternion.AngleAxis(0, Camera.main.transform.up);

    }

    //sets both inventory canvases to active or inactive, used for switching to the shooting state
    private void SetInventoryCanvases(bool active)
    {

        if (!active)
        {
            UIData.instance.mergeingCanvas.renderMode = RenderMode.WorldSpace;
            UIData.instance.upgradesCanvas.renderMode = RenderMode.WorldSpace;
        }

        UIData.instance.mergeingCanvas.enabled = active;
        UIData.instance.upgradesCanvas.enabled = active;

    }


    private void InitilizeFromLevelMode()
    {
        switch (levelMode)
        {
            case LevelMode.StandardLevel:
                //sets gamestate to shooting by default
                UpdateGameState((int)Gamestate.Shooting);
                break;
            case LevelMode.ShootingRange:
                UpdateGameState((int)Gamestate.Shooting);
                break;
            default:
                break;
        }
    }
}
