using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private int[] levelSceneNumbers;
    [SerializeField] private Sprite[] levelSprites;
    [SerializeField] private string[] levelNames;

    [SerializeField] private Text nameText;
    [SerializeField] private Image levelImage;
    [SerializeField] private Button startButton;

    private Slider levelSelectSlider;
    private int levelQuantity;


    public int startNum;

    private bool isInitilized;

    // Start is called before the first frame update
    void Start()
    {
        isInitilized = false;

        levelQuantity = levelSceneNumbers.Length;

        if(TryGetComponent(out Slider slider))
        {
            slider.minValue = 1;
            slider.maxValue = levelQuantity;
            slider.wholeNumbers = true;

            slider.value = Mathf.Clamp(startNum, 1, levelQuantity);

            levelSelectSlider = slider;
        }
        else
        {
            Debug.LogWarning("LevelSelect.cs Object does not have a Slider component, located on: " + name);
        }


        isInitilized = true;
    }

    //function that updates select
    public void UpdateSelect()
    {
        if(isInitilized)
        {
            if((int)levelSelectSlider.value > ProgressManager.highestLevelCompleted)
            {
                startButton.interactable = false;
            }
            else
            {
                startButton.interactable = true;
            }

            nameText.text = levelNames[(int)levelSelectSlider.value - 1];
            levelImage.sprite = levelSprites[(int)levelSelectSlider.value - 1];
        }

    }

    public void LoadSelectedLevel()
    {
        StartMenuManager.instance.LoadScene(levelSceneNumbers[(int)levelSelectSlider.value - 1]);
    }
}
