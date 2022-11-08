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
        loading
    }

    [Header("UI Canvases")]

    [SerializeField]
    private GameObject MenuCanvas;

    [SerializeField]
    private GameObject levelSelectCanvas;
    [SerializeField]
    private GameObject shootingRangeCanvas;

    [SerializeField]
    private GameObject loadingCanvas;

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
        UpdateDisplayedMenu(0);

        //loads whenever the scene is opened
        if(ProgressManager.instance != null)
        {
            ProgressManager.instance.Load();
        }
    }

    public void UpdateDisplayedMenu(int newState)
    {
        menuState = (MenuActive)newState;

        switch (menuState)     
        {
            case MenuActive.levelSelect:
                shootingRangeCanvas.SetActive(false);
                levelSelectCanvas.SetActive(true);
                break;
            case MenuActive.shootingRange:
                levelSelectCanvas.SetActive(false);
                shootingRangeCanvas.SetActive(true);
                break;
            case MenuActive.loading:
                MenuCanvas.SetActive(false);
                loadingCanvas.SetActive(true);
                break;
            case MenuActive.noneActive:
                levelSelectCanvas.SetActive(false);
                shootingRangeCanvas.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void LoadScene(int sceneNumber)
    {
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
        Application.OpenURL("https://docs.google.com/forms/d/12PqXF_EcWWMgyVuR1Rd1FM-GsLdIBY6V60XcLjAFf9M/viewform?edit_requested=true");
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        //save game
    }
}
