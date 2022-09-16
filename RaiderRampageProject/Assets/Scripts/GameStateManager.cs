using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Script that controls the overall gamestate
public class GameStateManager : MonoBehaviour
{
    //used as a singleton
    static public GameStateManager instance;

    //initial state, set in the inspector
    [SerializeField]
    private Gamestate startingState;

    //TEMPORARY
    //used to control how the coroutines for rotating work
    private bool facingForward;

    //enum for gamestates
    public enum Gamestate
    {
        Shooting,
        BetweenWaves,
        Mergeing,
        Paused
    }

    //current gamestate
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

        //initilizes facing forward (assumes starting by shooting)
        facingForward = true;
    }

    private void Start()
    {
        //sets the initial state from input in the inspector
        UpdateGameState((int)startingState);
    }

    //switch statement used to update the gamestate
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

    //function called when switching to the merge state
    private void BeginMergeState()
    {
        PlayerResourcesManager.ammoRegen = true;
        StartCoroutine(RotatePlayerBack());
    }

    //function called when switching the the begin shooting state
    private void BeginShootingState()
    {
        PlayerResourcesManager.ammoRegen = true;
        GunData.instance.SwitchGunHeads(GunData.instance.currentHeadNumber);
        StartCoroutine(RotatePlayerForward());
    }

    //TEMPORARY
    //function used to rotate the player backwards, facing the workshop
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

    //TEMPORARY
    //function used to rotate the player forwards, facing the level
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
