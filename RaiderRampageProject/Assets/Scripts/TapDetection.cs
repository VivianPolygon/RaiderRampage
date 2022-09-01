using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapDetection : MonoBehaviour
{
    private void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
            print(touchPos);
        }
    }
}
