using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIFlash : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashSpeed;

    [SerializeField] public bool isFlashing;


    // Update is called once per frame
    void Update()
    {
        Flash();
    }

    private void Flash()
    {
        if(isFlashing)
        {
            if(Mathf.RoundToInt(Time.time * flashSpeed) % 2 == 0)
            {
                flashImage.enabled = true;
            }
            else
            {
                flashImage.enabled = false;
            }
        }
    }
}
