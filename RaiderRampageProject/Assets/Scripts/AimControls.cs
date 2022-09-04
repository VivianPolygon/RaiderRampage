using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimControls : MonoBehaviour
{
    private bool tracking;

    [Header("LD Dont Touch")]
    [SerializeField]
    Canvas cursorCanvas;
    [SerializeField]
    private RectTransform controlStickRect;
    [SerializeField]
    private RectTransform cursorRect;



    //movespeed for the courser on each axis
    [Header("Cursor Speed Along Each Axis")]
    [Header("LD Touch")]
    [SerializeField]
    Vector2 cursorMoveSpeed;

    //bounds in percents, (0-100) of the screen
    [Header("Cursor Bounds as Screen Percents")]
    [SerializeField]
    private Vector2 cursorBounds;
    [SerializeField]
    private Vector2 cursorBoundsOffset;

    //adjustments
    //adjusts screensize based on canvas scaling
    private Vector2 adjustedScreensize;

    //applied bounds of the cursor
    private Vector2 boundsMin;
    private Vector2 boundsMax;

    //new position for the cursor
    private Vector2 newCursorPosition;


    private void Start()
    {
        UpdateCursorBounds(cursorBounds.x, cursorBounds.y, cursorBoundsOffset.x, cursorBoundsOffset.y);
        UpdateCursorSpeed(cursorMoveSpeed.x, cursorMoveSpeed.y);

        CenterCursor();

        GetCursorWorldspace();
        GunData.instance.PointGunAtCursor();
    }

    public void StartTrackMovement()
    {
        tracking = true;
    }


    public void StopTrackMovement()
    {
        tracking = false;
    }

    private void Update()
    {
        if (tracking)
        {
            newCursorPosition.x += ((controlStickRect.anchoredPosition.x * Time.deltaTime) * cursorMoveSpeed.x);
            newCursorPosition.y += ((controlStickRect.anchoredPosition.y * Time.deltaTime) * cursorMoveSpeed.y);

            newCursorPosition.x = Mathf.Clamp(newCursorPosition.x, boundsMin.x, boundsMax.x);
            newCursorPosition.y = Mathf.Clamp(newCursorPosition.y, boundsMin.y, boundsMax.y);

            cursorRect.anchoredPosition = newCursorPosition;

            GetCursorWorldspace();
            GunData.instance.PointGunAtCursor();
        }

    }

    public void UpdateCursorBounds(float xBoundsPercent, float yBoundsPercent, float xOffsetPercent, float yOffsetPercent)
    {
        //adjusts canvas scaling to screen scaling
        adjustedScreensize.x = Screen.width / cursorCanvas.scaleFactor;
        adjustedScreensize.y = Screen.height / cursorCanvas.scaleFactor;

        //clamps the numbes to a percent and divides by 2 efectivly for caculation
        xBoundsPercent = Mathf.Clamp(xBoundsPercent, 0, 100) / 100;
        yBoundsPercent = Mathf.Clamp(yBoundsPercent, 0, 100) / 100;
        //sets the bounds in percent for the x values
        boundsMin.x = 0;
        boundsMax.x = adjustedScreensize.x * xBoundsPercent;
        //sets the bounds in percent for the y values
        boundsMin.y = 0;
        boundsMax.y = adjustedScreensize.y * yBoundsPercent;

        //converts and clamps offsts to a percent
        xOffsetPercent = Mathf.Clamp(xOffsetPercent, 0, 100) / 100;
        yOffsetPercent = Mathf.Clamp(yOffsetPercent, 0, 100) / 100;
        //applys the offsets on the x axis
        boundsMin.x += (adjustedScreensize.x * xOffsetPercent);
        boundsMax.x += (adjustedScreensize.x * xOffsetPercent);
        //applys the offsets on the y axis
        boundsMin.y += (adjustedScreensize.y * yOffsetPercent);
        boundsMax.y += (adjustedScreensize.y * yOffsetPercent);
    }

    public void UpdateCursorSpeed(float xSpeed, float ySpeed)
    {
        cursorMoveSpeed.x = xSpeed;
        cursorMoveSpeed.y = ySpeed;
    }



    public void CenterCursor()
    {
        cursorRect.anchoredPosition = (boundsMax + boundsMin) / 2;
        newCursorPosition = cursorRect.anchoredPosition;
    }

    private void GetCursorWorldspace()
    {
        GunData.instance.cursorPositon = cursorRect.transform.position;
    }

}
