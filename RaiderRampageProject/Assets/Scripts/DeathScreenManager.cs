using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private Color fadeColor;

    [SerializeField] private float fadeTime;

    [SerializeField] private Text gameOverText;
    [SerializeField] private string[] gameOverTexts;


    private void Start()
    {
        StartCoroutine(DeathScreenFadeOut());
        SetGameoverText();
    }

    private IEnumerator DeathScreenFadeOut()
    {
        for (float i = 0; i < fadeTime; i += Time.deltaTime)
        {
            fadeImage.color = Color.Lerp(fadeColor, Color.clear, (i / fadeTime));

            yield return null;
        }

        fadeImage.color = Color.clear;
    }

    //for gameover button

    public void LoadStartMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetGameoverText()
    {
        gameOverText.text = gameOverTexts[Random.Range(0, gameOverTexts.Length)];
    }

}
