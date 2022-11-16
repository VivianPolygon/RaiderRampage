using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public static StartMenuManager instance;

    public enum MenuActive
    {
        noneActive,
        levelSelect,
        shootingRange,
        loading,
        settings
    }

    [Header("UI Canvases")]

    [SerializeField]
    private GameObject MenuCanvas;

    //canvases for each menu
    [SerializeField]
    private GameObject levelSelectCanvas;
    [SerializeField]
    private GameObject shootingRangeCanvas;
    [SerializeField]
    private GameObject settingsCanvas;
    [SerializeField]
    private GameObject loadingCanvas;
    //array made from all of the canvases
    private GameObject[] canvases;




    [Header("Loadbar Slider")]
    [SerializeField]
    private Slider loadbarSlider;


    [HideInInspector]
    public MenuActive menuState;

    private AsyncOperation loadingOperation;

    private void Awake()
    {
        if(instance != null)
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
        //loads whenever the scene is opened
        if(ProgressManager.instance != null)
        {
            ProgressManager.instance.Load();
        }

        //sets the canvas array
        canvases = new GameObject[4];
        canvases[0] = levelSelectCanvas;
        canvases[1] = shootingRangeCanvas;
        canvases[2] = settingsCanvas;
        canvases[3] = loadingCanvas;

        UpdateDisplayedMenu(0);
    }

    public void UpdateDisplayedMenu(int newState)
    {
        menuState = (MenuActive)newState;

        switch (menuState)     
        {
            case MenuActive.levelSelect:
                SetSingleCanvasActive(levelSelectCanvas);
                break;
            case MenuActive.shootingRange:
                SetSingleCanvasActive(shootingRangeCanvas);
                break;
            case MenuActive.loading:
                SetSingleCanvasActive(loadingCanvas);
                break;
            case MenuActive.settings:
                SetSingleCanvasActive(settingsCanvas);
                break;
            case MenuActive.noneActive:
                SetSingleCanvasActive(null);
                break;
            default:
                break;
        }
    }

    public void LoadScene(int sceneNumber)
    {
        //sets timescale to one to prevent a quitout from game paused causing issues
        Time.timeScale = 1;

        loadingOperation = SceneManager.LoadSceneAsync(sceneNumber);
        UpdateDisplayedMenu(3);
        StartCoroutine(LoadingBar());
    }

    private IEnumerator LoadingBar()
    {
        float progress = 0;
        while(!loadingOperation.isDone)
        {
            progress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            loadbarSlider.value = progress;
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetSingleCanvasActive(GameObject canvasActive)
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            if(canvases[i] == canvasActive)
            {
                canvases[i].SetActive(true);
            }
            else
            {
                canvases[i].SetActive(false);
            }
        }
    }
}
