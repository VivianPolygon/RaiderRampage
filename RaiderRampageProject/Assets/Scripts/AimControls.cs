using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AimControls : MonoBehaviour
{
    [SerializeField]
    RectTransform controlStick;

    private List<Touch> touches;

    private RectTransform stickInitialPosition;

    private Vector3 stickOffset;

    private bool tracking;


    [SerializeField]
    private Canvas UICanvas;

    Vector2 previousPosition;
    Vector2 movement;
    Vector2 mousePosition;

    [SerializeField]
    float distance;

    float currentdistance;



    private void Start()
    {
        stickInitialPosition = controlStick;
        stickInitialPosition.anchoredPosition = controlStick.anchoredPosition;
    }

    private void Update()
    {
        if(tracking)
        {
            mousePosition = Input.mousePosition;

            movement = mousePosition - previousPosition;

            previousPosition = mousePosition;

            currentdistance = Mathf.Abs((controlStick.anchoredPosition.x + movement.x)) + Mathf.Abs((controlStick.anchoredPosition.y + movement.y));
            if (currentdistance > distance)
            {
                movement /= currentdistance;
            }
            controlStick.anchoredPosition += (movement / UICanvas.scaleFactor);
            print(distance);

        }
    }

    public void TrackMovement()
    {
        previousPosition = Input.mousePosition;
        tracking = true;
    }


    public void Release()
    {
        tracking = false;
        controlStick.anchoredPosition = Vector2.zero;
    }


}
